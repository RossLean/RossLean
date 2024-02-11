using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoseLynn.Analyzers;
using RoseLynn.Testing;
using System.Threading.Tasks;

namespace RossLean.NameOn.Test;

public abstract class BaseCodeFixTests<TAnalyzer, TCodeFix> : BaseCodeFixDiagnosticTests<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : NAMECodeFixProvider, new()
{
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => NAMEDiagnosticDescriptorStorage.Instance;

    protected sealed override async Task VerifyCodeFixAsync(string markupCode, string expected, int codeActionIndex)
    {
        await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCodeFixAsync(markupCode, expected, codeActionIndex);
    }

    [Test]
    public void TestExistingCodeFixName()
    {
        Assert.IsNotNull(new TCodeFix().CodeFixTitle);
    }
}
