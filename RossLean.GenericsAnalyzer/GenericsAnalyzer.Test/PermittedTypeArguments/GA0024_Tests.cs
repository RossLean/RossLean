using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0024_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void ProfileGroupInterfaceWithInstanceMembers()
    {
        var testCode =
            """
            [TypeConstraintProfileGroup]
            interface ↓IGroup0
            {
                int Property { get; set; }
            }

            [TypeConstraintProfileGroup]
            interface ↓IGroup1
            {
                void Function();
            }

            [TypeConstraintProfileGroup]
            interface ↓IGroup2
            {
                interface INested { }
            }

            [TypeConstraintProfile(typeof(IGroup0), typeof(IGroup1))]
            interface IB
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void PartialProfileGroupInterfaceWithMembers()
    {
        var testCode =
            """
            partial interface ↓IGroup0
            {
                int Property0 { get; set; }
            }

            partial interface ↓IGroup0
            {
                int Property1 { get; set; }
            }

            [TypeConstraintProfileGroup]
            partial interface IGroup0
            {
            }

            partial interface ↓IGroup0
            {
                int Property2 { get; set; }
            }

            partial interface IGroup0
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
