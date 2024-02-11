using System;

namespace RossLean.GenericsAnalyzer.Core;

using static TypeConstraintRule;

/// <summary>Denotes that a generic type argument permits the usage of the specified types.</summary>
public class PermittedTypesAttribute : ConstrainedTypesAttributeBase
{
    protected override TypeConstraintRule Rule => PermitExactType;

    /// <summary>Initializes a new instance of the <seealso cref="PermittedTypesAttribute"/> from the given permitted types.</summary>
    /// <param name="permittedTypes">The types that are permitted as a generic type argument for the marked generic type.</param>
    public PermittedTypesAttribute(params Type[] permittedTypes)
        : base(permittedTypes) { }
}
