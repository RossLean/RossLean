using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using RossLean.Smarttributes.AnalyzerTests.Helpers;
using System.Collections.Immutable;

namespace RossLean.Smarttributes.AnalyzerTests.Verifiers;

public static class CSharpVerifierHelper
{
    /// <summary>
    /// By default, the compiler reports diagnostics for nullable reference types at
    /// <see cref="DiagnosticSeverity.Warning"/>, and the analyzer test framework defaults to only validating
    /// diagnostics at <see cref="DiagnosticSeverity.Error"/>. This map contains all compiler diagnostic IDs
    /// related to nullability mapped to <see cref="ReportDiagnostic.Error"/>, which is then used to enable all
    /// of these warnings for default validation during analyzer and code fix tests.
    /// </summary>
    internal static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings { get; } = GetNullableWarningsFromCompiler();

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(
            args: ["/warnaserror:nullable"],
            baseDirectory: Environment.CurrentDirectory,
            sdkDirectory: Environment.CurrentDirectory);
        var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

        return nullableWarnings;
    }

    public static void SetupNET6AndDependencies<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        SetupSolutionTransforms(test);

        test.ReferenceAssemblies = ReferenceAssemblies.Net.Net60;

        SetupSmarttributesDependencies(test);
    }
    public static void SetupSmarttributesDependencies<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        test.TestState.AdditionalReferences.AddRange(SmarttributesMetadataReferences.BaseReferences);
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
