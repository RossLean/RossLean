using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace RossLean.GenericsAnalyzer.AnalysisTestsBase.Verifiers;

public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, NUnitVerifier>
    {
        public Test()
        {
            CSharpVerifierHelper.SetupNET6AndDependencies(this);
        }
    }
}
