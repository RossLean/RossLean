namespace RossLean.GenericsAnalyzer.Core;

// Aliases like that are great
/// <summary>
/// Determines the filter type to apply to a specific entity.
/// </summary>
public enum FilterType : byte
{
    /// <summary>
    /// Denotes that no filtering will be applied.
    /// This does not affect the results of the processing.
    /// </summary>
    None = default,
    /// <summary>
    /// Denotes that the entity is permitted.
    /// </summary>
    Permitted,
    /// <summary>
    /// Denotes that the entity is prohibited.
    /// </summary>
    Prohibited,
    /// <summary>
    /// Denotes that only the given entity is permitted.
    /// </summary>
    Exclusive,
}
