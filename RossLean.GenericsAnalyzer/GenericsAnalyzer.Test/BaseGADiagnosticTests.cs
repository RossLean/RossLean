using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoseLynn;
using RoseLynn.Analyzers;
using RoseLynn.Testing;
using RossLean.GenericsAnalyzer.AnalysisTestsBase.Helpers;

namespace RossLean.GenericsAnalyzer.Test;

public abstract class BaseGADiagnosticTests<TAnalyzer> : BaseGADiagnosticTests
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected sealed override DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance() => new TAnalyzer();
}

public abstract class BaseGADiagnosticTests : BaseDiagnosticTests
{
    protected ExpectedDiagnostic ExpectedDiagnostic => ExpectedDiagnostic.Create(TestedDiagnosticRule);
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => GADiagnosticDescriptorStorage.Instance;

    protected override UsingsProviderBase GetNewUsingsProviderInstance()
    {
        return GAUsingsProvider.Instance;
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
        ValidateCode(string.Empty);
    }
    [Test]
    public void EmptyCodeWithUsings()
    {
        ValidateCode(GAUsingsProvider.DefaultUsings);
    }
}
