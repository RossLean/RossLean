using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RoseLynn;
using RossLean.NameOn.Core;
using RossLean.NameOn.Test.Verifiers;

namespace RossLean.NameOn.Test;

public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, NUnitVerifier>
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                return solution;
            });

            TestState.AdditionalReferences.AddRange(new[]
            {
                MetadataReferenceFactory.CreateFromType<NameOfString>(),
            });
        }
    }
}
