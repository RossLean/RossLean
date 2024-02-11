using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace RossLean.GenericsAnalyzer.Core;

using TemplateType = TypeConstraintTemplateType;

public class TypeConstraintTemplateAttributeStorage
{
    public AttributeData ProfileAttribute { get; set; }
    public AttributeData ProfileGroupAttribute { get; set; }

    public AttributeData ProfileRelatedAttribute
    {
        get
        {
            if (ProfileAttribute is not null && ProfileGroupAttribute is not null)
                throw new InvalidOperationException("There should only be one profile-related attribute.");

            return ProfileAttribute ?? ProfileGroupAttribute;
        }
    }

    public IEnumerable<AttributeData> GetAllAssociatedAttributes() => GetAssociatedAttributes(TemplateType.All);
    public IEnumerable<AttributeData> GetAssociatedAttributes(TemplateType templateTypes)
    {
        // Enums deserve much more love
        for (int mask = 1; mask < (int)TemplateType.All; mask <<= 1)
        {
            if (((int)templateTypes & mask) == default)
                continue;

            var associatedAttribute = GetAssociatedProperty((TemplateType)mask);
            if (associatedAttribute is not null)
                yield return associatedAttribute;
        }
        yield break;
    }

    private AttributeData GetAssociatedProperty(TemplateType templateType)
    {
        return templateType switch
        {
            TemplateType.Profile => ProfileAttribute,
            TemplateType.ProfileGroup => ProfileGroupAttribute,
            _ => null,
        };
    }
}
