using Microsoft.CodeAnalysis;
using System;

namespace RossLean.GenericsAnalyzer.Core;

public class TypeConstraintProfileGroupInfo : IEquatable<TypeConstraintProfileGroupInfo>
{
    public INamedTypeSymbol GroupDeclaringInterface { get; }

    public bool Distinct { get; }

    public TypeConstraintProfileGroupInfo(INamedTypeSymbol groupDeclaringInterface, bool distinct)
    {
        GroupDeclaringInterface = groupDeclaringInterface;
        Distinct = distinct;
    }

    public bool Equals(TypeConstraintProfileGroupInfo group)
    {
        return SymbolEqualityComparer.Default.Equals(GroupDeclaringInterface, group.GroupDeclaringInterface);
    }
    public override bool Equals(object obj)
    {
        return obj is TypeConstraintProfileGroupInfo group && Equals(group);
    }
    public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(GroupDeclaringInterface);
}
