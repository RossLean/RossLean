using RossLean.Common.Test;
using System.Runtime.CompilerServices;

namespace RossLean.NameOn.Test;

public class GuRoslynAssertsSetup : BaseGuRoslynAssertsSetup
{
    [ModuleInitializer]
    public static void Setup()
    {
        SetupDefaultSettings<GuRoslynAssertsSetup>();
    }
}
