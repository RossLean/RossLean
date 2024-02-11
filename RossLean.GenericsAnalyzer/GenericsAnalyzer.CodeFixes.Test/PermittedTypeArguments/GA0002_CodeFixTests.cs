using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;


public class GA0002_CodeFixTests : DuplicateAttributeArgumentRemoverCodeFixTests
{
    // TODO: Find a way to test applying the code fix on a specified diagnostic
    [Test]
    public void ConflictingConstraintsCodeFix()
    {
        var testCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes({|*:typeof(int)|}, typeof(long), typeof(ulong))]
    [ProhibitedTypes({|*:typeof(int)|})]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        var fixedCode0 =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes(typeof(int), typeof(long), typeof(ulong))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        var fixedCode1 =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes(typeof(long), typeof(ulong))]
    [ProhibitedTypes(typeof(int))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode0, 0);
        //TestCodeFixWithUsings(testCode, fixedCode1, 1);
    }
    [Test]
    public void MultipleConflictingConstraintsCodeFix()
    {
        var testCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes({|*:typeof(List<int>)|}, typeof(long), typeof(ulong))]
    [ProhibitedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [ProhibitedBaseTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        var fixedCode0 =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes(typeof(List<int>), typeof(long), typeof(ulong))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        var fixedCode1 =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes(typeof(long), typeof(ulong))]
    [ProhibitedTypes(typeof(List<int>))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode0, 0);
        //TestCodeFixWithUsings(testCode, fixedCode1, 1);
    }
    [Test]
    public void MultipleClassesConflictingConstraintsCodeFix()
    {
        var testCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes({|*:typeof(List<int>)|}, typeof(long), typeof(ulong))]
    [ProhibitedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [ProhibitedBaseTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
class D
<
    [PermittedTypes(typeof(List<int>), typeof(long), typeof(ulong))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        var fixedCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes(typeof(List<int>), typeof(long), typeof(ulong))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
class D
<
    [PermittedTypes(typeof(List<int>), typeof(long), typeof(ulong))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode, 0);
    }
    [Test]
    public void ExtraAttributesConflictingConstraintsCodeFix()
    {
        var testCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes({|*:typeof(List<int>)|}, typeof(long), typeof(ulong))]
    [ProhibitedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [ProhibitedBaseTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [Example(typeof(List<int>), 1)]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        var fixedCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes(typeof(List<int>), typeof(long), typeof(ulong))]
    [Example(typeof(List<int>), 1)]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode, 0);
    }
}
