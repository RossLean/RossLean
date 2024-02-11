using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


public class GA0015_CodeFixTests : RedundantAttributeRemoverCodeFixTests
{
    [Test]
    public void RedundantUsageWithCodeFix()
    {
        var testCode =
@"
class C
<
    [{|*:InheritBaseTypeUsageConstraints|}]
    T
>
{
}
";

        var fixedCode =
@"
class C
<
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
