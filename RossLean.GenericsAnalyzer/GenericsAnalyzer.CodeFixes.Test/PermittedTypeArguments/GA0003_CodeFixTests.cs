using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


public class GA0003_CodeFixTests : RedundantAttributeArgumentRemoverCodeFixTests
{
    [Test]
    public void RedundantBoundUnboundCodeFix()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes({|*:typeof(IComparable<int>)|})]
                [PermittedTypes(typeof(IComparable<>))]
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
                [PermittedTypes(typeof(IComparable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
    [Test]
    public void RedundantBaseTypeRuleWithinMultipleTypeRulesWithCodeFix()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes({|*:typeof(IComparable<int>)|}, typeof(IComparable<>))]
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
                [PermittedTypes(typeof(IComparable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
