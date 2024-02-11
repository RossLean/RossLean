namespace RossLean.Smarttributes.Constraints;

/// <summary>
/// Denotes that the target attribute requires meeting some requirements
/// about the attribute targets.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class RequiredAttributeTargetsAttribute : Attribute
{
    /// <summary>
    /// The restriction kind applied to the comparison of the restriction values.
    /// </summary>
    public RestrictionKind RestrictionKind { get; }
    /// <summary>
    /// The attribute targets that are part of the restriction, in conjuction with
    /// the <seealso cref="RestrictionKind"/>.
    /// </summary>
    public AttributeTargets Targets { get; }

    /// <inheritdoc cref="RequiredAttributeTargetsAttribute"/>
    /// <param name="restrictionKind">
    /// The restriction kind applied to the comparison of the restriction values.
    /// </param>
    /// <param name="targets">
    /// The attribute targets that are part of the restriction, in conjuction with
    /// the <paramref name="restrictionKind"/>.
    /// </param>
    public RequiredAttributeTargetsAttribute(RestrictionKind restrictionKind, AttributeTargets targets)
    {
        RestrictionKind = restrictionKind;
        Targets = targets;
    }
}