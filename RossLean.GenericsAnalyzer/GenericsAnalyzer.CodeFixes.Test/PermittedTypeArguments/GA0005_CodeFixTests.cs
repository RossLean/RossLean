using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


public class GA0005_CodeFixTests : RedundantAttributeArgumentRemoverCodeFixTests
{
    [Test]
    public void ConstrainedTypeArgumentCodeFix()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes({|*:typeof(string)|})]
                [PermittedTypes(typeof(IList<int>), typeof(ISet<int>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
                where T : IEnumerable<int>
            {
            }
            """;

        var fixedCode =
            """
            class C
            <
                [PermittedTypes(typeof(IList<int>), typeof(ISet<int>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
                where T : IEnumerable<int>
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
    [Test]
    public void ConstrainedTypeArgumentMultipleTypeRulesCodeFix()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes({|*:typeof(string)|}, typeof(IList<int>), typeof(ISet<int>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
                where T : IEnumerable<int>
            {
            }
            """;

        var fixedCode =
            """
            class C
            <
                [PermittedTypes(typeof(IList<int>), typeof(ISet<int>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
                where T : IEnumerable<int>
            {
            }
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
