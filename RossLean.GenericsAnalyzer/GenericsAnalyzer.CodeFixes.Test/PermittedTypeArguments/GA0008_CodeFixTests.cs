using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


[Ignore("Resulting document's indentation is somehow altered during assertion")]
public class GA0008_CodeFixTests : RedundantBaseTypeRuleConverterCodeFixTests
{
    [Test]
    public void RedundantBaseTypeRuleWithCodeFix()
    {
        var testCode =
@"
class C
<
    [PermittedBaseTypes({|*:typeof(long)|})]
    [PermittedBaseTypes(typeof(IEnumerable<>))]
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
    [PermittedTypes(typeof(long))]
    [PermittedBaseTypes(typeof(IEnumerable<>))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
    [Test]
    public void RedundantBaseTypeRuleWithinMultipleTypeRulesWithCodeFix()
    {
        var testCode =
@"
class C
<
    [ProhibitedBaseTypes(typeof(Attribute), {|*:typeof(long)|}, typeof(IEnumerable<>))]
    T
>
{
}
";

        var fixedCode =
@"
class C
<
    [ProhibitedTypes(typeof(long))]
    [ProhibitedBaseTypes(typeof(Attribute), typeof(IEnumerable<>))]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
