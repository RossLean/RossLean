using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoseLynn.Analyzers;
using RoseLynn.Testing;
using RossLean.GenericsAnalyzer.AnalysisTestsBase.Helpers;
using RossLean.GenericsAnalyzer.AnalysisTestsBase.Verifiers;
using System.Threading.Tasks;

namespace RossLean.GenericsAnalyzer.CodeFixes.Test;

public abstract class BaseCodeFixTests<TAnalyzer, TCodeFix> : BaseCodeFixDiagnosticTests<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : GACodeFixProvider, new()
{
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => GADiagnosticDescriptorStorage.Instance;

    protected sealed override async Task VerifyCodeFixAsync(string markupCode, string expected, int codeActionIndex)
    {
        await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCodeFixAsync(markupCode, expected, codeActionIndex);
    }

    [Test]
    public void TestExistingCodeFixName()
    {
        Assert.IsNotNull(new TCodeFix().CodeFixTitle);
    }

    public void TestCodeFixWithUsings(string markupCode, string expected, int codeActionIndex = 0)
    {
        TestCodeFix(GAUsingsProvider.Instance.WithUsings(markupCode), GAUsingsProvider.Instance.WithUsings(expected), codeActionIndex);
    }
}
