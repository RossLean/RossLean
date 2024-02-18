using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0040_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void ClashingPermissionsWithExclusive()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterArrayTypes(FilterType.Exclusive)]
                [↓FilterGenericTypes(FilterType.Permitted)]
                [↓FilterAbstractClasses(FilterType.Permitted)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void ClashingPermissionsWithAvailableCombination()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterAbstractClasses(FilterType.Exclusive)]
                [↓FilterSealedClasses(FilterType.Permitted)]
                [↓FilterRecordClasses(FilterType.Permitted)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void ClashingPermissionsOnExistingCombination()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterAbstractClasses(FilterType.Exclusive)]
                [FilterRecordClasses(FilterType.Exclusive)]
                [↓FilterSealedClasses(FilterType.Permitted)]
                [↓FilterRecordStructs(FilterType.Permitted)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
