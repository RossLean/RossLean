using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using System;
using System.Collections.Immutable;

namespace RossLean.Common.Test;

public static class CommonCSharpVerifierHelper
{
    /// <summary>
    /// By default, the compiler reports diagnostics for nullable reference types at
    /// <see cref="DiagnosticSeverity.Warning"/>, and the analyzer test framework defaults to only validating
    /// diagnostics at <see cref="DiagnosticSeverity.Error"/>. This map contains all compiler diagnostic IDs
    /// related to nullability mapped to <see cref="ReportDiagnostic.Error"/>, which is then used to enable all
    /// of these warnings for default validation during analyzer and code fix tests.
    /// </summary>
    public static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings { get; } = GetNullableWarningsFromCompiler();

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(
            args: ["/warnaserror:nullable"],
            baseDirectory: Environment.CurrentDirectory,
            sdkDirectory: Environment.CurrentDirectory);
        var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

        return nullableWarnings;
    }

    public static void SetupNetReferenceAndDependencies<TVerifier>(
        AnalyzerTest<TVerifier> test,
        ImmutableArray<MetadataReference> metadataReferences,
        ReferenceAssemblies referenceAssemblies)
        where TVerifier : IVerifier, new()
    {
        SetupSolutionTransforms(test);
        test.ReferenceAssemblies = referenceAssemblies;
        test.TestState.AdditionalReferences.AddRange(metadataReferences);
    }

    private static void SetupSolutionTransforms<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        test.SolutionTransforms.Add((solution, projectId) =>
        {
            var compilationOptions = solution.GetProject(projectId)!.CompilationOptions!;
            compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                compilationOptions.SpecificDiagnosticOptions.SetItems(NullableWarnings));
            solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

            return solution;
        });
    }
}
