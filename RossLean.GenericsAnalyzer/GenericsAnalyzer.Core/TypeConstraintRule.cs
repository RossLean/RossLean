using System;

namespace RossLean.GenericsAnalyzer.Core;

#nullable enable

public record struct TypeConstraintRule(
    ConstraintRule Rule, TypeConstraintReferencePoint TypeReferencePoint)
    : IEquatable<TypeConstraintRule>
{
    public static TypeConstraintRule PermitExactType
        => new(ConstraintRule.Permit, TypeConstraintReferencePoint.ExactType);
    public static TypeConstraintRule PermitBaseType
        => new(ConstraintRule.Permit, TypeConstraintReferencePoint.BaseType);
    public static TypeConstraintRule ProhibitExactType
        => new(ConstraintRule.Prohibit, TypeConstraintReferencePoint.ExactType);
    public static TypeConstraintRule ProhibitBaseType
        => new(ConstraintRule.Prohibit, TypeConstraintReferencePoint.BaseType);

    public static TypeConstraintRule[] AllValidRules
        => [PermitExactType, PermitBaseType, ProhibitExactType, ProhibitBaseType];

    /// <summary>
    /// A custom reason on why the constraint is placed. This must be uniquely
    /// attributed per instance of restriction.
    /// </summary>
    public string? Reason { get; set; }

    public readonly bool FullySatisfies(TypeConstraintRule other)
    {
        if (this == other)
            return true;

        if (Rule != other.Rule)
            return false;

        if (other.TypeReferencePoint != TypeConstraintReferencePoint.ExactType)
            return false;

        return true;
    }

    public readonly bool Equals(TypeConstraintRule other) => Rule == other.Rule && TypeReferencePoint == other.TypeReferencePoint;

    public override readonly int GetHashCode()
    {
        return TypeReferencePoint.GetHashCode()
            | Rule.GetHashCode() << sizeof(TypeConstraintReferencePoint) * 8;
    }
    public override readonly string ToString() => $"{Rule} {TypeReferencePoint}";
}
