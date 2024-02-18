using System;

namespace RossLean.GenericsAnalyzer.Core;

public abstract class BaseTypeGroupFilterAttribute(FilterType filterType)
    : Attribute, IGenericTypeConstraintAttribute
{
    public FilterType FilterType { get; } = filterType;
}

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class FilterInterfacesAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType);

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class FilterDelegatesAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType);

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class FilterEnumsAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType);

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class FilterAbstractClassesAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType);

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class FilterSealedClassesAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType);

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class FilterRecordClassesAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType);

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = false)]
public sealed class FilterRecordStructsAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType);

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = true)]
public sealed class FilterGenericTypesAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType)
{
    public uint? Arity { get; }

    public FilterGenericTypesAttribute(FilterType filterType, uint arity)
        : this(filterType)
    {
        Arity = arity;
    }
}

[AttributeUsage(CommonAttributeTargets.TypeParametersAndProfiles, AllowMultiple = true)]
public sealed class FilterArrayTypesAttribute(FilterType filterType)
    : BaseTypeGroupFilterAttribute(filterType)
{
    public uint? Rank { get; }

    public FilterArrayTypesAttribute(FilterType filterType, uint rank)
        : this(filterType)
    {
        Rank = rank;
    }
}
