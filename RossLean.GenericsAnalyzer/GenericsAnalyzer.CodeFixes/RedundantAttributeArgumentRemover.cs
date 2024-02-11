using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CodeFixes;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using static RossLean.GenericsAnalyzer.GADiagnosticDescriptorStorage;

namespace RossLean.GenericsAnalyzer;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantAttributeArgumentRemover))]
public class RedundantAttributeArgumentRemover : GACodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => new[]
    {
        Instance[0003],
        Instance[0004],
        Instance[0005],
        Instance[0010],
        Instance[0011],
    };

    protected override async Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        return await context.RemoveAttributeArgumentCleanAsync(syntaxNode as AttributeArgumentSyntax, SyntaxRemoveOptions.KeepNoTrivia, cancellationToken);
    }
}
