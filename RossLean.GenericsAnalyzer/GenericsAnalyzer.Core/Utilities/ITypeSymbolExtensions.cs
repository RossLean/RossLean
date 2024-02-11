using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core.Utilities;

public static class ITypeSymbolExtensions
{
    public static bool IsNonProfileTypeConstraintAttribute(this ITypeSymbol attributeType)
    {
        return attributeType.IsGenericConstraintAttribute()
            && !attributeType.IsGenericConstraintAttribute<ITypeConstraintProfileRelatedAttribute>();
    }

    /// <summary>Determines whether the given attribute type is a generic constaint attribute that the analyzer should take into account.</summary>
    /// <param name="attributeType">The attribute type that will be evaluated.</param>
    /// <returns><see langword="true"/> if the given attribute type is a generic constraint attribute one that is important enough, otherwise <see langword="false"/>.</returns>
    public static bool IsGenericConstraintAttribute(this ITypeSymbol attributeType)
    {
        return attributeType.IsGenericConstraintAttribute<IGenericTypeConstraintAttribute>();
    }
    /// <summary>Determines whether the given attribute type is a generic constaint attribute that the analyzer should take into account.</summary>
    /// <typeparam name="T">The base type that the attribute type should inherit if it's an important one.</typeparam>
    /// <param name="attributeType">The attribute type that will be evaluated.</param>
    /// <returns><see langword="true"/> if the given attribute type is a generic constraint attribute one that is important enough, otherwise <see langword="false"/>.</returns>
    public static bool IsGenericConstraintAttribute<T>(this ITypeSymbol attributeType)
        where T : IGenericTypeConstraintAttribute
    {
        return attributeType.GetAllBaseTypesAndInterfaces().Any(t => t.Name == typeof(T).Name);
    }
}
