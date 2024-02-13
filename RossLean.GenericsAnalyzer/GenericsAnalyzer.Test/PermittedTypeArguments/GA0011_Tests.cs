using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0011_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void RedundantPermissions()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes(↓typeof(int), ↓typeof(short))]
                [ProhibitedTypes(typeof(long))]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void RedundantPermissionsOnRestrictedPermissions()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes(↓typeof(int), ↓typeof(short))]
                [PermittedBaseTypes(typeof(IComparable<>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void DeepInheritanceTree()
    {
        var testCode =
            """
            class A
            <
                [ProhibitedBaseTypes(typeof(IA))]
                [PermittedBaseTypes(typeof(ID), ↓typeof(IE))]
                [ProhibitedBaseTypes(typeof(IF))]
                [PermittedBaseTypes(typeof(IG), ↓typeof(IH))]
                [ProhibitedBaseTypes(typeof(II))]
                T
            >
            {
            }

            interface IBase { }
            interface IA : IBase { }
            interface IB : IA { }
            interface IC : IB { }
            interface ID : IC { }
            interface IE : ID { }
            interface IF : IE { }
            interface IG : IF { }
            interface IH : IG { }
            interface II : IH { }
            interface IJ : II { }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
