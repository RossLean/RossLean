using Garyon.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;

#nullable enable

public sealed class SpecializableTypeGroupFilterInstanceCollection(
    TypeGroupFilterInstance? defaultCase,
    IReadOnlyCollection<TypeGroupFilterInstance> specializedCases)
{
    public TypeGroupFilterInstance? DefaultCase { get; set; } = defaultCase;
    public IReadOnlyCollection<TypeGroupFilterInstance> SpecializedCases { get; }
        = specializedCases;

    public IEnumerable<TypeGroupFilterInstance> EnumerateAll()
    {
        if (DefaultCase is not null)
            yield return DefaultCase;

        foreach (var @case in SpecializedCases)
        {
            yield return @case;
        }
    }

    public static SpecializableTypeGroupFilterInstanceCollection FromInstances(
        IEnumerable<TypeGroupFilterInstance> instances)
    {
        var list = instances.ToList();
        var defaultCaseIndex = list.FindIndex(static s =>
        {
            return s.Identifier
                is ISpecializableTypeGroupFilterIdentifier { IsSpecialized: false };
        });

        var defaultCase = null as TypeGroupFilterInstance;

        if (defaultCaseIndex >= 0)
        {
            defaultCase = list[defaultCaseIndex];
            list.RemoveAt(defaultCaseIndex);
        }

        return new(defaultCase, list);
    }
}
