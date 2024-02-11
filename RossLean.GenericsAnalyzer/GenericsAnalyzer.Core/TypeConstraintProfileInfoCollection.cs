using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;

public class TypeConstraintProfileInfoCollection
{
    private readonly Dictionary<INamedTypeSymbol, TypeConstraintProfileInfo> profiles = new(SymbolEqualityComparer.Default);
    private readonly Dictionary<INamedTypeSymbol, TypeConstraintProfileGroupInfo> groups = new(SymbolEqualityComparer.Default);

    public IEnumerable<TypeConstraintProfileInfo> Profiles => profiles.Values;
    public IEnumerable<TypeConstraintProfileGroupInfo> Groups => groups.Values;

    public void AddProfile(INamedTypeSymbol profileDeclarationType, IEnumerable<INamedTypeSymbol> groupTypes)
    {
        profiles.Add(profileDeclarationType, new TypeConstraintProfileInfo(profileDeclarationType, groupTypes.Select(group => groups[group])));
    }
    public void AddGroup(INamedTypeSymbol groupDeclarationType, bool distinct)
    {
        groups.Add(groupDeclarationType, new TypeConstraintProfileGroupInfo(groupDeclarationType, distinct));
    }

    public bool ContainsProfile(INamedTypeSymbol profileDeclarationType) => profiles.ContainsKey(profileDeclarationType);
    public bool ContainsGroup(INamedTypeSymbol groupDeclarationType) => groups.ContainsKey(groupDeclarationType);

    public bool ContainsDeclaringType(INamedTypeSymbol declarationType) => ContainsProfile(declarationType) || ContainsGroup(declarationType);

    public TypeConstraintProfileInfo GetProfileInfo(INamedTypeSymbol profileDeclarationType) => profiles[profileDeclarationType];
    public TypeConstraintProfileGroupInfo GetGroupInfo(INamedTypeSymbol groupDeclarationType) => groups[groupDeclarationType];
}
