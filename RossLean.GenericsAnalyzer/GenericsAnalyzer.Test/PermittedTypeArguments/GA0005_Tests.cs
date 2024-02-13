using NUnit.Framework;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0005_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void InvalidTypeArguments()
    {
        var testCode =
            """
            class C
            <
                [PermittedTypes(↓typeof(string))]
                [PermittedBaseTypes(typeof(IEnumerable<>))]
                [PermittedBaseTypes(↓typeof(List<>))]
                [PermittedTypes(typeof(IList<int>), typeof(ISet<int>))]
                [OnlyPermitSpecifiedTypes]
                T
            >
                where T : IEnumerable<int>
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void StructConstraint()
    {
        var testCode =
            """
            class C { }
            struct S { }
            struct Managed
            {
                List<int> list;
            }

            class Generic
            <
                [PermittedTypes(↓typeof(string))]
                [ProhibitedTypes(↓typeof(IEnumerable<int>))]
                [ProhibitedBaseTypes(typeof(IEnumerable<uint>))]
                [ProhibitedTypes(typeof(int))]
                [ProhibitedTypes(typeof(Managed))]
                [PermittedBaseTypes(↓typeof(C))]
                T
            >
                where T : struct
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void UnmanagedConstraint()
    {
        var testCode =
            """
            class C { }
            struct S { }
            struct Managed
            {
                List<int> list;
            }

            class Generic
            <
                [PermittedTypes(↓typeof(string))]
                [ProhibitedTypes(↓typeof(IEnumerable<int>))]
                [ProhibitedBaseTypes(typeof(IEnumerable<uint>))]
                [ProhibitedTypes(typeof(int))]
                [ProhibitedTypes(↓typeof(Managed))]
                [PermittedBaseTypes(↓typeof(C))]
                T
            >
                where T : unmanaged
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void ClassConstraint()
    {
        var testCode =
            """
            class C { }
            struct S { }
            struct Managed
            {
                List<int> list;
            }

            class Generic
            <
                [PermittedTypes(typeof(string))]
                [ProhibitedTypes(typeof(IEnumerable<int>))]
                [ProhibitedBaseTypes(typeof(IEnumerable<uint>))]
                [ProhibitedTypes(↓typeof(int))]
                [ProhibitedTypes(↓typeof(Managed))]
                [PermittedBaseTypes(typeof(C))]
                T
            >
                where T : class
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void NewConstraint()
    {
        var testCode =
            """
            class C { }
            struct S { }
            struct Managed
            {
                List<int> list;
            }

            class Generic
            <
                [PermittedTypes(↓typeof(string))]
                [ProhibitedTypes(↓typeof(IEnumerable<int>))]
                [ProhibitedBaseTypes(typeof(IEnumerable<uint>))]
                [ProhibitedTypes(typeof(int))]
                [ProhibitedTypes(typeof(Managed))]
                [PermittedBaseTypes(typeof(C))]
                T
            >
                where T : new()
            {
            }
            """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
