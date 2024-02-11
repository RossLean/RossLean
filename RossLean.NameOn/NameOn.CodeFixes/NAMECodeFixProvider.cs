using RoseLynn.CodeFixes;
using System.Resources;

namespace RossLean.NameOn;

public abstract class NAMECodeFixProvider : MultipleDiagnosticCodeFixProvider
{
    protected sealed override ResourceManager ResourceManager => CodeFixResources.ResourceManager;
}
