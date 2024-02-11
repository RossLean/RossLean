using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0025_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void UnrelatedProfileInterfaces()
    {
        var testCode =
$@"
[TypeConstraintProfile]
interface IA {{ }}

interface IB : ↓IA {{ }}

[TypeConstraintProfile]
interface IC : ↓IB {{ }}

[TypeConstraintProfile]
interface ID : IC {{ }}

interface IE {{ }}

[TypeConstraintProfile]
interface IF : IA, ↓IB, ↓IE {{ }}
";

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void ProfileInterfacesAndProfileGroupInterfaces()
    {
        var testCode =
$@"
[TypeConstraintProfile]
interface IA {{ }}

[TypeConstraintProfileGroup]
interface IB : ↓IA {{ }}

[TypeConstraintProfile]
interface IC : ↓IB {{ }}
";

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void PartialInterfaceInheritingMixed()
    {
        var testCode =
$@"
[TypeConstraintProfile]
interface IA {{ }}

interface IB {{ }}

[TypeConstraintProfile]
interface IC {{ }}

[TypeConstraintProfile]
interface ID {{ }}

[TypeConstraintProfile]
partial interface IE : IA, IC, ↓IB {{ }}
partial interface IE : ↓IB, ID, IA {{ }}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
