using System;

namespace RossLean.GenericsAnalyzer.Core;

[Flags]
public enum TypeConstraintTemplateType
{
    None,

    // Profile-related
    Profile = 1,
    ProfileGroup = 1 << 1,
    ProfileRelated = Profile | ProfileGroup,

    // All
    All = ProfileRelated,
}
