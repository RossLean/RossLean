using System.Collections.Immutable;

namespace RossLean.GenericsAnalyzer.Core;

/// <summary>Denotes that the marked generic type parameter constraints may be inherited from other type parameters declared in the declaring member.</summary>
public sealed class InheritTypeConstraintsAttribute : BaseInheritConstraintsAttribute
{
    public ImmutableArray<string> TypeParameterNames { get; }

    /// <summary>Denotes that the marked generic type parameter constraints may be inherited from other type parameters declared in the declaring member.</summary>
    /// <param name="typeParameterNames">
    /// The names of the type parameters whose type constraints are inherited.<br/>
    /// Prefer using <see langword="nameof"/>(...) for each inheriting type parameter. For functions, this solution is unavailable, therefore raw string literals will have to be used.
    /// </param>
    public InheritTypeConstraintsAttribute(params string[] typeParameterNames)
    {
        TypeParameterNames = typeParameterNames.ToImmutableArray();
    }
}
