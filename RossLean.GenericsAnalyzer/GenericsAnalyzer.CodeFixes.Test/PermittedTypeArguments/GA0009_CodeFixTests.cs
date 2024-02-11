using NUnit.Framework;
using RossLean.Common.Test;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test.PermittedTypeArguments;

public class GA0009_CodeFixTests : DuplicateAttributeArgumentRemoverCodeFixTests
{
    [Test]
    public void ConflictingConstraintsCodeFix_0()
    {
        var testCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes({|*:typeof(int)|}, typeof(long), typeof(ulong))]
    [PermittedTypes({|*:typeof(int)|})]
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
    [PermittedTypes(typeof(int), typeof(long), typeof(ulong))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode, 0);
    }

    [Test]
    [Ignore(TestIgnoreStrings.SpecificDiagnosticIndexNotSupported)]
    public void ConflictingConstraintsCodeFix_1()
    {
        var testCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes({|*:typeof(int)|}, typeof(long), typeof(ulong))]
    [PermittedTypes({|*:typeof(int)|})]
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
    [PermittedTypes(typeof(long), typeof(ulong))]
    [PermittedTypes(typeof(int))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode, 1);
    }

    [Test]
    public void MultipleConflictingConstraintsCodeFix_0()
    {
        var testCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes({|*:typeof(List<int>)|}, typeof(long), typeof(ulong))]
    [PermittedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [PermittedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
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
";

        TestCodeFixWithUsings(testCode, fixedCode, 0);
    }

    [Test]
    [Ignore(TestIgnoreStrings.SpecificDiagnosticIndexNotSupported)]
    public void MultipleConflictingConstraintsCodeFix_1()
    {
        var testCode =
@"
#pragma warning disable GA0010
#pragma warning disable GA0011

class C
<
    [PermittedTypes({|*:typeof(List<int>)|}, typeof(long), typeof(ulong))]
    [PermittedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [PermittedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
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
    [PermittedTypes(typeof(long), typeof(ulong))]
    [PermittedTypes(typeof(List<int>))]
    [OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        TestCodeFixWithUsings(testCode, fixedCode, 1);
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
    [PermittedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [PermittedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
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
    [PermittedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
    [PermittedTypes({|*:typeof(List<int>)|}, {|*:typeof(List<int>)|})]
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
