using RoseLynn.CodeFixes;
using System.Resources;

namespace RossLean.GenericsAnalyzer;

public abstract class GACodeFixProvider : MultipleDiagnosticCodeFixProvider
{
    protected sealed override ResourceManager ResourceManager => CodeFixResources.ResourceManager;
}
