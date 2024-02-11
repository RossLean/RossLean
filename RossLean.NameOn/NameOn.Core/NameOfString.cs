using System;

namespace RossLean.NameOn.Core;

/// <summary>
/// Represents a string whose assigned expression is a <see langword="nameof"/> expression.
/// </summary>
public readonly struct NameOfString : IEquatable<string>, IEquatable<NameOfString>
{
    /// <summary>The resulting value of the <see langword="nameof"/> expression.</summary>
    public string Value { get; }

    public NameOfString([ForceNameOf] string value) => Value = value;
    public NameOfString(NameOfString value) => Value = value.Value;

    public static implicit operator NameOfString([ForceNameOf] string value) => new(value);

    public static bool operator ==(NameOfString left, NameOfString right) => left.Value == right.Value;
    public static bool operator ==(NameOfString left, [ForceNameOf] string right) => left.Value == right;
    public static bool operator ==([ForceNameOf] string left, NameOfString right) => left == right.Value;

    public static bool operator !=(NameOfString left, NameOfString right) => left.Value != right.Value;
    public static bool operator !=(NameOfString left, [ForceNameOf] string right) => left.Value != right;
    public static bool operator !=([ForceNameOf] string left, NameOfString right) => left != right.Value;

    public bool Equals([ForceNameOf] string other) => Value == other;
    public bool Equals(NameOfString other) => Value == other.Value;

    public override bool Equals(object obj)
    {
        if (obj is string s)
            return Value == s;

        return obj is NameOfString nameOfString && Equals(nameOfString);
    }

    public override int GetHashCode() => Value.GetHashCode();
}
