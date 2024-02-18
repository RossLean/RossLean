using RossLean.Common.Base;
using System;

namespace RossLean.GenericsAnalyzer.Core;

[Flags]
public enum FilterableTypeGroups
{
    None = 0,

    Class = 1,
    Struct = 1 << 1,
    Interface = 1 << 2,
    Enum = 1 << 3,
    Delegate = 1 << 4,

    RecordClass = 1 << 5,
    RecordStruct = 1 << 6,

    AbstractClass = 1 << 7,
    SealedClass = 1 << 8,

    Generic = 1 << 9,
    Array = 1 << 10,
}

public static class FilterableTypeGroupFacts
{
    private static readonly FilterableTypeGroups[] _validCombinationFlags =
    [
        FilterableTypeGroups.AbstractClass |
        FilterableTypeGroups.RecordClass |
        FilterableTypeGroups.Generic,

        FilterableTypeGroups.SealedClass |
        FilterableTypeGroups.RecordClass |
        FilterableTypeGroups.Generic,

        FilterableTypeGroups.Struct |
        FilterableTypeGroups.Generic,

        FilterableTypeGroups.Interface |
        FilterableTypeGroups.Generic,

        FilterableTypeGroups.Delegate |
        FilterableTypeGroups.Generic,
    ];

    public static bool IsValidCombination(FilterableTypeGroups groups)
    {
        int count = BitOperationsEx.DirtyPopCount((int)groups);
        if (count is 0 or 1)
            return true;

        if (count > 3)
            return false;

        foreach (var validCombinationFlags in _validCombinationFlags)
        {
            if (HasAnyFlags(groups, validCombinationFlags))
                return true;
        }

        return false;
    }

    private static bool HasAnyFlags(
        FilterableTypeGroups groups, FilterableTypeGroups desiredFlags)
    {
        var masked = groups & desiredFlags;
        return masked == groups;
    }
    private static bool HasAllFlags(
        FilterableTypeGroups groups, FilterableTypeGroups desiredFlags)
    {
        var masked = groups & desiredFlags;
        return masked == desiredFlags;
    }
}
