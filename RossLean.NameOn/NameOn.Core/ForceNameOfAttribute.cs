using RoseLynn;
using System;

namespace RossLean.NameOn.Core;

/// <summary>The base attribute class for <see langword="nameof"/>-related restrictions applicable to <see langword="string"/>-typed symbols.</summary>
[AttributeUsage(AttributeTargeting.NameOfTargets, Inherited = true, AllowMultiple = false)]
public sealed class ForceNameOfAttribute : NameOfRestrictionAttributeBase
{
    public override NameOfRestrictions Restrictions => NameOfRestrictions.Force;

    public ForceNameOfAttribute()
        : base() { }
    public ForceNameOfAttribute(IdentifiableSymbolKind affectedSymbolKinds)
        : base(affectedSymbolKinds) { }
}
