using RossLean.Common.Test;
using System.Runtime.CompilerServices;

namespace RossLean.Smarttributes.AnalyzerTests;

public class GuRoslynAssertsSetup : BaseGuRoslynAssertsSetup
{
    [ModuleInitializer]
    public static void Setup()
    {
        SetupDefaultSettings<GuRoslynAssertsSetup>();
    }
}
