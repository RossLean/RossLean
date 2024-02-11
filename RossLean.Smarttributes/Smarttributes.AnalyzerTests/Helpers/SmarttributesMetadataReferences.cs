using Microsoft.CodeAnalysis;
using RoseLynn;
using RossLean.Smarttributes.Constraints;
using System.Collections.Immutable;

namespace RossLean.Smarttributes.AnalyzerTests.Helpers;

public static class SmarttributesMetadataReferences
{
    public static readonly ImmutableArray<MetadataReference> BaseReferences =
        ImmutableArray.Create<MetadataReference>(
        [
            // Smarttributes.Core
            MetadataReferenceFactory.CreateFromType<RequiresPresenceAttribute>(),
        ]);
}
