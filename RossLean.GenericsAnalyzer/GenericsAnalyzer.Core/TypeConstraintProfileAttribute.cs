using System;
using System.Collections.Immutable;

namespace RossLean.GenericsAnalyzer.Core;

/// <summary>Denotes that the marked interface represents a type constraint profile.</summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public sealed class TypeConstraintProfileAttribute : Attribute, ITypeConstraintProfileRelatedAttribute
{
    /// <summary>The type constraint profile group types.</summary>
    public ImmutableArray<Type> ProfileGroupTypes { get; }

    /// <summary>Denotes that the marked interface represents a type constraint profile.</summary>
    /// <param name="profileGroups">The type constraint profile group types.</param>
    public TypeConstraintProfileAttribute(params Type[] profileGroups)
    {
        ProfileGroupTypes = profileGroups.ToImmutableArray();
    }
}
