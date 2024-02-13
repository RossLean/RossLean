using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0028_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void MultipleDistinctGroupUsages()
    {
        var testCode =
            """
            [TypeConstraintProfileGroup]
            interface IGroup0 { }
            [TypeConstraintProfileGroup]
            interface IGroup1 { }
            [TypeConstraintProfileGroup]
            interface IGroup2 { }

            [TypeConstraintProfile(typeof(IGroup1), typeof(IGroup2))]
            interface IProfile0 { }
            [TypeConstraintProfile(typeof(IGroup0), typeof(IGroup2))]
            interface IProfile1 { }
            [TypeConstraintProfile(typeof(IGroup0), typeof(IGroup1))]
            interface IProfile2 { }

            [TypeConstraintProfile(typeof(IGroup0))]
            interface IProfile3 { }
            [TypeConstraintProfile(typeof(IGroup1))]
            interface IProfile4 { }
            [TypeConstraintProfile(typeof(IGroup2))]
            interface IProfile5 { }

            class Generic
            <
                [InheritProfileTypeConstraints(↓typeof(IProfile0), ↓typeof(IProfile1))]
                T,
                [InheritProfileTypeConstraints(↓typeof(IProfile1), ↓typeof(IProfile2))]
                U,
                [InheritProfileTypeConstraints(↓typeof(IProfile0), ↓typeof(IProfile2))]
                V,
                [InheritProfileTypeConstraints(typeof(IProfile3), typeof(IProfile4), typeof(IProfile5))]
                W,
                [InheritProfileTypeConstraints(typeof(IProfile0), typeof(IProfile3))]
                X,
                [InheritProfileTypeConstraints(↓typeof(IProfile1), ↓typeof(IProfile3))]
                Y,
                [InheritProfileTypeConstraints(↓typeof(IProfile0), typeof(IProfile3), ↓typeof(IProfile4))]
                Z
            >
            { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
