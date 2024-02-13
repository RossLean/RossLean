using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0022_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void ConflictingConstraintRules()
    {
        var testCode =
            """
            class A
            <
                [PermittedBaseTypes(typeof(IEnumerable<string>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            { }

            class B
            <
                [ProhibitedBaseTypes(typeof(IEnumerable<string>))]
                [OnlyPermitSpecifiedTypes]
                T,
                [InheritTypeConstraints(nameof(T))]
                [InheritBaseTypeUsageConstraints]
                ↓U
            >
                : A<U>
            { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void MultipleConflictingConstraintRules()
    {
        var testCode =
            """
            class A
            <
                [ProhibitedBaseTypes(typeof(IEnumerable<string>))]
                [OnlyPermitSpecifiedTypes]
                T,
                [ProhibitedTypes(typeof(IEnumerable<string>))]
                [OnlyPermitSpecifiedTypes]
                U,
                [PermittedBaseTypes(typeof(IEnumerable<string>))]
                [OnlyPermitSpecifiedTypes]
                V,
                [InheritTypeConstraints(nameof(T), nameof(U), nameof(V))]
                ↓W,
                [InheritTypeConstraints(nameof(W))]
                X,
                [InheritTypeConstraints(nameof(X))]
                Y
            >
            { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
