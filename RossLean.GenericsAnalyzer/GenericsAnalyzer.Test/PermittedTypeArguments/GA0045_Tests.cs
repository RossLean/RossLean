using NUnit.Framework;
using RossLean.Common.Test;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

[Ignore(TestIgnoreStrings.NotYetImplemented)]
public sealed class GA0045_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void OnlyFilterAttributes()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterAbstractClasses(FilterType.Permitted)]
                [↓FilterSealedClasses(FilterType.Prohibited)]
                [OnlyPermitSpecifiedTypes]
                [PermittedTypes(typeof(string), typeof(int), typeof(object))]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void WithOnlyPermitSpecifiedTypeGroups()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓FilterAbstractClasses(FilterType.Permitted)]
                [↓FilterSealedClasses(FilterType.Prohibited)]
                [↓OnlyPermitSpecifiedTypeGroups]
                [OnlyPermitSpecifiedTypes]
                [PermittedTypes(typeof(string), typeof(int), typeof(object))]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void WithBaseTypes()
    {
        const string testCode =
            """
            class GenericType
            <
                [FilterAbstractClasses(FilterType.Permitted)]
                [FilterSealedClasses(FilterType.Prohibited)]
                [OnlyPermitSpecifiedTypeGroups]
                [OnlyPermitSpecifiedTypes]
                [PermittedBaseTypes(typeof(string), typeof(object))]
                T
            >;

            // Added to report diagnostics somewhere
            class GenericType1
            <
                [↓FilterAbstractClasses(FilterType.Permitted)]
                [↓FilterSealedClasses(FilterType.Prohibited)]
                [↓OnlyPermitSpecifiedTypeGroups]
                [OnlyPermitSpecifiedTypes]
                [PermittedTypes(typeof(string), typeof(int), typeof(object))]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
