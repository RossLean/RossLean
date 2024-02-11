using RoseLynn.Analyzers;

namespace RossLean.Smarttributes;

public abstract class SmarttributesDiagnosticAnalyzer : StoredDescriptorDiagnosticAnalyzer
{
    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage
        => SmarttributesDiagnosicDescriptorStorage.Instance;
}
