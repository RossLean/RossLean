namespace RossLean.Smarttributes.AnalyzerTests.AttributePresence;

public sealed class SMTTR0010_Tests : ParameterTypeMatchAnalyzerDiagnosticTests
{
    [Test]
    public void SingleParameterTypeMatching()
    {
        var testCode = $$"""
                       {{ExampleAttributes}}

                       public class MethodExampleClass
                       {
                           [SingleParameterTypeMatch(
                               new object[] { 1, "hello", false })]
                           [SingleParameterTypeMatch(
                               ↓new object[] { "hello", 1, false, null, "extra" })]
                           public void MethodExample(int a, string b, bool c) { }
                       }
                       """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void ParamsParameterTypeMatching()
    {
        var testCode = $$"""
                       {{ExampleAttributes}}

                       public class MethodExampleClass
                       {
                           [ParamsParameterTypeMatch(
                               1, "hello", false)]
                           [ParamsParameterTypeMatch(
                               ↓"hello", 1, false, null, "extra")]
                           public void MethodExample(int a, string b, bool c) { }
                       }
                       """;

        AssertDiagnosticsWithUsings(testCode);
    }
    [Test]
    public void ComplexParameterTypeMatching()
    {
        var testCode = $$"""
                       {{ExampleAttributes}}

                       public class MethodExampleClass
                       {
                           [ComplexParameterTypeMatch(
                               1,
                               "hello",
                               new object[] { 1, "hello", false },
                               null)]
                           [ComplexParameterTypeMatch(
                               1,
                               "hello",
                               ↓new object[] { "hello", 1, false, null, "extra" },
                               null)]
                           public void MethodExample(int a, string b, bool c) { }
                       }
                       """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
