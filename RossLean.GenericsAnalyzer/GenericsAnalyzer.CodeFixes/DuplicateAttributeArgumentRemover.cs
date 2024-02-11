using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CodeFixes;
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
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DuplicateAttributeArgumentRemover))]
public class DuplicateAttributeArgumentRemover : GACodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => new[]
    {
        Instance[0002],
        Instance[0009],
    };

    public override FixAllProvider GetFixAllProvider() => null;

    protected override async Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        var document = context.Document;
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        var argument = syntaxNode as AttributeArgumentSyntax;
        var targetType = GetTypeSymbol(argument);

        argument.GetAttributeRelatedParents(out var argumentList, out var attribute, out var attributeList);

        var typeParameter = attributeList.Parent as TypeParameterSyntax;
        var attributes = typeParameter.AttributeLists.SelectMany(l => l.Attributes)
            .Where(a => a.ArgumentList?.Arguments.Count > 0)
            .Where(a => a.IsGenericConstraintAttribute<ConstrainedTypesAttributeBase>(semanticModel));

        var arguments = attributes.SelectMany(a => a.ArgumentList.Arguments);
        arguments = arguments.Where(a => a != argument);
        var removed = arguments.Where(ArgumentRemovalPredicate);

        return await context.RemoveAttributeArgumentsCleanAsync(removed, SyntaxRemoveOptions.KeepNoTrivia, cancellationToken);

        ITypeSymbol GetTypeSymbol(AttributeArgumentSyntax arg)
        {
            return semanticModel.GetTypeInfo((arg.Expression as TypeOfExpressionSyntax)?.Type).Type;
        }
        bool ArgumentRemovalPredicate(AttributeArgumentSyntax arg)
        {
            var typeSymbol = GetTypeSymbol(arg);
            return typeSymbol.Equals(targetType, SymbolEqualityComparer.Default);
        }
    }
}
