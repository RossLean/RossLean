using System;

namespace RossLean.GenericsAnalyzer.Core;

#nullable enable

/// <summary>
/// Denotes that a generic type argument prohibits the usage of the specified types.
/// </summary>
public class ProhibitedTypesAttribute
    : ConstrainedTypesAttributeBase, IReasonedConstraint
{
    protected override TypeConstraintRule Rule
        => TypeConstraintRule.ProhibitExactType with
        {
            Reason = Reason,
        };

    /// <summary>
    /// The reason for this prohibition.
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// Initializes a new instance of the
    /// <seealso cref="ProhibitedTypesAttribute"/> from the given prohibited types.
    /// </summary>
    /// <param name="prohibitedTypes">
    /// The types that are prohibited as a generic type argument
    /// for the marked generic type.
    /// </param>
    public ProhibitedTypesAttribute(params Type[] prohibitedTypes)
        : base(prohibitedTypes) { }
}
