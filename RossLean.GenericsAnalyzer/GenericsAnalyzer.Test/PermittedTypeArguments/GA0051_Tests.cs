using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0051_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void FilterTypeNone()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterAbstractClasses(↓FilterType.None)]
                [FilterGenericTypes(FilterType.Exclusive)]
                [FilterGenericTypes(↓FilterType.None, 2)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
