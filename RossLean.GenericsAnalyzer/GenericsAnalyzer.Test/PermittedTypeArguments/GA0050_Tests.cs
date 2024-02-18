using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0050_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void SpecializedExclusives_01()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterArrayTypes(FilterType.Exclusive)]
                [FilterArrayTypes(FilterType.Exclusive, 1)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void SpecializedExclusives_02()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterArrayTypes(FilterType.Exclusive)]
                [FilterArrayTypes(FilterType.Prohibited, 1)]
                [FilterArrayTypes(FilterType.Exclusive, 2)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void SpecializedExclusives_03()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterGenericTypes(FilterType.Exclusive)]
                [FilterGenericTypes(FilterType.Exclusive, 1)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void SpecializedExclusives_04()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterGenericTypes(FilterType.Exclusive)]
                [FilterGenericTypes(FilterType.Prohibited, 1)]
                [FilterGenericTypes(FilterType.Exclusive, 2)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
