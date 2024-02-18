using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0052_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void InvalidSpecializationValues()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterGenericTypes(FilterType.Prohibited, ↓0)]
                [FilterArrayTypes(FilterType.Prohibited, ↓0)]
                [FilterGenericTypes(FilterType.Permitted)]
                [FilterArrayTypes(FilterType.Permitted)]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
