using Microsoft.CodeAnalysis;
using RoseLynn;
using static RossLean.NameOn.NAMEDiagnosticDescriptorStorage;

namespace RossLean.NameOn;

internal static class Diagnostics
{
    public static Diagnostic CreateNAME0001(SyntaxNode node, ISymbol assignedSymbol, IdentifiableSymbolKind symbolKind)
    {
        return CreateInvalidNameOfAssignment(0001, node, assignedSymbol, symbolKind);
    }
    public static Diagnostic CreateNAME0002(SyntaxNode node, ISymbol assignedSymbol, IdentifiableSymbolKind symbolKind)
    {
        return CreateInvalidNameOfAssignment(0002, node, assignedSymbol, symbolKind);
    }
    public static Diagnostic CreateNAME0003(SyntaxNode node, ISymbol assignedSymbol, IdentifiableSymbolKind symbolKind)
    {
        return CreateInvalidNameOfAssignment(0003, node, assignedSymbol, symbolKind);
    }
    public static Diagnostic CreateNAME0004(SyntaxNode node, ISymbol assignedSymbol, IdentifiableSymbolKind symbolKind)
    {
        return CreateInvalidNameOfAssignment(0004, node, assignedSymbol, symbolKind);
    }

    public static Diagnostic CreateNAME0010(SyntaxNode node, ISymbol assignedSymbol)
    {
        return CreateInvalidNameOfAssignment(0010, node, assignedSymbol, default);
    }

    private static Diagnostic CreateInvalidNameOfAssignment(int id, SyntaxNode node, ISymbol assignedSymbol, IdentifiableSymbolKind symbolKind)
    {
        return Diagnostic.Create(Instance[id], node?.GetLocation(), assignedSymbol.ToDisplayString(), symbolKind);
    }

    public delegate Diagnostic InvalidNameOfAssignmentDiagnosticCreator(SyntaxNode node, ISymbol assignedSymbol, IdentifiableSymbolKind symbolKind);
}
