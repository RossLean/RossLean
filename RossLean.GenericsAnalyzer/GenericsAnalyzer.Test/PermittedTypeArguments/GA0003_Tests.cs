using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0003_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void BoundUnbound()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes(↓typeof(IComparable<int>), typeof(IComparable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void BoundUnboundDifferentAttributes()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes(↓typeof(IComparable<int>))]
                [PermittedTypes(typeof(IComparable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
