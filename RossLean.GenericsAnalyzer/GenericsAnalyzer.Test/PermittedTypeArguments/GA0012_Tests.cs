using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0012_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void MultipleConstraints()
    {
        var testCode =
@"
class C
<
    [ProhibitedTypes(typeof(int), typeof(long))]
    [ProhibitedBaseTypes(typeof(IEnumerable<int>), typeof(ICollection<string>))]
    [↓OnlyPermitSpecifiedTypes]
    T
>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
