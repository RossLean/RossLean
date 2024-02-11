namespace RossLean.Smarttributes.AnalyzerTests.AttributePresence;

public abstract class ParameterTypeMatchAnalyzerDiagnosticTests : BaseSmarttributesDiagnosticTests<ParameterTypeMatchAnalyzer>
{
    public const string ExampleAttributes =
        """
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public sealed class SingleParameterTypeMatchAttribute : Attribute
        {
            public SingleParameterTypeMatchAttribute(
                [ParameterTypeMatch] object?[] parameters) { }
        }
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public sealed class ParamsParameterTypeMatchAttribute : Attribute
        {
            public ParamsParameterTypeMatchAttribute(
                [ParameterTypeMatch] params object?[] parameters) { }
        }
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public sealed class ComplexParameterTypeMatchAttribute : Attribute
        {
            public ComplexParameterTypeMatchAttribute(
                int firstParameter,
                string secondParameter,
                [ParameterTypeMatch] object?[] parameters,
                object? lastParameter) { }
        }
        """;
}
