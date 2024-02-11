using System;

namespace RossLean.GenericsAnalyzer.Core;

/// <summary>Denotes that the marked interface represents a type constraint profile group.</summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public sealed class TypeConstraintProfileGroupAttribute : Attribute, ITypeConstraintProfileRelatedAttribute
{
    /// <summary>Determines whether the profile group is a distinct one. Defaults to <see langword="true"/>.</summary>
    public bool Distinct { get; set; }

    /// <summary>Initializes a new instance of the <seealso cref="TypeConstraintProfileGroupAttribute"/> class.</summary>
    /// <param name="distinct">Determines whether the profile group is a distinct one. Defaults to <see langword="true"/>.</param>
    public TypeConstraintProfileGroupAttribute(bool distinct = true)
    {
        Distinct = distinct;
    }
}
