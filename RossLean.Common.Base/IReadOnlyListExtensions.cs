using System.Collections.Generic;

namespace RossLean.Common.Base;

#nullable enable

public static class IReadOnlyListExtensions
{
    public static bool TryGetSingle<T>(this IReadOnlyList<T> source, out T? value)
    {
        if (source.Count is 1)
        {
            value = source[0];
            return true;
        }

        value = default;
        return false;
    }

    public static bool TryGetAtIndex<T>(
        this IReadOnlyList<T> source,
        int index,
        out T? value)
    {
        if (source.Count > index)
        {
            value = source[index];
            return true;
        }

        value = default;
        return false;
    }

    public static T? AtIndexOrDefault<T>(this IReadOnlyList<T> source, int index)
    {
        TryGetAtIndex(source, index, out var value);
        return value;
    }
}
