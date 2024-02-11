using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0029_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void InvalidProfileInterfaces()
    {
        var testCode =
$@"
[TypeConstraintProfileGroup(false)]
interface IGroup0 {{ }}
[TypeConstraintProfileGroup(false)]
interface IGroup1 {{ }}
[TypeConstraintProfileGroup(false)]
interface IGroup2 {{ }}

[TypeConstraintProfile]
[TypeConstraintProfileGroup]
interface ↓IA {{ }}

[TypeConstraintProfile(typeof(IGroup0), typeof(IGroup1), typeof(IGroup2))]
[TypeConstraintProfileGroup(false)]
interface ↓IB {{ }}
";

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void PartialInvalidInterface()
    {
        var testCode =
$@"
[TypeConstraintProfile]
partial interface ↓IA {{ }}
[TypeConstraintProfileGroup]
partial interface ↓IA {{ }}
[Example]
partial interface IA {{ }}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
