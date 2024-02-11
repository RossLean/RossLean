using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RossLean.NameOn;

public class SymbolDictionary<TValue>
{
    private readonly Dictionary<ISymbol, TValue> dictionary = new(SymbolEqualityComparer.Default);

    public void Register(ISymbol symbol, TValue restrictionType) => dictionary.TryAddPreserve(symbol, restrictionType);

    public bool ContainsKey(ISymbol symbol) => dictionary.ContainsKey(symbol);

    public TValue this[ISymbol symbol] => dictionary.ValueOrDefault(symbol);
}
