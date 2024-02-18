using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0041_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void ExclusiveCombination_01()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterArrayTypes(FilterType.Exclusive)]
                [↓FilterGenericTypes(FilterType.Exclusive)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void ExclusiveCombination_02()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterInterfaces(FilterType.Exclusive)]
                [↓FilterEnums(FilterType.Exclusive)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    // TODO: Test more patterns

    [Test]
    public void AllExclusive()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterInterfaces(FilterType.Exclusive)]
                [↓FilterEnums(FilterType.Exclusive)]
                [↓FilterDelegates(FilterType.Exclusive)]
                [↓FilterAbstractClasses(FilterType.Exclusive)]
                [↓FilterSealedClasses(FilterType.Exclusive)]
                [↓FilterRecordClasses(FilterType.Exclusive)]
                [↓FilterRecordStructs(FilterType.Exclusive)]
                [↓FilterArrayTypes(FilterType.Exclusive)]
                [↓FilterGenericTypes(FilterType.Exclusive)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
