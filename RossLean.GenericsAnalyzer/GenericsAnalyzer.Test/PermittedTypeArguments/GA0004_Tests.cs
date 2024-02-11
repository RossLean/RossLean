using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0004_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void InvalidTypeArguments()
    {
        var testCode =
@"
class C
<
    [PermittedTypes(↓typeof(int*), ↓typeof(void))]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
