using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Globalization;

namespace Smarttributes;

#pragma warning disable RS1009 // Roslyn internal implementations

public interface INullTypeSymbol : ITypeSymbol { }

public sealed partial class NullTypeSymbol
{
    public static NullTypeSymbol Instance { get; } = new();
}

// Redundant implementation
public sealed partial class NullTypeSymbol : INullTypeSymbol
{
    TypeKind ITypeSymbol.TypeKind => throw new NotImplementedException();

    INamedTypeSymbol? ITypeSymbol.BaseType => throw new NotImplementedException();

    ImmutableArray<INamedTypeSymbol> ITypeSymbol.Interfaces => throw new NotImplementedException();

    ImmutableArray<INamedTypeSymbol> ITypeSymbol.AllInterfaces => throw new NotImplementedException();

    bool ITypeSymbol.IsReferenceType => throw new NotImplementedException();

    bool ITypeSymbol.IsValueType => throw new NotImplementedException();

    bool ITypeSymbol.IsAnonymousType => throw new NotImplementedException();

    bool ITypeSymbol.IsTupleType => throw new NotImplementedException();

    bool ITypeSymbol.IsNativeIntegerType => throw new NotImplementedException();

    ITypeSymbol ITypeSymbol.OriginalDefinition => throw new NotImplementedException();

    ISymbol ISymbol.OriginalDefinition => throw new NotImplementedException();

    SpecialType ITypeSymbol.SpecialType => throw new NotImplementedException();

    bool ITypeSymbol.IsRefLikeType => throw new NotImplementedException();

    bool ITypeSymbol.IsUnmanagedType => throw new NotImplementedException();

    bool ITypeSymbol.IsReadOnly => throw new NotImplementedException();

    bool ITypeSymbol.IsRecord => throw new NotImplementedException();

    NullableAnnotation ITypeSymbol.NullableAnnotation => throw new NotImplementedException();

    bool INamespaceOrTypeSymbol.IsNamespace => throw new NotImplementedException();

    bool INamespaceOrTypeSymbol.IsType => throw new NotImplementedException();

    SymbolKind ISymbol.Kind => throw new NotImplementedException();

    string ISymbol.Language => throw new NotImplementedException();

    string ISymbol.Name => throw new NotImplementedException();

    string ISymbol.MetadataName => throw new NotImplementedException();

    int ISymbol.MetadataToken => throw new NotImplementedException();

    ISymbol ISymbol.ContainingSymbol => throw new NotImplementedException();

    IAssemblySymbol ISymbol.ContainingAssembly => throw new NotImplementedException();

    IModuleSymbol ISymbol.ContainingModule => throw new NotImplementedException();

    INamedTypeSymbol ISymbol.ContainingType => throw new NotImplementedException();

    INamespaceSymbol ISymbol.ContainingNamespace => throw new NotImplementedException();

    bool ISymbol.IsDefinition => throw new NotImplementedException();

    bool ISymbol.IsStatic => throw new NotImplementedException();

    bool ISymbol.IsVirtual => throw new NotImplementedException();

    bool ISymbol.IsOverride => throw new NotImplementedException();

    bool ISymbol.IsAbstract => throw new NotImplementedException();

    bool ISymbol.IsSealed => throw new NotImplementedException();

    bool ISymbol.IsExtern => throw new NotImplementedException();

    bool ISymbol.IsImplicitlyDeclared => throw new NotImplementedException();

    bool ISymbol.CanBeReferencedByName => throw new NotImplementedException();

    ImmutableArray<Location> ISymbol.Locations => throw new NotImplementedException();

    ImmutableArray<SyntaxReference> ISymbol.DeclaringSyntaxReferences => throw new NotImplementedException();

    Accessibility ISymbol.DeclaredAccessibility => throw new NotImplementedException();

    bool ISymbol.HasUnsupportedMetadata => throw new NotImplementedException();

    void ISymbol.Accept(SymbolVisitor visitor)
    {
        throw new NotImplementedException();
    }

    TResult? ISymbol.Accept<TResult>(SymbolVisitor<TResult> visitor)
        where TResult : default
    {
        throw new NotImplementedException();
    }

    TResult ISymbol.Accept<TArgument, TResult>(SymbolVisitor<TArgument, TResult> visitor, TArgument argument)
    {
        throw new NotImplementedException();
    }

    bool ISymbol.Equals(ISymbol? other, SymbolEqualityComparer equalityComparer)
    {
        throw new NotImplementedException();
    }

    bool IEquatable<ISymbol?>.Equals(ISymbol? other)
    {
        throw new NotImplementedException();
    }

    ISymbol? ITypeSymbol.FindImplementationForInterfaceMember(ISymbol interfaceMember)
    {
        throw new NotImplementedException();
    }

    ImmutableArray<AttributeData> ISymbol.GetAttributes()
    {
        throw new NotImplementedException();
    }

    string? ISymbol.GetDocumentationCommentId()
    {
        throw new NotImplementedException();
    }

    string? ISymbol.GetDocumentationCommentXml(CultureInfo? preferredCulture, bool expandIncludes, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    ImmutableArray<ISymbol> INamespaceOrTypeSymbol.GetMembers()
    {
        throw new NotImplementedException();
    }

    ImmutableArray<ISymbol> INamespaceOrTypeSymbol.GetMembers(string name)
    {
        throw new NotImplementedException();
    }

    ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers()
    {
        throw new NotImplementedException();
    }

    ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers(string name)
    {
        throw new NotImplementedException();
    }

    ImmutableArray<INamedTypeSymbol> INamespaceOrTypeSymbol.GetTypeMembers(string name, int arity)
    {
        throw new NotImplementedException();
    }

    ImmutableArray<SymbolDisplayPart> ITypeSymbol.ToDisplayParts(NullableFlowState topLevelNullability, SymbolDisplayFormat? format)
    {
        throw new NotImplementedException();
    }

    ImmutableArray<SymbolDisplayPart> ISymbol.ToDisplayParts(SymbolDisplayFormat? format)
    {
        throw new NotImplementedException();
    }

    string ITypeSymbol.ToDisplayString(NullableFlowState topLevelNullability, SymbolDisplayFormat? format)
    {
        throw new NotImplementedException();
    }

    string ISymbol.ToDisplayString(SymbolDisplayFormat? format)
    {
        throw new NotImplementedException();
    }

    ImmutableArray<SymbolDisplayPart> ITypeSymbol.ToMinimalDisplayParts(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position, SymbolDisplayFormat? format)
    {
        throw new NotImplementedException();
    }

    ImmutableArray<SymbolDisplayPart> ISymbol.ToMinimalDisplayParts(SemanticModel semanticModel, int position, SymbolDisplayFormat? format)
    {
        throw new NotImplementedException();
    }

    string ITypeSymbol.ToMinimalDisplayString(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position, SymbolDisplayFormat? format)
    {
        throw new NotImplementedException();
    }

    string ISymbol.ToMinimalDisplayString(SemanticModel semanticModel, int position, SymbolDisplayFormat? format)
    {
        throw new NotImplementedException();
    }

    ITypeSymbol ITypeSymbol.WithNullableAnnotation(NullableAnnotation nullableAnnotation)
    {
        throw new NotImplementedException();
    }
}
