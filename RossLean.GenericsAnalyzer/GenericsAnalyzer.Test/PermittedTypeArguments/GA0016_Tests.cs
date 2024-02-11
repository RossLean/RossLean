using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0016_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void RedundantUsageInClass()
    {
        var testCode =
@"
class C<T>
{
}
class D
<
    T,
    [↓InheritBaseTypeUsageConstraints]
    U
> : C<T>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
