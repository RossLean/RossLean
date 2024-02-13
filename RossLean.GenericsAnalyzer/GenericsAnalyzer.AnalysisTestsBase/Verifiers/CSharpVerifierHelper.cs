using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using RossLean.Common.Test;
using RossLean.GenericsAnalyzer.AnalysisTestsBase.Helpers;
using System.Collections.Immutable;

namespace RossLean.GenericsAnalyzer.AnalysisTestsBase.Verifiers;

public static class CSharpVerifierHelper
{
    /// <inheritdoc cref="CommonCSharpVerifierHelper.NullableWarnings"/>
    internal static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings
        => CommonCSharpVerifierHelper.NullableWarnings;

    public static void SetupNET6AndDependencies<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        CommonCSharpVerifierHelper.SetupNetReferenceAndDependencies(
            test,
            GenericsAnalyzerMetadataReferences.BaseReferences,
            ReferenceAssemblies.Net.Net60);
    }
}
