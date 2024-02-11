using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0002_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void ConflictingConstraints()
    {
        var testCode =
@"
class C
<
    [PermittedTypes(↓typeof(int), typeof(long))]
    [ProhibitedTypes(↓typeof(int))]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void MultipleConflictingConstraints()
    {
        var testCode =
@"
class C
<
    [PermittedTypes(↓typeof(List<int>), typeof(long))]
    [ProhibitedTypes(↓typeof(List<int>))]
    [ProhibitedBaseTypes(↓typeof(List<int>))]
    [PermittedBaseTypes(↓typeof(List<int>))]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void MultipleConflictingDuplicateConstraints()
    {
        var testCode =
@"
class C
<
    [PermittedTypes(↓typeof(List<int>), typeof(long))]
    [ProhibitedTypes(↓typeof(List<int>), ↓typeof(List<int>))]
    [ProhibitedBaseTypes(↓typeof(List<int>))]
    [PermittedBaseTypes(↓typeof(List<int>), ↓typeof(List<int>))]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void MultipleDifferentConflictingConstraints()
    {
        var testCode =
@"
class D
<
    [PermittedTypes(↓typeof(int), ↓typeof(long[]))]
    [ProhibitedTypes(↓typeof(int), typeof(short), ↓typeof(long[]))]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
