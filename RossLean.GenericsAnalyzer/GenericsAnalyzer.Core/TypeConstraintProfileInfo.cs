using Microsoft.CodeAnalysis;
using RoseLynn.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;

public class TypeConstraintProfileInfo : IEquatable<TypeConstraintProfileInfo>
{
    private readonly HashSet<TypeConstraintProfileGroupInfo> groups = new();

    public TypeConstraintSystem System { get; private set; }
    public TypeConstraintSystem.Builder Builder { get; }

    public IEnumerable<TypeConstraintProfileGroupInfo> Groups => groups.ToArray();

    public INamedTypeSymbol ProfileDeclaringInterface => Builder.ProfileInterface;

    public TypeConstraintProfileInfo(INamedTypeSymbol profileDeclaringInterface)
    {
        Builder = new TypeConstraintSystem.Builder(profileDeclaringInterface);
    }
    public TypeConstraintProfileInfo(INamedTypeSymbol profileDeclaringInterface, IEnumerable<TypeConstraintProfileGroupInfo> groupInfos)
        : this(profileDeclaringInterface)
    {
        AddToGroups(groupInfos);
    }

    public void AddToGroup(TypeConstraintProfileGroupInfo groupInfo) => groups.Add(groupInfo);
    public void AddToGroups(params TypeConstraintProfileGroupInfo[] groupInfos) => groups.AddRange(groupInfos);
    public void AddToGroups(IEnumerable<TypeConstraintProfileGroupInfo> groupInfos) => groups.AddRange(groupInfos);

    public void FinalizeSystem()
    {
        System = Builder.FinalizeSystem();
    }

    public bool Equals(TypeConstraintProfileInfo profile)
    {
        return SymbolEqualityComparer.Default.Equals(ProfileDeclaringInterface, profile.ProfileDeclaringInterface);
    }
    public override bool Equals(object obj)
    {
        return obj is TypeConstraintProfileInfo group && Equals(group);
    }
    public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(ProfileDeclaringInterface);
}
