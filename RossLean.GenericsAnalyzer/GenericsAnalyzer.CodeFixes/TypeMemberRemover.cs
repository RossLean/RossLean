using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static RossLean.GenericsAnalyzer.GADiagnosticDescriptorStorage;

namespace RossLean.GenericsAnalyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AllTypeMemberRemover))]
public sealed class AllTypeMemberRemover : TypeMemberRemover
{
    protected override RemovableTypeMembers GetRemovableTypeMembers() => RemovableTypeMembers.All;

    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => [];
}
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StaticTypeMemberRemover))]
public sealed class StaticTypeMemberRemover : TypeMemberRemover
{
    protected override RemovableTypeMembers GetRemovableTypeMembers() => RemovableTypeMembers.Static;

    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => [];
}
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InstanceTypeMemberRemover))]
public sealed class InstanceTypeMemberRemover : TypeMemberRemover
{
    protected override RemovableTypeMembers GetRemovableTypeMembers() => RemovableTypeMembers.Instance;

    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors =>
    [
        Instance[0024],
    ];
}

[Shared]
public abstract class TypeMemberRemover : GACodeFixProvider
{
    protected abstract RemovableTypeMembers GetRemovableTypeMembers();

    protected override async Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        var document = context.Document;

        var removable = GetRemovableTypeMembers();
        if (removable is RemovableTypeMembers.None)
            return document;

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        if (syntaxNode is not TypeDeclarationSyntax typeDeclaration)
            return document;

        var declaredMembers = typeDeclaration.Members;
        var removedMembers = Enumerable.Empty<MemberDeclarationSyntax>();
        switch (removable)
        {
            case RemovableTypeMembers.Instance:
                removedMembers = declaredMembers.Where(FilterRemovableInstanceMember);
                break;

            case RemovableTypeMembers.Static:
                removedMembers = declaredMembers.Where(FilterRemovableStaticMember);
                break;

            case RemovableTypeMembers.All:
                removedMembers = declaredMembers;
                break;
        }

        document = await document.RemoveSyntaxNodesAsync(removedMembers, SyntaxRemoveOptions.KeepNoTrivia, cancellationToken);

        return document;

        bool FilterRemovableStaticMember(MemberDeclarationSyntax memberDeclaration)
        {
            return FilterRemovableMember(memberDeclaration, true);
        }
        bool FilterRemovableInstanceMember(MemberDeclarationSyntax memberDeclaration)
        {
            return FilterRemovableMember(memberDeclaration, false);
        }
        bool FilterRemovableMember(MemberDeclarationSyntax memberDeclaration, bool matchStatic)
        {
            var symbol = semanticModel.GetDeclaredSymbol(memberDeclaration);
            return symbol.IsStatic == matchStatic;
        }
    }

    [Flags]
    protected enum RemovableTypeMembers
    {
        None = 0,

        Static = 1,
        Instance = 2,

        All = Static | Instance,
    }
}
