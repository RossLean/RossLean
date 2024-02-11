using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0021_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void SameTypeParameterInheritance()
    {
        var testCode =
@"
class C
<
    [InheritTypeConstraints(↓nameof(T), ↓""T"")]
    T
>
{ }
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
