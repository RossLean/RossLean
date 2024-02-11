using System;
using System.Collections.Immutable;

namespace RossLean.GenericsAnalyzer.Core;

/// <summary>Denotes that the marked generic type parameter constraints may be inherited from type constraint profiles.</summary>
public sealed class InheritProfileTypeConstraintsAttribute : BaseInheritConstraintsAttribute
{
    /// <summary>The type constraint profile types.</summary>
    public ImmutableArray<Type> ProfileTypes { get; }

    /// <summary>Denotes that the marked generic type parameter constraints may be inherited from type constraint profiles.</summary>
    /// <param name="profileTypes">The type constraint profile types.</param>
    public InheritProfileTypeConstraintsAttribute(params Type[] profileTypes)
    {
        ProfileTypes = profileTypes.ToImmutableArray();
    }
}
