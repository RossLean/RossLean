namespace RossLean.Smarttributes.Constraints;

/// <summary>
/// Specifies the comparison type against the restriction values.
/// </summary>
public enum RestrictionKind
{
    /// <summary>
    /// Denotes that there should be at least one value from the specified ones.
    /// </summary>
    AnyFromSpecified,
    /// <summary>
    /// Denotes that there should be no value from the specified ones present.
    /// </summary>
    NoneFromSpecified,
    /// <summary>
    /// Denotes that there should be all the specified values present.
    /// </summary>
    Exact,
    /// <summary>
    /// Denotes that there should be at least all the specified values present.
    /// </summary>
    Minimum,
}
