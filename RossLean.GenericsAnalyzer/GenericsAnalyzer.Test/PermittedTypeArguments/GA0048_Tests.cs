using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0048_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void WithoutPermittedTypeGroupFilters()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓OnlyPermitSpecifiedTypeGroups]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    [Ignore("Revisit this case once behavior customization is introduced")]
    public void WithPermittedExactTypes()
    {
        const string testCode =
            """
            class GenericType
            <
                [↓OnlyPermitSpecifiedTypeGroups]
                [OnlyPermitSpecifiedTypes]
                [PermittedBaseTypes(typeof(string), typeof(object))]

                // TODO: Revisit this attribute declaration
                [FilterBehavior(FilteringBehavior.TypeGroupsAfterSpecificTypes) ... ]
                T
            >;
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
