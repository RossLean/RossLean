using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using RossLean.Common.Test;
using RossLean.Smarttributes.AnalyzerTests.Helpers;
using System.Collections.Immutable;

namespace RossLean.Smarttributes.AnalyzerTests.Verifiers;

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
            SmarttributesMetadataReferences.BaseReferences,
            ReferenceAssemblies.Net.Net60);
    }
}
