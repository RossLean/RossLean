using Microsoft.CodeAnalysis;
using RoseLynn;
using RossLean.NameOn.Core;
using System.Collections.Immutable;

namespace RossLean.NameOn.Test.Verifiers;

public static class NameOnMetadataReferences
{
    public static readonly ImmutableArray<MetadataReference> BaseReferences =
        ImmutableArray.Create<MetadataReference>(
        [
            // NameOn.Core
            MetadataReferenceFactory.CreateFromType<NameOfString>(),
        ]);
}
