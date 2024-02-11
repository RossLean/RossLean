using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using RoseLynn;
using RoseLynn.CSharp.Syntax;
using RossLean.GenericsAnalyzer.Core;
using RossLean.GenericsAnalyzer.Core.Utilities;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static RossLean.GenericsAnalyzer.GADiagnosticDescriptorStorage;

namespace RossLean.GenericsAnalyzer;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstraintClauseTypeConstraintPlacer))]
public class ConstraintClauseTypeConstraintPlacer : GACodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => new[]
    {
        Instance[0006]
    };

    protected override async Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        var document = context.Document;

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        // Find the useful nodes before altering the document to ensure the node is valid
        var memberDeclarationSyntax = syntaxNode.GetNearestParentOfType<MemberDeclarationSyntax>();
        var attributeListSyntax = syntaxNode.GetNearestParentOfType<AttributeListSyntax>();

        var typeParameter = attributeListSyntax.Parent as TypeParameterSyntax;

        var constraintClauses = memberDeclarationSyntax.GetConstraintClauses();
        TypeParameterConstraintClauseSyntax oldConstraintClause = null;
        TypeParameterConstraintClauseSyntax newConstraintClause;
        foreach (var c in constraintClauses)
        {
            if (c.Name.Identifier.ValueText == typeParameter.Identifier.ValueText)
            {
                oldConstraintClause = c;
                break;
            }
        }

        // Get the single type that is constrained
        var attributeExpression = (syntaxNode as AttributeArgumentSyntax).Expression as TypeOfExpressionSyntax;
        var attributeExpressionType = attributeExpression.Type;
        var typeConstraint = SyntaxFactory.TypeConstraint(attributeExpressionType);
        bool createdNewConstraintClause;

        // Create a new constraint clause if it does not exist already
        if (createdNewConstraintClause = oldConstraintClause is null)
            oldConstraintClause = SyntaxFactory.TypeParameterConstraintClause(typeParameter.Identifier.ValueText);

        newConstraintClause = oldConstraintClause.AddUpdateTypeConstraint(semanticModel, typeConstraint, semanticModel.GetTypeInfo(attributeExpressionType).Type);

        var originalArgumentNodeSpan = syntaxNode.Span;
        var originalMemberDeclarationSpan = memberDeclarationSyntax.Span;

        // Add the new constaint clause to the document
        if (createdNewConstraintClause)
        {
            // Completely decorative; add the new constraint clause for the type parameter right before the one that succeeds it
            // Initially get the semantic model info for all type parameters and then evaluate their ordinals
            int targetTypeParameterOrdinal = semanticModel.GetDeclaredSymbol(typeParameter, cancellationToken).Ordinal;
            int insertionIndex = constraintClauses.Count;
            for (int i = 0; i < constraintClauses.Count; i++)
            {
                var clause = constraintClauses[i];
                var clauseTypeParameter = semanticModel.GetTypeInfo(clause.Name, cancellationToken).Type as ITypeParameterSymbol;

                if (clauseTypeParameter.Ordinal > targetTypeParameterOrdinal)
                {
                    insertionIndex = i;
                    break;
                }
            }

            // Unfortunately, the resulting generated node is not properly formatted, since declaring generic type parameters
            // with attributes in multiple lines is not expected
            var newConstraintClauseList = constraintClauses.Insert(insertionIndex, newConstraintClause);
            var replacedConstriantClauseNode = memberDeclarationSyntax.WithConstraintClauses(newConstraintClauseList);
            document = await document.ReplaceNodeAsync(memberDeclarationSyntax, replacedConstriantClauseNode, cancellationToken);
        }
        else
            document = await document.ReplaceNodeAsync(oldConstraintClause, newConstraintClause, cancellationToken);

        // Remove the attribute that referred to the type constraint that was placed to the constraint clause
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        var argumentNode = root.FindNode(originalArgumentNodeSpan);

        var oldDocument = document;
        document = await document.RemoveAttributeArgumentCleanAsync(argumentNode as AttributeArgumentSyntax, SyntaxRemoveOptions.KeepNoTrivia, cancellationToken);
        semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        root = await document.GetSyntaxRootAsync(cancellationToken);

        // Also remove the remaining OnlyPermitSpecifiedTypes if there are no other type constraint attributes
        var difference = await document.LengthDifferenceFrom(oldDocument, cancellationToken);

        var newMemberDeclarationNode = root.FindNode(new TextSpan(originalMemberDeclarationSpan.Start, originalMemberDeclarationSpan.Length + difference)) as MemberDeclarationSyntax;
        var typeParameters = newMemberDeclarationNode.GetTypeParameterList().Parameters;
        var newDocumentTypeParameter = typeParameters.First(t => t.Identifier.ValueText == typeParameter.Identifier.ValueText);

        var remainingConstraintAttributes = newDocumentTypeParameter.AttributeLists.SelectMany(list => list.Attributes)
            .Where(a => a.IsGenericConstraintAttribute(semanticModel)).ToArray();

        var remainingPermissionConstraintAttributes = remainingConstraintAttributes.Where(a => a.GetAttributeIdentifierString().StartsWith("Permitted"));

        // There will always be at least OnlyPermitSpecifiedTypes, therefore it will remain if there's other attributes that contribute to the constraint system
        if (remainingPermissionConstraintAttributes.Any())
            return document;

        // If this throws, a unit test for the rule should have failed beforehand
        var remainingRemovedAttribute = remainingConstraintAttributes.First(a => nameof(OnlyPermitSpecifiedTypesAttribute).StartsWith(a.GetAttributeIdentifierString()));
        document = await document.RemoveAttributeCleanAsync(remainingRemovedAttribute, SyntaxRemoveOptions.KeepNoTrivia, cancellationToken);

        return document;
    }
}
