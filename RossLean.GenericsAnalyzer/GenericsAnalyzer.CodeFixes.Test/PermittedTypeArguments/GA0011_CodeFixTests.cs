using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;

public class GA0011_CodeFixTests : RedundantAttributeArgumentRemoverCodeFixTests
{
    [Test]
    public void RedundantlyProhibitedTypeWithCodeFix()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes({|*:typeof(long)|})]
                [PermittedBaseTypes(typeof(IComparable<>))]
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
                [PermittedBaseTypes(typeof(IComparable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
    [Test]
    public void RedundantlyProhibitedTypeInAttributeListWithCodeFix()
    {
        var testCode =
            """
            class C
            <
                [Example, PermittedTypes({|*:typeof(long)|}), Example]
                [PermittedBaseTypes(typeof(IComparable<>))]
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
                [Example, Example]
                [PermittedBaseTypes(typeof(IComparable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
