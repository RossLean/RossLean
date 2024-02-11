using Microsoft.CodeAnalysis;
using RoseLynn;
using RossLean.GenericsAnalyzer.AnalysisTestsBase.Resources;
using RossLean.GenericsAnalyzer.Core;
using System.Collections.Immutable;

namespace RossLean.GenericsAnalyzer.AnalysisTestsBase.Helpers;

public static class GenericsAnalyzerMetadataReferences
{
    public static readonly ImmutableArray<MetadataReference> BaseReferences =
        ImmutableArray.Create<MetadataReference>(
        [
            // GenericsAnalyzer.Core
            MetadataReferenceFactory.CreateFromType<IGenericTypeConstraintAttribute>(),
            // GenericsAnalyzer.AnalysisTestBase
            MetadataReferenceFactory.CreateFromType<ExampleAttribute>(),
        ]);
}
