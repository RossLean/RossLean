#nullable enable

using Garyon.Objects.Enumerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using RoseLynn;
using RoseLynn.Analyzers;
using RossLean.NameOn.Core;
using RossLean.NameOn.Core.Utilities;
using System.Collections.Generic;
using System.Linq;

// The analyzer should not be run concurrently due to the state that it preserves
#pragma warning disable RS1026 // Enable concurrent execution

namespace RossLean.NameOn;

using static Diagnostics;

using NameOfRestrictionDictionary = SymbolDictionary<NameOfRestrictionAssociation>;
using SymbolNameOfStateDictionary = SymbolDictionary<NameOfState>;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NameOfUsageAnalyzer : CSharpDiagnosticAnalyzer
{
    private readonly NameOfRestrictionDictionary restrictionDictionary = new();
    private readonly SymbolNameOfStateDictionary symbolStateDictionary = new();

    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => NAMEDiagnosticDescriptorStorage.Instance;

    public override void Initialize(AnalysisContext context)
    {
        // Concurrent execution is disabled due to the stateful profile of the analyzer
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);

        RegisterNodeActions(context);
    }

    private void RegisterNodeActions(AnalysisContext context)
    {
        // Assignment of a variable could be tracked to analyze its nameof state
        // Likewise, assignment of a field/property that is specifically attributed
        var assignmentKinds = new SyntaxKind[]
        {
            SyntaxKind.SimpleAssignmentExpression,
        };

        // Returning in a function is the equivalent of assigning the final value
        var returnKinds = new SyntaxKind[]
        {
            SyntaxKind.ReturnStatement,
        };

        // Invoking a method that requires an argument be passed 
        var invocationKinds = new SyntaxKind[]
        {
            SyntaxKind.InvocationExpression,
        };

        // Passing a method group as an argument, which could enforce nameof restrictions
        // which must be respected upon being called
        var methodGroupPassingKinds = new SyntaxKind[]
        {
            SyntaxKind.IdentifierName,
        };

        // Invokable member declarations contain arguments that could restrict nameof usage 
        var invokableDeclarationKinds = new SyntaxKind[]
        {
            SyntaxKind.MethodDeclaration,
            SyntaxKind.LocalFunctionStatement,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.DelegateDeclaration,
        };

        // Fields and properties could also restrict nameof usage
        var attributableDeclarationKinds = new SyntaxKind[]
        {
            SyntaxKind.FieldDeclaration,
            SyntaxKind.PropertyDeclaration,
        };

        context.RegisterSyntaxNodeAction(AnalyzeAssignment, assignmentKinds);
        context.RegisterSyntaxNodeAction(AnalyzeReturnAssignment, returnKinds);
    }

    private void AnalyzeReturnAssignment(SyntaxNodeAnalysisContext context)
    {
        var returnStatementNode = context.Node as ReturnStatementSyntax;
        var declaration = returnStatementNode!.GetNearestParentOfType<BaseMethodDeclarationSyntax>()!;
        var declarationSymbol = context.SemanticModel.GetDeclaredSymbol(declaration)!;
        AnalyzeFunctionRestrictions(context, declarationSymbol);
        EvaluateExpressionSubstitution(context, declarationSymbol, returnStatementNode!.Expression!);
    }

    private void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignmentNode = context.Node as AssignmentExpressionSyntax;
        var left = assignmentNode!.Left;
        var right = assignmentNode.Right;

        var operation = context.SemanticModel.GetOperation(assignmentNode);
        if (operation!.Kind is OperationKind.DeconstructionAssignment)
        {
            // in the case of tuples, analyze
        }
        else
        {
            AnalyzeSimpleAssignment(context, left, right);
        }
    }
    private void AnalyzeSimpleAssignment(SyntaxNodeAnalysisContext context, ExpressionSyntax left, ExpressionSyntax right)
    {
        // TODO: Evaluate case
        /*
         * // Assume definition
         * CustomString Function([ForceNameOf] string s, [ProhibitNameOf] string t) => s + t;
         * 
         * // Suspected that this throws
         * var s = Function(nameof(Function), "F");
         */

        var semanticModel = context.SemanticModel;
        var substitutedSymbol = semanticModel.GetSymbolInfo(left, context.CancellationToken).Symbol!;
        AnalyzeRestrictions(context, substitutedSymbol);
        EvaluateExpressionSubstitution(context, substitutedSymbol, right);
    }

    private void EvaluateExpressionSubstitution(SyntaxNodeAnalysisContext context, ISymbol substitutedSymbol, ExpressionSyntax substitutingExpression)
    {
        var semanticModel = context.SemanticModel;

        // First ensure that the assigned expression is of type string, otherwise we don't care
        var substitutingExpressionType = semanticModel.GetTypeInfo(substitutingExpression);
        if (!substitutingExpressionType.MatchesExplicitlyOrImplicitly(SpecialType.System_String))
            return;

        // Now analyze the nameof state of the assigned expression
        var state = GetNameOfExpressionState(substitutingExpression, semanticModel, out var nameofOperations);
        symbolStateDictionary.Register(substitutedSymbol, state);

        // Then evaluate whether the nameof state matches that of the assigned symbol's restrictions
        var symbolRestrictionDictionary = restrictionDictionary[substitutedSymbol];

        foreach (var nameofOperation in nameofOperations)
        {
            var namedOperationNode = nameofOperation.Argument.Syntax;
            var alias = semanticModel.GetAliasOrSymbolInfo(namedOperationNode, context.CancellationToken);
            var symbolKind = alias.GetIdentifiableSymbolKind();

            var restrictions = symbolRestrictionDictionary[symbolKind];

            EvaluateDiagnosticReport(restrictions, namedOperationNode, symbolKind);
        }

        EvaluateDiagnosticReport(symbolRestrictionDictionary.RestrictionForAllKinds, substitutingExpression, IdentifiableSymbolKind.All);

        void EvaluateDiagnosticReport(NameOfRestrictions restrictions, SyntaxNode node, IdentifiableSymbolKind symbolKinds)
        {
            if (!state.ValidForRestrictions(restrictions))
            {
                var diagnosticCreator = GetDiagnosticCreator(restrictions);
                context.ReportDiagnostic(diagnosticCreator?.Invoke(node, substitutedSymbol, symbolKinds));
            }
        }
    }

    private InvalidNameOfAssignmentDiagnosticCreator? GetDiagnosticCreator(NameOfRestrictions restrictions) => restrictions switch
    {
        NameOfRestrictions.Force => CreateNAME0001,
        NameOfRestrictions.ForceContained => CreateNAME0002,
        NameOfRestrictions.Prohibit => CreateNAME0003,
        NameOfRestrictions.ProhibitContained => CreateNAME0004,
        _ => null,
    };

    private void AnalyzeRestrictions(SyntaxNodeAnalysisContext context, ISymbol symbol)
    {
        if (restrictionDictionary.ContainsKey(symbol))
            return;

        RegisterRestrictions(symbol);
    }
    private void AnalyzeFunctionRestrictions(SyntaxNodeAnalysisContext context, BaseMethodDeclarationSyntax declaration)
    {
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(declaration)!;
        AnalyzeFunctionRestrictions(context, methodSymbol);
    }
    private void AnalyzeFunctionRestrictions(SyntaxNodeAnalysisContext context, IMethodSymbol methodSymbol)
    {
        if (restrictionDictionary.ContainsKey(methodSymbol))
            return;

        var attributes = methodSymbol.GetReturnTypeAttributes();
        var restrictions = GetRestrictionAssociation(attributes);
        restrictionDictionary.Register(methodSymbol, restrictions);
    }

    private void RegisterRestrictions(ISymbol symbol)
    {
        restrictionDictionary.Register(symbol, GetRestrictionAssociation(symbol));
    }
    private static NameOfRestrictionAssociation GetRestrictionAssociation(ISymbol symbol)
    {
        return GetRestrictionAssociation(symbol.GetAttributes());
    }
    private static NameOfRestrictionAssociation GetRestrictionAssociation(IEnumerable<AttributeData> attributes)
    {
        var restrictions = new NameOfRestrictionAssociation();

        foreach (var attribute in attributes)
        {
            var restriction = GetRestriction(attribute);
            if (restriction is null)
                continue;

            restrictions.AddKinds(NameOfRestrictionAttributeBase.GetConstructorRestrictions(attribute), restriction.Value);
        }

        return restrictions;
    }

    private static NameOfState GetNameOfExpressionState(ExpressionSyntax expression, SemanticModel model, out IEnumerable<INameOfOperation> nameofOperations)
    {
        var operation = model.GetOperation(expression);
        if (operation?.Kind is OperationKind.NameOf)
        {
            nameofOperations = new SingleElementCollection<INameOfOperation>(operation as INameOfOperation);
            return NameOfState.Whole;
        }

        var nameofNodeList = new List<INameOfOperation>();

        // Recursively attempt to find nameof expressions
        foreach (var child in expression.ChildNodes().OfType<ExpressionSyntax>())
        {
            if (GetNameOfExpressionState(child, model, out var childNameOfOperations) is not NameOfState.None)
                nameofNodeList.AddRange(childNameOfOperations);
        }

        nameofOperations = nameofNodeList;
        return nameofNodeList.Count > 0 ? NameOfState.Contained : NameOfState.None;
    }

    private static NameOfRestrictions? GetRestriction(AttributeData attribute)
    {
        return NameOfRestrictionAttributeBase.GetRestrictionsFromAttributeName(attribute.AttributeClass!.Name);
    }
}
