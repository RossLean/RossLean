using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.Analyzers;
using RoseLynn.Testing;
using RoseLynn;

namespace RossLean.Smarttributes.AnalyzerTests;

public abstract class BaseSmarttributesDiagnosticTests<TAnalyzer> : BaseSmarttributesDiagnosticTests
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected sealed override DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance() => new TAnalyzer();
}

public abstract class BaseSmarttributesDiagnosticTests : BaseDiagnosticTests
{
    protected ExpectedDiagnostic ExpectedDiagnostic => ExpectedDiagnostic.Create(TestedDiagnosticRule);
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => SmarttributesDiagnosticDescriptorStorage.Instance;

    protected override UsingsProviderBase GetNewUsingsProviderInstance()
    {
        return SmarttributesUsingsProvider.Instance;
    }

    protected override void ValidateCode(string testCode)
    {
        RoslynAssert.Valid(GetNewDiagnosticAnalyzerInstance(), testCode);
    }
    protected override void AssertDiagnostics(string testCode)
    {
        RoslynAssert.Diagnostics(GetNewDiagnosticAnalyzerInstance(), ExpectedDiagnostic, testCode);
    }

    [Test]
    public void EmptyCode()
    {
        ValidateCode(@"");
    }
    [Test]
    public void EmptyCodeWithUsings()
    {
        ValidateCode(SmarttributesUsingsProvider.DefaultUsings);
    }
}
