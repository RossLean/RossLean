using RossLean.Common.Test;
using System.Runtime.CompilerServices;

namespace RossLean.GenericsAnalyzer.Test;

public class GuRoslynAssertsSetup : BaseGuRoslynAssertsSetup
{
    [ModuleInitializer]
    public static void Setup()
    {
        SetupDefaultSettings<GuRoslynAssertsSetup>();
    }
}
