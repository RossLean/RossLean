namespace RossLean.Smarttributes.AnalyzerTests.AttributePresence;

public sealed class SMTTR0011_Tests : ParameterTypeMatchAnalyzerDiagnosticTests
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
                               new object[] { 1, "hello", ↓"false" })]
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
                               1, "hello", ↓"false")]
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
                               new object[] { 1, "hello", ↓"false" },
                               null)]
                           public void MethodExample(int a, string b, bool c) { }
                       }
                       """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
