using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0017_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void IntermediateSubsetGenericTypeUsage()
    {
        var testCode =
            """
            class A
            <
                [ProhibitedBaseTypes(typeof(IA))]
                T
            >
            {
            }
            class B
            <
                [ProhibitedBaseTypes(typeof(IB))]
                T
            > : A<T>
            {
            }
            class C
            <
                [ProhibitedBaseTypes(typeof(IC))]
                T
            > : A<↓T>
            {
            }

            interface IA : IB { }
            interface IB { }
            interface IC : IA { }

            class A : IA { }
            class B : IB { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void IntermediateExplicitlyPermittedGenericType()
    {
        var testCode =
            """
            class A
            <
                [PermittedTypes(typeof(int), typeof(uint))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            class B
            <
                [InheritBaseTypeUsageConstraints]
                T
            > : A<T>
            {
            }
            class C<T> : A<↓T>
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

}
