using System;

namespace RossLean.GenericsAnalyzer.Core;

using static TypeConstraintRule;

/// <summary>Denotes that a generic type argument permits the usage of the specified types and the types' inheritors. That means, if a type inherits one of the base types that are provided in this attribute, it is permitted too.</summary>
public class PermittedBaseTypesAttribute : ConstrainedTypesAttributeBase
{
    protected override TypeConstraintRule Rule => PermitBaseType;

    /// <summary>Initializes a new instance of the <seealso cref="PermittedBaseTypesAttribute"/> from the given permitted types.</summary>
    /// <param name="permittedBaseTypes">The base types that are permitted as a generic type argument for the marked generic type.</param>
    public PermittedBaseTypesAttribute(params Type[] permittedBaseTypes)
        : base(permittedBaseTypes) { }
}
