using System;

namespace RossLean.GenericsAnalyzer.Core;

/// <summary>
/// Common attribute targets for GenericsAnalyzer
/// </summary>
internal static class CommonAttributeTargets
{
    /// <summary>
    /// Denotes that an attribute is valid on a generic type parameter and a type constraint
    /// profile interface.
    /// </summary>
    public const AttributeTargets TypeParametersAndProfiles
        = AttributeTargets.GenericParameter | AttributeTargets.Interface;
}
