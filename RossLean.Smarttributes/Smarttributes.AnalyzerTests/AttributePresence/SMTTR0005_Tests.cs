namespace RossLean.Smarttributes.AnalyzerTests.AttributePresence;

public sealed class SMTTR0005_Tests : RestrictFunctionsAnalyzerDiagnosticTests
{
    [Test]
    public void RestrictedFunctionKinds()
    {
        var testCode = """
                       public class Test
                       {
                           [Custom1]
                           [↓Custom4]
                           public void Method()
                           {
                               [↓Custom1]
                               void Local()
                               {

                               }

                               Func<int, int> lambda = x => x + 1;
                               var lambda2 = [↓Custom2] (int x) => x + 1;

                               var lambda3 =
                                   [Custom3, ↓Custom1]
                                   [Custom4]
                                   (int x) => x + 1;
                           }
                       }

                       [RestrictFunctions(FunctionTargets.Method)]
                       public sealed class Custom1Attribute : Attribute { }

                       [RestrictFunctions(FunctionTargets.Method | FunctionTargets.LocalMethod)]
                       public sealed class Custom2Attribute : Attribute { }

                       [RestrictFunctions(FunctionTargets.All)]
                       public sealed class Custom3Attribute : Attribute { }

                       [RestrictFunctions(FunctionTargets.AnonymousMethod | FunctionTargets.Lambda)]
                       public sealed class Custom4Attribute : Attribute { }
                       """;

        AssertDiagnosticsWithUsings(testCode);
    }
}
