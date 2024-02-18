using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0042_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void RedundantByExclusive()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterGenericTypes(FilterType.Exclusive)]
                [↓OnlyPermitSpecifiedTypeGroups]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
