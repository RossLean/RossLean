using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RossLean.Smarttributes.AnalyzerTests.Verifiers;

namespace Smarttributes.AnalyzerTests.Verifiers;

public static partial class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public class Test : CSharpAnalyzerTest<TAnalyzer, NUnitVerifier>
    {
        public Test()
        {
            CSharpVerifierHelper.SetupNET6AndDependencies(this);
        }
    }
}
