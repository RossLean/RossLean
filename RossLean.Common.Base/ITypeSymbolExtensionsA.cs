using Microsoft.CodeAnalysis;
using RoseLynn;
using System;

namespace RossLean.Common.Base;

public static class ITypeSymbolExtensionsA
{
    public static readonly FullSymbolName SystemAttributeFullName
        = new(nameof(System.Attribute), [nameof(System)]);

    /// <summary>
    /// Determines whether the given <see cref="ITypeSymbol"/> is an
    /// attribute type that effectively inherits <see cref="Attribute"/>.
    /// </summary>
    /// <param name="type">The given type.</param>
    /// <remarks>
    /// This method relies on <see cref="IsSystemAttribute(ITypeSymbol)"/>.
    /// </remarks>
    public static bool IsAttribute(this ITypeSymbol type)
    {
        var current = type;
        while (true)
        {
            if (current is null)
                return false;

            if (IsSystemAttribute(current))
                return true;

            current = current.BaseType;
        }
    }

    /// <summary>
    /// Determines whether the given <see cref="ITypeSymbol"/> is the well-known
    /// <see cref="Attribute"/> type.
    /// </summary>
    /// <param name="type">The given type.</param>
    public static bool IsSystemAttribute(this ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol named)
            return false;

        var fullName = named.GetFullSymbolName()!;
        return fullName.Matches(SystemAttributeFullName, SymbolNameMatchingLevel.Namespace);
    }
}
