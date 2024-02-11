using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


public class GA0004_CodeFixTests : RedundantAttributeArgumentRemoverCodeFixTests
{
    [Test]
    public void InvalidTypeArgumentCodeFix()
    {
        var testCode =
@"
class C
<
    [PermittedTypes({|*:typeof(int*)|})]
    [ProhibitedBaseTypes(typeof(object))]
    T
>
{
}
";

        var fixedCode =
@"
class C
<
    [ProhibitedBaseTypes(typeof(object))]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
    [Test]
    public void InvalidTypeArgumentMultipleTypeRulesCodeFix()
    {
        var testCode =
@"
class C
<
    [PermittedTypes({|*:typeof(int*)|}, typeof(List<>))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        var fixedCode =
@"
class C
<
    [PermittedTypes(typeof(List<>))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
