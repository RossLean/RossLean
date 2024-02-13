using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0015_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void RedundantUsageInClass()
    {
        var testCode =
            """
            class Base { }
            class A
            <
                [↓InheritBaseTypeUsageConstraints]
                T
            > : Base
            {
            }
            class C
            <
                [↓InheritBaseTypeUsageConstraints]
                T
            >
            {
            }
            class D
            <
                [InheritBaseTypeUsageConstraints]
                T,
                U
            > : C<T>
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
