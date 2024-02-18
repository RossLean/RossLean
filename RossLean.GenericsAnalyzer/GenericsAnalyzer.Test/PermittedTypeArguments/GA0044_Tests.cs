using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0044_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void RedundantUncombinableProhibitedFilters()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterGenericTypes(FilterType.Exclusive)]
                [FilterGenericTypes(FilterType.Prohibited, 1)]
                [FilterSealedClasses(FilterType.Prohibited)]
                [FilterDelegates(FilterType.Prohibited)]
                [↓FilterEnums(FilterType.Prohibited)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
