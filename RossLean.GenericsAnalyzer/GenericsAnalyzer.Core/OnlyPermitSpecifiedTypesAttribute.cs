using System;

namespace RossLean.GenericsAnalyzer.Core;

/// <summary>Denotes that the marked generic type parameter may only be substituted by the specified permitted types, and no other.</summary>
[AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class OnlyPermitSpecifiedTypesAttribute : Attribute, IGenericTypeConstraintAttribute
{
}
