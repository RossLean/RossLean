using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


public class GA0014_CodeFixTests : RedundantAttributeRemoverCodeFixTests
{
    [Test]
    public void RedundantUsageWithCodeFix()
    {
        var testCode =
            """
            delegate void Delegate
            <
                [{|*:InheritBaseTypeUsageConstraints|}]
                T
            >(T something);
            """;

        var fixedCode =
            """
            delegate void Delegate
            <
                T
            >(T something);
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
    // This is only tested in GA0014 since it's the same code fix
    [Test]
    public void AttributeListRedundantUsageWithCodeFix()
    {
        var testCode =
            """
            delegate void Delegate
            <
                [Example, {|*:InheritBaseTypeUsageConstraints|}, Example]
                T
            >(T something);
            """;

        var fixedCode =
            """
            delegate void Delegate
            <
                [Example, Example]
                T
            >(T something);
            """;

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
