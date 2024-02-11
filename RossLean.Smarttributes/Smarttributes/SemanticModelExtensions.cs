using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp.Syntax;

namespace RossLean.Smarttributes;

internal static class SemanticModelExtensions
{
    public static ISymbol? GetDeclaredOrAnonymousSymbol(this SemanticModel semanticModel, SyntaxNode node)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(node);
        if (declaredSymbol is not null)
            return declaredSymbol;

        if (node is AnonymousFunctionExpressionSyntax)
            return semanticModel.GetSymbolInfo(node).Symbol;

        return null;
    }

    // Fixing the undiscoverability of attributes on lambdas
    public static AttributeData? GetAttributeDataEx(this AttributeSyntax attributeSyntax, SemanticModel semanticModel)
    {
        return attributeSyntax.GetAttributeDataEx(semanticModel, out _);
    }
    public static AttributeData? GetAttributeDataEx(this AttributeSyntax attributeSyntax, SemanticModel semanticModel, out ISymbol? attributedSymbol)
    {
        attributedSymbol = attributeSyntax.GetAttributedSymbolEx(semanticModel);
        return attributedSymbol?.GetAttributes().FirstOrDefault(MatchesAttributeData);

        bool MatchesAttributeData(AttributeData attribute)
        {
            return attribute.ApplicationSyntaxReference!.GetSyntax() == attributeSyntax;
        }
    }
    public static ISymbol? GetAttributedSymbolEx(this AttributeSyntax attributeSyntax, SemanticModel semanticModel)
    {
        if (attributeSyntax.SyntaxTree != semanticModel.SyntaxTree)
            return null;

        var declarationParentNode = attributeSyntax.GetAttributeDeclarationParent();
        return attributeSyntax.GetAttributedSymbol(semanticModel)
            ?? semanticModel.GetDeclaredOrAnonymousSymbol(declarationParentNode);
    }
}