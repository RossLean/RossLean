using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoseLynn;
using RoseLynn.Analyzers;
using RoseLynn.Testing;

namespace RossLean.NameOn.Test;

public abstract class BaseNAMEDiagnosticTests<TAnalyzer> : BaseNAMEDiagnosticTests
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected sealed override DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance() => new TAnalyzer();
}

public abstract class BaseNAMEDiagnosticTests : BaseDiagnosticTests
{
    protected ExpectedDiagnostic ExpectedDiagnostic => ExpectedDiagnostic.Create(TestedDiagnosticRule);
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => NAMEDiagnosticDescriptorStorage.Instance;

    protected override UsingsProviderBase GetNewUsingsProviderInstance()
    {
        return NAMEUsingsProvider.Instance;
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
        ValidateCode(NAMEUsingsProvider.DefaultUsings);
    }
}
