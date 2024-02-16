using System;

namespace RossLean.GenericsAnalyzer.Core;

/// <summary>
/// Denotes that the marked generic type parameter may only be substituted
/// by the specified permitted types, and no other.
/// </summary>
[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class OnlyPermitSpecifiedTypesAttribute
    : Attribute, IGenericTypeConstraintAttribute;

/// <summary>
/// Denotes that the marked generic type parameter may only be substituted
/// by the specified permitted type groups, and no other. Further filtering of
/// the permitted types is allowed with prohibition of specified type groups
/// and types.
/// </summary>
/// <remarks>
/// This attribute may <b>not</b> be combined with
/// <see cref="OnlyPermitSpecifiedTypesAttribute"/>.
/// </remarks>
[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class OnlyPermitSpecifiedTypeGroupsAttribute
    : Attribute, IGenericTypeConstraintAttribute;
