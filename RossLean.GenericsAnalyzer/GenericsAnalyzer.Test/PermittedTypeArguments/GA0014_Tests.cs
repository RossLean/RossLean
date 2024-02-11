using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0014_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void RedundantUsageInFunctionAndDelegate()
    {
        var testCode =
@"
class A<T> { }
class C
<
    [InheritBaseTypeUsageConstraints]
    T0
> : A<T0>
{
    void Function
    <
        [↓InheritBaseTypeUsageConstraints]
        T1
    >()
    {
    }
}

delegate void Function
<
    [↓InheritBaseTypeUsageConstraints]
    T
>();
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
