using System;

namespace RossLean.NameOn.Core;

public static class AttributeTargeting
{
    public const AttributeTargets NameOfTargets = AttributeTargets.Parameter
                                                | AttributeTargets.Field
                                                | AttributeTargets.Property
                                                | AttributeTargets.ReturnValue;
}
