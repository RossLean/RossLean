namespace RossLean.Smarttributes.Constraints;

/// <summary>
/// Denotes the type targets for an attribute.
/// </summary>
[Flags]
public enum TypeTargets
{
    // Non-records

    /// <summary>
    /// Represents a <see langword="class"/>.
    /// </summary>
    Class = 1 << 0,
    /// <summary>
    /// Represents a <see langword="struct"/>.
    /// </summary>
    Struct = 1 << 1,
    /// <summary>
    /// Represents an <see langword="interface"/>.
    /// </summary>
    Interface = 1 << 2,
    /// <summary>
    /// Represents a <see langword="delegate"/>.
    /// </summary>
    Delegate = 1 << 3,
    /// <summary>
    /// Represents an <see langword="enum"/>.
    /// </summary>
    Enum = 1 << 4,

    /// <summary>
    /// Represents any valid non-record type target.
    /// </summary>
    AnyNonRecord = Class | Struct | Interface | Delegate | Enum,

    // Records

    /// <summary>
    /// Represents a <see langword="record class"/>.
    /// </summary>
    RecordClass = 1 << 8,
    /// <summary>
    /// Represents a <see langword="record struct"/>.
    /// </summary>
    RecordStruct = 1 << 9,

    /// <summary>
    /// Represents any valid record type target.
    /// </summary>
    AnyRecord = RecordClass | RecordStruct,

    /// <summary>
    /// Represents a <see langword="class"/> or a <see langword="record class"/>.
    /// </summary>
    AnyClass = Class | RecordClass,
    /// <summary>
    /// Represents a <see langword="struct"/> or a <see langword="record struct"/>.
    /// </summary>
    AnyStruct = Struct | RecordStruct,

    /// <summary>
    /// Represents any valid type target.
    /// </summary>
    All = AnyNonRecord | AnyRecord,
}
