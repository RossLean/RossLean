using Garyon.Reflection;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System;

namespace RossLean.NameOn.Core;

/// <summary>The base attribute class for <see langword="nameof"/>-related restrictions applicable to <see langword="string"/>-typed symbols.</summary>
[AttributeUsage(AttributeTargeting.NameOfTargets, Inherited = true, AllowMultiple = false)]
public abstract class NameOfRestrictionAttributeBase : Attribute, INameOfRelatedAttribute
{
    private static readonly InstanceContainer instanceContainer = new();

    private sealed class InstanceContainer : DefaultInstanceContainer<NameOfRestrictionAttributeBase>
    {
        protected override object[] GetDefaultInstanceArguments()
        {
            return [IdentifiableSymbolKind.All];
        }
    }

    /// <summary>Determines the restriction type of <see langword="nameof"/> expressions this attribute denotes.</summary>
    public abstract NameOfRestrictions Restrictions { get; }

    /// <summary>Determines the symbol kinds that the restriction is applied for.</summary>
    public IdentifiableSymbolKind AffectedSymbolKinds { get; }

    protected NameOfRestrictionAttributeBase()
        : this(IdentifiableSymbolKind.All) { }
    protected NameOfRestrictionAttributeBase(IdentifiableSymbolKind affectedSymbolKinds)
    {
        AffectedSymbolKinds = affectedSymbolKinds;
    }

    public static NameOfRestrictions? GetRestrictions<T>()
        where T : NameOfRestrictionAttributeBase
    {
        return instanceContainer.GetDefaultInstance<T>()?.Restrictions;
    }
    /// <summary>Gets the constraint rule that the attribute with the given attribute name reflects.</summary>
    /// <param name="attributeTypeName">The name of the attribute whose constraint rule to get.</param>
    /// <returns>The <seealso cref="NameOfRestrictions"/> that is reflected from the attribute with the given name.</returns>
    public static NameOfRestrictions? GetRestrictionsFromAttributeName(string attributeTypeName)
    {
        return instanceContainer.GetDefaultInstance(attributeTypeName)?.Restrictions;
    }
    public static NameOfRestrictions? GetRestrictions(Type type)
    {
        return instanceContainer.GetDefaultInstance(type)?.Restrictions;
    }

    public static IdentifiableSymbolKind GetConstructorRestrictions(AttributeData attribute)
    {
        var args = attribute.ConstructorArguments;
        if (args.IsEmpty)
            return IdentifiableSymbolKind.All;

        return (IdentifiableSymbolKind)attribute.ConstructorArguments[0].Value;
    }
}
