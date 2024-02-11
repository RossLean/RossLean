using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using RoseLynn;
using RoseLynn.CSharp.Syntax;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using static RossLean.GenericsAnalyzer.GADiagnosticDescriptorStorage;

namespace RossLean.GenericsAnalyzer;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantBaseTypeRuleConverter))]
public class RedundantBaseTypeRuleConverter : GACodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => new[]
    {
        Instance[0008]
    };

    public override FixAllProvider GetFixAllProvider() => null;

    protected override async Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        var document = context.Document;

        // Construct the new attribute to insert
        var attributeArgumentNode = syntaxNode as AttributeArgumentSyntax;
        var attributeNode = attributeArgumentNode.Parent.Parent as AttributeSyntax;
        var attributeListNode = attributeNode.Parent as AttributeListSyntax;
        var attributeName = attributeNode.GetAttributeIdentifierString();
        var newAttributeName = attributeName.Replace("BaseType", "Type");
        var newAttributeArgumentList = SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(new[] { attributeArgumentNode }));
        var newAttributeNode = attributeNode.WithName(SyntaxFactory.IdentifierName(newAttributeName)).WithArgumentList(newAttributeArgumentList);
        var newAttributeListNode = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[] { newAttributeNode })).WithTriviaFrom(attributeListNode);

        // Get the type parameter node info to match it in the resulting document after the argument removal
        var typeParameterNode = attributeListNode.Parent as TypeParameterSyntax;
        var originalTypeParameterSpan = typeParameterNode.FullSpan;

        // Get the index to insert the new attribute list at
        int newAttributeIndex = 0;
        var typeParameterAttributeLists = typeParameterNode.AttributeLists;
        for (int i = 0; i < typeParameterAttributeLists.Count; i++)
        {
            if (typeParameterAttributeLists[i] == attributeListNode)
            {
                newAttributeIndex = i;
                break;
            }
        }

        // Remove the original argument from the attribute
        var oldDocument = document;
        document = await document.RemoveAttributeArgumentCleanAsync(attributeArgumentNode, SyntaxRemoveOptions.KeepNoTrivia, cancellationToken);
        int difference = await document.LengthDifferenceFrom(oldDocument, cancellationToken);

        var root = await document.GetSyntaxRootAsync(cancellationToken);

        // Insert the new attribute
        var newTypeParameterSpan = new TextSpan(originalTypeParameterSpan.Start, originalTypeParameterSpan.Length + difference);
        typeParameterNode = root.FindNode(newTypeParameterSpan) as TypeParameterSyntax;
        var lists = typeParameterNode.AttributeLists.Insert(newAttributeIndex, newAttributeListNode);
        var newTypeParameterNode = typeParameterNode.WithAttributeLists(lists);

        // The resulting document is correct, the asserions are wrong
        return document = await document.ReplaceNodeAsync(typeParameterNode, newTypeParameterNode, cancellationToken);
    }
}
