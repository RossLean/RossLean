using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0049_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void SpecializedExclusives_01()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterArrayTypes(FilterType.Exclusive, 1)]
                [↓FilterArrayTypes(FilterType.Exclusive, 2)]
                [FilterGenericTypes(FilterType.Exclusive, 2)]
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
                [↓FilterArrayTypes(FilterType.Exclusive, 1)]
                [FilterArrayTypes(FilterType.Prohibited, 2)]
                [FilterArrayTypes(FilterType.Prohibited, 3)]
                [↓FilterArrayTypes(FilterType.Exclusive, 4)]
                [↓FilterArrayTypes(FilterType.Exclusive, 5)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
