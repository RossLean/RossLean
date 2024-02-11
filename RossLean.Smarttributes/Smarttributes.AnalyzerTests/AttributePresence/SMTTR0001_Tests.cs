namespace RossLean.Smarttributes.AnalyzerTests.AttributePresence;

public sealed class SMTTR0001_Tests : AttributePresenceAnalyzerDiagnosticTests
{
    [Test]
    public void MissingDeclaredAttribute()
    {
        var testCode = """
                       [RequiresPresence<AttributeUsageAttribute>]
                       public sealed class ExampleAttribute : Attribute { }

                       // There should be an AttributeUsageAttribute here
                       [↓Example]
                       public sealed class CustomAttribute : Attribute { }
                       """;

        AssertDiagnosticsWithUsings(testCode);
    }

    [Test]
    public void MultipleMissingDeclaredAttributes()
    {
        var testCode = """
                       [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
                       public sealed class DummyAttribute : Attribute { }

                       public sealed class Constraint0Attribute : Attribute { }
                       public sealed class Constraint1Attribute : Attribute { }
                       public sealed class Constraint2Attribute : Attribute { }
                       public sealed class Constraint3Attribute : Attribute { }
                       public sealed class Constraint4Attribute : Attribute { }
                       public sealed class Constraint5Attribute : Attribute { }
                       public sealed class Constraint6Attribute : Attribute { }
                       public sealed class Constraint7Attribute : Attribute { }

                       [RequiresPresence<AttributeUsageAttribute>]
                       [RequiresPresence<Constraint0Attribute, Constraint1Attribute>]
                       public sealed class Example0Attribute : Attribute { }

                       [RequiresPresence<AttributeUsageAttribute>]
                       [RequiresPresence<Constraint2Attribute>]
                       [RequiresPresence<Constraint3Attribute, Constraint4Attribute>]
                       public sealed class Example1Attribute : Attribute { }

                       // Example0 and Example1 missing AttributeUsage
                       [Dummy, ↓Example0]
                       [↓Example1, Dummy]
                       [Constraint0, Constraint1, Constraint2, Constraint3, Constraint4]
                       public sealed class Sample0Attribute : Attribute { }

                       // Example0 and Example1 missing their custom constraint attributes
                       [Dummy, ↓Example0]
                       [↓Example1, Dummy]
                       [AttributeUsage(AttributeTargets.All)]
                       [Constraint7]
                       public sealed class Sample1Attribute : Attribute { }
                       """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
