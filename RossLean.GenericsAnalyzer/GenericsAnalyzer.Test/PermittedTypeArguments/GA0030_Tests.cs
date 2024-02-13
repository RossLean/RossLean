using NUnit.Framework;
using RossLean.GenericsAnalyzer.Core;

namespace RossLean.GenericsAnalyzer.Test.PermittedTypeArguments;

public sealed class GA0030_Tests : PermittedTypeArgumentAnalyzerDiagnosticTests
{
    [Test]
    public void NonProfileInterfaceWithProfileRelatedAttributes()
    {
        TestInterfaceWithProfileRelatedAttributes(null, true);
    }
    [Test]
    public void ProfileGroupInterfaceWithProfileRelatedAttributes()
    {
        TestInterfaceWithProfileRelatedAttributes(nameof(TypeConstraintProfileGroupAttribute), true);
    }
    [Test]
    public void ProfileInterfaceWithProfileRelatedAttributes()
    {
        TestInterfaceWithProfileRelatedAttributes(nameof(TypeConstraintProfileAttribute), false);
    }

    private void TestInterfaceWithProfileRelatedAttributes(string profileAttribute, bool assertDiagnostics)
    {
        if (!string.IsNullOrEmpty(profileAttribute))
            profileAttribute = $"[{profileAttribute}]";

        var testCode =
            $$"""
            // Individually test every attribute
            [↓PermittedTypes(typeof(int))]
            {{profileAttribute}}
            interface IA { }
            [↓ProhibitedTypes(typeof(long))]
            {{profileAttribute}}
            interface IB { }
            [↓PermittedBaseTypes(typeof(IEnumerable))]
            {{profileAttribute}}
            interface IC { }
            [↓ProhibitedBaseTypes(typeof(ICollection))]
            {{profileAttribute}}
            interface ID { }
            [↓OnlyPermitSpecifiedTypes]
            {{profileAttribute}}
            interface IE { }
            """;

        AssertOrValidateWithUsings(testCode, assertDiagnostics);
    }
}
