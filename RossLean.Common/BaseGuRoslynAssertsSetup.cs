using Gu.Roslyn.Asserts;

namespace RossLean.Common.Test;

public class BaseGuRoslynAssertsSetup
{
    public static void SetupDefaultSettings<TSetup>()
    {
        Settings.Default = Settings.Default
            .WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.WarningsAndErrors)
            .WithMetadataReferences(MetadataReferences.Transitive(typeof(TSetup)));
    }
}
