using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;


public sealed class GA0026_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void InvalidProfileUsage()
    {
        var testCode =
$@"
class ClassA {{ }} 
struct StructA {{ }}
enum EnumA {{ }}
delegate void DelegateA();
record RecordA;

interface IIrrelevant {{ }}

[TypeConstraintProfile]
interface IProfile {{ }}

class ExampleGeneric
<
    [InheritProfileTypeConstraints(↓typeof(void*), ↓typeof(ClassA), ↓typeof(StructA), ↓typeof(EnumA), ↓typeof(DelegateA), ↓typeof(RecordA), ↓typeof(IIrrelevant), typeof(IProfile))]
    T
> {{ }}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
