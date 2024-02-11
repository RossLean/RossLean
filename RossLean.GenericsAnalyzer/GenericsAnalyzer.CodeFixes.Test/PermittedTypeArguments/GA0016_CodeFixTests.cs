using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


public class GA0016_CodeFixTests : RedundantAttributeRemoverCodeFixTests
{
    [Test]
    public void RedundantUsageWithCodeFix()
    {
        var testCode =
@"
class A<T> { }
class C
<
    [{|*:InheritBaseTypeUsageConstraints|}]
    T
>
    : A<int>
{
}
";

        var fixedCode =
@"
class A<T> { }
class C
<
    T
>
    : A<int>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
