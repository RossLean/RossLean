using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0047_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void DuplicateSpecializations()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterArrayTypes(FilterType.Prohibited)]
                [↓FilterArrayTypes(FilterType.Prohibited, 1)]
                [↓FilterArrayTypes(FilterType.Permitted, 1)]
                [↓FilterArrayTypes(FilterType.Prohibited, 2)]
                [↓FilterArrayTypes(FilterType.Prohibited, 2)]
                [FilterArrayTypes(FilterType.Prohibited, 3)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
