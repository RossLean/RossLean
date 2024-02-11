using System;

namespace RossLean.GenericsAnalyzer.Core;

public struct TypeConstraintRule : IEquatable<TypeConstraintRule>
{
    public static TypeConstraintRule PermitExactType => new(ConstraintRule.Permit, TypeConstraintReferencePoint.ExactType);
    public static TypeConstraintRule PermitBaseType => new(ConstraintRule.Permit, TypeConstraintReferencePoint.BaseType);
    public static TypeConstraintRule ProhibitExactType => new(ConstraintRule.Prohibit, TypeConstraintReferencePoint.ExactType);
    public static TypeConstraintRule ProhibitBaseType => new(ConstraintRule.Prohibit, TypeConstraintReferencePoint.BaseType);

    public static TypeConstraintRule[] AllValidRules => new[] { PermitExactType, PermitBaseType, ProhibitExactType, ProhibitBaseType };

    public ConstraintRule Rule { get; set; }
    public TypeConstraintReferencePoint TypeReferencePoint { get; set; }

    public TypeConstraintRule(ConstraintRule rule, TypeConstraintReferencePoint referecePoint)
    {
        Rule = rule;
        TypeReferencePoint = referecePoint;
    }
    public TypeConstraintRule(TypeConstraintRule other)
        : this(other.Rule, other.TypeReferencePoint) { }

    public bool FullySatisfies(TypeConstraintRule other)
    {
        if (this == other)
            return true;

        if (Rule != other.Rule)
            return false;

        if (other.TypeReferencePoint != TypeConstraintReferencePoint.ExactType)
            return false;

        return true;
    }

    public static bool operator ==(TypeConstraintRule left, TypeConstraintRule right) => left.Equals(right);
    public static bool operator !=(TypeConstraintRule left, TypeConstraintRule right) => !left.Equals(right);

    public bool Equals(TypeConstraintRule other) => Rule == other.Rule && TypeReferencePoint == other.TypeReferencePoint;
    public override bool Equals(object obj) => obj is TypeConstraintRule rule && Equals(rule);

    public override int GetHashCode()
    {
        return TypeReferencePoint.GetHashCode() | Rule.GetHashCode() << sizeof(TypeConstraintReferencePoint) * 8;
    }
    public override string ToString() => $"{Rule} {TypeReferencePoint}";
}
