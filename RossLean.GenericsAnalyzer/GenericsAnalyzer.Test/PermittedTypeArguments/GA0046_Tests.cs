using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0046_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void RedundantSpecializationWithSameFilterType()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterArrayTypes(FilterType.Prohibited)]
                [↓FilterArrayTypes(FilterType.Prohibited, 1)]
                [FilterArrayTypes(FilterType.Permitted, 2)]
                [↓FilterArrayTypes(FilterType.Prohibited, 3)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
