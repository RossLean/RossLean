using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using RossLean.Smarttributes.Constraints;
using System.Collections.Immutable;

namespace RossLean.Smarttributes;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RestrictFunctionsAnalyzer : BaseAttributeAnalyzer
{
    protected override void AnalyzeAttributedSymbol(AttributeSyntaxNodeAnalysisContext attributeContext)
    {
        var (context, attributeNode, _, declaredSymbol) = attributeContext;

        if (declaredSymbol is not IMethodSymbol declaredMethod)
            return;

        var declaredSymbolAttribute = attributeNode.GetAttributeDataEx(context.SemanticModel);

        var declaredSymbolAttributeClass = declaredSymbolAttribute?.AttributeClass;
        if (declaredSymbolAttributeClass is null)
            return;

        var restrictFunctionsAttribute = GetRestrictFunctionsAttribute(declaredSymbolAttributeClass);
        if (restrictFunctionsAttribute is null)
            return;

        var targets = restrictFunctionsAttribute.FunctionTargets;
        bool matches = MethodMatchesFunctionTargets(declaredMethod, targets);
        if (matches)
            return;

        var diagnostic = Diagnostics.CreateSMTTR0005(
            declaredSymbolAttribute!, // Erroneously reported CS8604
            restrictFunctionsAttribute.FunctionTargets);

        context.ReportDiagnostic(diagnostic);
    }

    private static bool MethodMatchesFunctionTargets(IMethodSymbol symbol, FunctionTargets targets)
    {
        var FunctionType = GetFunctionType(symbol);
        return targets.HasFlag(FunctionType);
    }

    private static FunctionTargets GetFunctionType(IMethodSymbol method)
    {
        switch (method.MethodKind)
        {
            case MethodKind.AnonymousFunction:
            {
                var declaringSyntaxReference = method.DeclaringSyntaxReferences.FirstOrDefault();
                var declaringSyntax = declaringSyntaxReference?.GetSyntax();
                return declaringSyntax switch
                {
                    LambdaExpressionSyntax => FunctionTargets.Lambda,
                    _ => FunctionTargets.AnonymousMethod,
                };
            }
            case MethodKind.LocalFunction:
            {
                return FunctionTargets.LocalMethod;
            }
            default:
            {
                return FunctionTargets.Method;
            }
        }
    }

    private static RestrictFunctionsAttributeData? GetRestrictFunctionsAttribute(ISymbol symbol)
    {
        var attributes = symbol.GetAttributes();
        return RestrictFunctionsAttributeData.ParseSingleFromRange(attributes);
    }

    private sealed record RestrictFunctionsAttributeData(AttributeData Attribute, FunctionTargets FunctionTargets)
        : CustomAttributeData(Attribute)
    {
        public static RestrictFunctionsAttributeData? ParseSingleFromRange(ImmutableArray<AttributeData> attributes)
        {
            return attributes
                .Select(Parse)
                .Where(r => r is not null)
                .FirstOrDefault();
        }

        public static RestrictFunctionsAttributeData? Parse(AttributeData attribute)
        {
            if (attribute.AttributeClass is null)
                return null;

            var attributeClassFullName = attribute.AttributeClass.GetFullSymbolName()!;

            bool matchesAttribute = attributeClassFullName.Matches(
                KnownSymbolNames.Full.RestrictFunctionsAttribute,
                SymbolNameMatchingLevel.Namespace);

            if (matchesAttribute)
            {
                var value = attribute.ConstructorArguments[0].ValueOrDefault<int>();
                var functionTargets = (FunctionTargets)value;

                return new(attribute, functionTargets);
            }

            return null;
        }
    }
}
