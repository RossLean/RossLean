using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;

public class GA0008_CodeFixTests : RedundantBaseTypeRuleConverterCodeFixTests
{
    internal const string AttributeListMissingTrivia =
        """
        The leading trivia of the one remaining attribute list are missing
        after performing the code fix and preparing its assertion.
        """;

    [Test]
    [Ignore(AttributeListMissingTrivia)]
    public void RedundantBaseTypeRuleWithCodeFix()
    {
        var testCode =
            """
            class C
            <
                [PermittedBaseTypes({|*:typeof(long)|})]
                [PermittedBaseTypes(typeof(IEnumerable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        var fixedCode =
            """
            class C
            <
                [PermittedTypes(typeof(long))]
                [PermittedBaseTypes(typeof(IEnumerable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }

    [Test]
    [Ignore(AttributeListMissingTrivia)]
    public void RedundantBaseTypeRuleWithinMultipleTypeRulesWithCodeFix()
    {
        var testCode =
            """
            class C
            <
                [ProhibitedBaseTypes(typeof(Attribute), {|*:typeof(long)|}, typeof(IEnumerable<>))]
                T
            >
            {
            }
            """;

        var fixedCode =
            """
            class C
            <
                [ProhibitedTypes(typeof(long))]
                [ProhibitedBaseTypes(typeof(Attribute), typeof(IEnumerable<>))]
                T
            >
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
