using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RossLean.NameOn.Test.Verifiers;

namespace RossLean.NameOn.Test;

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
