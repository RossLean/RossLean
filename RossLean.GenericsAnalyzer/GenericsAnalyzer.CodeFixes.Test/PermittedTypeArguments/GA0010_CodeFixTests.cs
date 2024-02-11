using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;

public class GA0010_CodeFixTests : RedundantAttributeArgumentRemoverCodeFixTests
{
    [Test]
    public void RedundantlyProhibitedTypeWithCodeFix()
    {
        var testCode =
@"
class C
<
    [ProhibitedTypes({|*:typeof(long)|})]
    [ProhibitedBaseTypes(typeof(IComparable<>))]
    T
>
{
}
";

        var fixedCode =
@"
class C
<
    [ProhibitedBaseTypes(typeof(IComparable<>))]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
    [Test]
    public void RedundantlyProhibitedTypeInAttributeListWithCodeFix()
    {
        var testCode =
@"
class C
<
    [Example, ProhibitedTypes({|*:typeof(long)|}), Example]
    [ProhibitedBaseTypes(typeof(IComparable<>))]
    T
>
{
}
";

        var fixedCode =
@"
class C
<
    [Example, Example]
    [ProhibitedBaseTypes(typeof(IComparable<>))]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
    // Also applies to GA0011
    [Test]
    public void RedundantlyProhibitedAndValidTypeInAttributeListWithCodeFix()
    {
        var testCode =
@"
class C
<
    [Example, ProhibitedTypes({|*:typeof(long)|}, typeof(List<>)), Example]
    [ProhibitedBaseTypes(typeof(IComparable<>))]
    T
>
{
}
";

        var fixedCode =
@"
class C
<
    [Example, ProhibitedTypes(typeof(List<>)), Example]
    [ProhibitedBaseTypes(typeof(IComparable<>))]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
