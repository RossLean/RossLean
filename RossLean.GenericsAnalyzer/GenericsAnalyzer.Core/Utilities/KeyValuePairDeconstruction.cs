using System.Collections.Generic;

namespace RossLean.GenericsAnalyzer.Core.Utilities;

public static class KeyValuePairDeconstruction
{
    public static void Deconstruct<TKey, TValue>(
        this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
    {
        key = kvp.Key;
        value = kvp.Value;
    }
}
