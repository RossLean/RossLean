using NUnit.Framework;
using RossLean.GenericsAnalyzer.Core;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0023_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void UnrelatedGenericInterface()
    {
        InvalidProfileRelatedInterface(null, false);
    }

    [Test]
    public void InvalidProfileInterfaces()
    {
        InvalidProfileRelatedInterface(nameof(TypeConstraintProfileAttribute), true);
    }

    [Test]
    public void InvalidProfileGroupInterfaces()
    {
        InvalidProfileRelatedInterface(nameof(TypeConstraintProfileGroupAttribute), true);
    }

    private void InvalidProfileRelatedInterface(string attributeSymbolName, bool assertDiagnostics)
    {
        var attributeSymbol = attributeSymbolName;
        if (!string.IsNullOrEmpty(attributeSymbol))
            attributeSymbol = $"[{attributeSymbol}]";

        var testCode =
            $$"""
            {{attributeSymbol}}
            interface I { }
            {{attributeSymbol}}
            interface ↓I<T> { }
            {{attributeSymbol}}
            interface ↓I<T1, T2> { }
            {{attributeSymbol}}
            interface ↓A<T1, T2, T3, T4> { }
            """;

        AssertOrValidateWithUsings(testCode, assertDiagnostics);
    }

    [Test]
    public void PartialGenericProfileInterface()
    {
        var testCode =
            """
            partial interface I<T1, T2> { }

            [TypeConstraintProfile]
            partial interface ↓I<T1, T2> { }

            partial interface I<T1, T2> { }

            partial interface I<T1, T2> { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
