using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0009_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void DuplicateConstraints()
    {
        var testCode =
@"
class C
<
    [PermittedTypes(↓typeof(int), typeof(long))]
    [PermittedTypes(↓typeof(int))]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void MultiplicateConstraints()
    {
        var testCode =
@"
class C
<
    [PermittedTypes(↓typeof(int), typeof(long))]
    [PermittedTypes(↓typeof(int), ↓typeof(int), ↓typeof(int))]
    [PermittedTypes(↓typeof(int), ↓typeof(int))]
    [PermittedTypes(↓typeof(int), ↓typeof(int), ↓typeof(int),  ↓typeof(int), ↓typeof(int))]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void MultipleDuplicateConstraints()
    {
        var testCode =
@"
class D
<
    [PermittedTypes(↓typeof(int), ↓typeof(long))]
    [PermittedTypes(↓typeof(int), typeof(short), ↓typeof(long))]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
