using Garyon.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;
#nullable enable

public sealed class SpecializableTypeGroupFilterIdentifierCollection<T>(
    T? defaultCase,
    IReadOnlyCollection<T> specializedCases)
    where T : ISpecializableTypeGroupFilterIdentifier
{
    public T? DefaultCase { get; set; } = defaultCase;
    public IReadOnlyCollection<T> SpecializedCases { get; } = specializedCases;

    public static SpecializableTypeGroupFilterIdentifierCollection<T> FromIdentifiers(
        IEnumerable<T> identifiers)
    {
        var list = identifiers.ToList();
        var defaultCase = list.FirstOrDefault(static s => !s.IsSpecialized);
        if (defaultCase is not null)
        {
            list.Remove(defaultCase);
        }

        return new(defaultCase, list);
    }
}
