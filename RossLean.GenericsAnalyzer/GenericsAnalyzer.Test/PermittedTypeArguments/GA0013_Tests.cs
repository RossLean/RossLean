using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0013_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void MultipleConstraints()
    {
        var testCode =
            """
            class C
            <
                [↓PermittedTypes(typeof(int))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
