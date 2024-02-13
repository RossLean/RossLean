namespace RossLean.GenericsAnalyzer.Core;

#nullable enable

/// <summary>
/// Denotes a constraint capable of having an accompanying reason.
/// </summary>
public interface IReasonedConstraint
{
    /// <summary>
    /// Gets the reason for this constraint.
    /// </summary>
    public string? Reason { get; }
}
