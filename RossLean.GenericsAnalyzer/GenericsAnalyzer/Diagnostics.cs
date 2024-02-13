using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace RossLean.GenericsAnalyzer;

#nullable enable

internal static class Diagnostics
{
    private static GADiagnosticDescriptorStorage Storage => GADiagnosticDescriptorStorage.Instance;

    public static Diagnostic CreateGA0001(
        SyntaxNode node,
        ISymbol originalDefinition,
        ITypeSymbol argumentType,
        string? reason)
    {
        return Diagnostic.Create(
            Storage[0001]!,
            node?.GetLocation(),
            originalDefinition.ToDisplayString(),
            argumentType.ToDisplayString(),
            CreateReasonSuffix(reason));
    }
    public static Diagnostic CreateGA0002(
        AttributeArgumentSyntax attributeArgumentSyntaxNode,
        ITypeParameterSymbol typeParameter,
        ITypeSymbol argumentType)
    {
        return Diagnostic.Create(
            Storage[0002]!,
            attributeArgumentSyntaxNode?.GetLocation(),
            typeParameter.ToDisplayString(),
            argumentType.ToDisplayString());
    }
    public static Diagnostic CreateGA0003(
        AttributeArgumentSyntax attributeArgumentSyntaxNode,
        ITypeParameterSymbol typeParameter,
        INamedTypeSymbol genericTypeArgument)
    {
        return Diagnostic.Create(
            Storage[0003]!,
            attributeArgumentSyntaxNode?.GetLocation(),
            typeParameter.ToDisplayString(),
            genericTypeArgument.ToDisplayString());
    }
    public static Diagnostic CreateGA0004(
        AttributeArgumentSyntax attributeArgumentSyntaxNode,
        ITypeSymbol argumentType)
    {
        return Diagnostic.Create(
            Storage[0004]!,
            attributeArgumentSyntaxNode?.GetLocation(),
            argumentType.ToDisplayString());
    }
    public static Diagnostic CreateGA0005(
        AttributeArgumentSyntax attributeArgumentSyntaxNode,
        ITypeSymbol argumentType,
        ITypeParameterSymbol typeParameter)
    {
        return Diagnostic.Create(
            Storage[0005]!,
            attributeArgumentSyntaxNode?.GetLocation(),
            argumentType.ToDisplayString(),
            typeParameter.ToDisplayString());
    }
    public static Diagnostic CreateGA0006(
        AttributeArgumentSyntax attributeArgumentSyntaxNode)
    {
        return Diagnostic.Create(
            Storage[0006]!,
            attributeArgumentSyntaxNode?.GetLocation());
    }
    public static Diagnostic CreateGA0008(
        AttributeArgumentSyntax attributeArgumentSyntaxNode,
        ITypeSymbol argumentType)
    {
        return Diagnostic.Create(
            Storage[0008]!,
            attributeArgumentSyntaxNode?.GetLocation(),
            argumentType.ToDisplayString());
    }
    public static Diagnostic CreateGA0009(
        AttributeArgumentSyntax attributeArgumentSyntaxNode,
        ITypeSymbol argumentType)
    {
        return Diagnostic.Create(
            Storage[0009]!,
            attributeArgumentSyntaxNode?.GetLocation(),
            argumentType.ToDisplayString());
    }
    public static Diagnostic CreateGA0010(
        AttributeArgumentSyntax attributeArgumentSyntaxNode,
        ITypeSymbol argumentType)
    {
        return Diagnostic.Create(
            Storage[0010]!,
            attributeArgumentSyntaxNode?.GetLocation(),
            argumentType.ToDisplayString());
    }
    public static Diagnostic CreateGA0011(
        AttributeArgumentSyntax attributeArgumentSyntaxNode,
        ITypeSymbol argumentType)
    {
        return Diagnostic.Create(
            Storage[0011]!,
            attributeArgumentSyntaxNode?.GetLocation(),
            argumentType.ToDisplayString());
    }
    public static Diagnostic CreateGA0012(AttributeSyntax attributeSyntaxNode)
    {
        return Diagnostic.Create(Storage[0012]!, attributeSyntaxNode?.GetLocation());
    }
    public static Diagnostic CreateGA0013(
        AttributeSyntax attributeSyntaxNode,
        ITypeParameterSymbol typeParameter)
    {
        return Diagnostic.Create(
            Storage[0013]!,
            attributeSyntaxNode?.GetLocation(),
            typeParameter.ToDisplayString());
    }
    public static Diagnostic CreateGA0014(
        AttributeSyntax attributeSyntaxNode,
        ISymbol symbol)
    {
        return Diagnostic.Create(
            Storage[0014]!,
            attributeSyntaxNode?.GetLocation(),
            symbol.ToDisplayString());
    }
    public static Diagnostic CreateGA0015(
        AttributeSyntax attributeSyntaxNode,
        ISymbol symbol)
    {
        return Diagnostic.Create(
            Storage[0015]!,
            attributeSyntaxNode?.GetLocation(),
            symbol.ToDisplayString());
    }
    public static Diagnostic CreateGA0016(
        AttributeSyntax attributeSyntaxNode,
        ISymbol symbol)
    {
        return Diagnostic.Create(
            Storage[0016]!,
            attributeSyntaxNode?.GetLocation(),
            symbol.ToDisplayString());
    }
    public static Diagnostic CreateGA0017(
        SyntaxNode node,
        ISymbol originalDefinition,
        ITypeSymbol argumentType)
    {
        return Diagnostic.Create(
            Storage[0017]!,
            node?.GetLocation(),
            originalDefinition.ToDisplayString(),
            argumentType.ToDisplayString());
    }
    public static Diagnostic CreateGA0019(
        AttributeArgumentSyntax attributeArgumentNode,
        string typeParameterName)
    {
        return Diagnostic.Create(
            Storage[0019]!,
            attributeArgumentNode?.GetLocation(),
            typeParameterName);
    }
    public static Diagnostic CreateGA0020(
        AttributeArgumentSyntax attributeArgumentNode,
        IEnumerable<ITypeParameterSymbol> recursionPath)
    {
        string recursionPathString = string.Join(", ",
            recursionPath.Select(t => t.ToDisplayString()));

        return Diagnostic.Create(
            Storage[0020]!,
            attributeArgumentNode?.GetLocation(),
            recursionPathString);
    }
    public static Diagnostic CreateGA0021(AttributeArgumentSyntax attributeArgumentNode)
    {
        return Diagnostic.Create(Storage[0021]!, attributeArgumentNode?.GetLocation());
    }
    public static Diagnostic CreateGA0022(TypeParameterSyntax typeParameterDeclarationNode)
    {
        return Diagnostic.Create(Storage[0022]!, typeParameterDeclarationNode?.Identifier.GetLocation());
    }
    public static Diagnostic CreateGA0023(
        InterfaceDeclarationSyntax interfaceDeclarationNode)
    {
        return CreateProfileInterfaceDiagnostic(interfaceDeclarationNode, Storage[0023]!);
    }
    public static Diagnostic CreateGA0024(
        InterfaceDeclarationSyntax interfaceDeclarationNode)
    {
        return CreateProfileInterfaceDiagnostic(interfaceDeclarationNode, Storage[0024]!);
    }
    public static Diagnostic CreateGA0025(
        InterfaceDeclarationSyntax interfaceDeclarationNode)
    {
        return CreateProfileInterfaceDiagnostic(interfaceDeclarationNode, Storage[0025]!);
    }
    public static Diagnostic CreateGA0025(BaseTypeSyntax baseTypeNode)
    {
        return Diagnostic.Create(Storage[0025]!, baseTypeNode?.GetLocation());
    }
    public static Diagnostic CreateGA0026(
        AttributeArgumentSyntax nonProfileTypeArgumentNode)
    {
        return Diagnostic.Create(Storage[0026]!, nonProfileTypeArgumentNode?.GetLocation());
    }
    public static Diagnostic CreateGA0027(
        AttributeArgumentSyntax nonProfileGroupTypeArgumentNode)
    {
        return Diagnostic.Create(
            Storage[0027]!,
            nonProfileGroupTypeArgumentNode?.GetLocation());
    }
    public static Diagnostic CreateGA0028(
        AttributeArgumentSyntax typeProfileInheritanceAttributeArgumentNode)
    {
        return Diagnostic.Create(
            Storage[0028]!,
            typeProfileInheritanceAttributeArgumentNode?.GetLocation());
    }
    public static Diagnostic CreateGA0029(
        InterfaceDeclarationSyntax interfaceDeclarationNode)
    {
        return CreateProfileInterfaceDiagnostic(interfaceDeclarationNode, Storage[0029]!);
    }
    public static Diagnostic CreateGA0030(AttributeSyntax attributeNode)
    {
        return Diagnostic.Create(Storage[0030]!, attributeNode?.GetLocation());
    }

    private static string CreateReasonSuffix(string? reason)
    {
        if (string.IsNullOrEmpty(reason))
            return string.Empty;

        // Significant newline whitespace
        return $"""

            Reason: {reason}
            """;
    }

    private static Diagnostic CreateProfileInterfaceDiagnostic(
        InterfaceDeclarationSyntax interfaceDeclarationNode,
        DiagnosticDescriptor rule)
    {
        return Diagnostic.Create(rule, interfaceDeclarationNode?.Identifier.GetLocation());
    }
}
