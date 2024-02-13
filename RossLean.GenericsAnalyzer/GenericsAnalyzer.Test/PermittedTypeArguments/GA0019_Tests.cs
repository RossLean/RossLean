using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0019_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void InvalidTypeParameterNameClass()
    {
        var testCode =
            """
            class C
            <
                [InheritTypeConstraints(↓"T1", "U", nameof(U))]
                T,
                U
            >
            { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void InvalidTypeParameterNameFunction()
    {
        var testCode =
            """
            class C
            {
                void Function
                <
                    [InheritTypeConstraints(↓"T1", "U")] // nameof is not yet legal here
                    T,
                    U
                >()
                { }
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
