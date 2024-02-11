using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.CSharp.Syntax;
using System.Diagnostics;

namespace RossLean.Smarttributes;

public abstract class BaseAttributeAnalyzer : SmarttributesDiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);

        RegisterNodeActions(context);
    }

    protected virtual void RegisterNodeActions(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeAttributedSymbol, SyntaxKind.Attribute);
    }

    protected virtual void AnalyzeAttributedSymbol(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AttributeSyntax attributeNode)
        {
            Debug.Fail("Visited not an attribute node");
            return;
        }

        var attributedNode = attributeNode.GetAttributeDeclarationParent();
        var declaredSymbol = context.SemanticModel.GetDeclaredOrAnonymousSymbol(attributedNode);
        if (declaredSymbol is null)
            return;

        var attributeContext = new AttributeSyntaxNodeAnalysisContext(context, attributeNode, attributedNode, declaredSymbol);
        AnalyzeAttributedSymbol(attributeContext);
    }

    protected abstract void AnalyzeAttributedSymbol(AttributeSyntaxNodeAnalysisContext context);

    protected abstract record CustomAttributeData(AttributeData Attribute);

    public readonly record struct AttributeSyntaxNodeAnalysisContext(
        SyntaxNodeAnalysisContext Context,
        AttributeSyntax AttributeNode,
        SyntaxNode AttributedNode,
        ISymbol? DeclaredAttributedSymbol);
}