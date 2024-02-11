using System;

namespace RossLean.NameOn.Core;

/// <summary>Determines the state of a member regarding its <see langword="nameof"/> expressions.</summary>
[Flags]
public enum NameOfState
{
    /// <summary>No relation to <see langword="nameof"/> expressions.</summary>
    None,
    /// <summary>The assigned expression contains <see langword="nameof"/> expressions.</summary>
    Contained,
    /// <summary>The whole assigned expression is a <see langword="nameof"/> expression.</summary>
    Whole,
}