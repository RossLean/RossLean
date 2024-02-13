using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0006_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void Reducible()
    {
        var testCode =
            """
            class C
            <
                [PermittedBaseTypes(↓typeof(IComparable<int>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void ReducibleClass()
    {
        var testCode =
            """
            class Test
            <
                [PermittedBaseTypes(↓typeof(C))]
                [OnlyPermitSpecifiedTypes]
                T
            >
                where T : A
            {
            }

            class A { }
            class B : A { }
            class C : B { }
            class D : C { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
