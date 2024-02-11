using System;

namespace RossLean.NameOn.Core;

/// <summary>Determines the restriction type of <see langword="nameof"/> expressions being substituted.</summary>
[Flags]
public enum NameOfRestrictions : uint
{
    /// <summary>No restrictions are applied.</summary>
    None,
    /// <summary>The string expression must be a <see langword="nameof"/> expression. Concatenations of multiple expressions (<see langword="nameof"/> or not) are invalid.</summary>
    Force = 1,
    /// <summary>The string expression must contain at least one <see langword="nameof"/> expression.</summary>
    ForceContained = 1 << 1,

    /// <summary>The string expression must not be a <see langword="nameof"/> expression. Concatenations of multiple <see langword="nameof"/> expressions are valid.</summary>
    Prohibit = Force << 12,
    /// <summary>The string expression must contain no <see langword="nameof"/> expressions. The string expression itself should also not be a <see langword="nameof"/> expression.</summary>
    ProhibitContained = ForceContained << 12,
}