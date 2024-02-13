using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0008_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void RedundantBaseTypeRules()
    {
        var testCode =
            """
            class C
            <
                [PermittedBaseTypes(typeof(Class))]
                [PermittedBaseTypes(typeof(Record))]
                [PermittedBaseTypes(↓typeof(SealedClass))]
                [PermittedBaseTypes(↓typeof(SealedRecord))]
                [PermittedBaseTypes(↓typeof(Struct))]
                [PermittedBaseTypes(↓typeof(Struct?))]
                [PermittedBaseTypes(↓typeof((Struct, Record)))]
                [PermittedBaseTypes(typeof(Interface))]
                [PermittedBaseTypes(↓typeof(EnumA))]
                [PermittedBaseTypes(↓typeof(DelegateA))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }

            class Class { }
            record Record { }
            sealed class SealedClass { }
            sealed record SealedRecord { }
            struct Struct { }
            interface Interface { }
            enum EnumA { }
            delegate void DelegateA();
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
