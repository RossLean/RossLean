namespace RossLean.GenericsAnalyzer.Core;

public interface ITypeGroupFilterIdentifier
{
    public FilterableTypeGroups TypeGroup { get; }
}

public interface ISpecializableTypeGroupFilterIdentifier
    : ITypeGroupFilterIdentifier
{
    public bool IsSpecialized { get; }
}

public record class BasicTypeGroupFilter(FilterableTypeGroups TypeGroup)
    : ITypeGroupFilterIdentifier
{
    public static readonly BasicTypeGroupFilter
        Interface = new(FilterableTypeGroups.Interface),
        Delegate = new(FilterableTypeGroups.Delegate),
        Enum = new(FilterableTypeGroups.Enum),
        AbstractClass = new(FilterableTypeGroups.AbstractClass),
        SealedClass = new(FilterableTypeGroups.SealedClass),
        RecordClass = new(FilterableTypeGroups.RecordClass),
        RecordStruct = new(FilterableTypeGroups.RecordStruct)
        ;
}

public sealed record class GenericTypeGroupFilter(uint? Arity)
    : BasicTypeGroupFilter(FilterableTypeGroups.Generic),
        ISpecializableTypeGroupFilterIdentifier
{
    public static readonly GenericTypeGroupFilter
        Default = new(null as uint?);

    bool ISpecializableTypeGroupFilterIdentifier.IsSpecialized
        => Arity is not null;

    public static GenericTypeGroupFilter DefaultOrSpecialized(uint? arity)
    {
        if (arity is null)
            return Default;

        return new(arity);
    }
}

public sealed record class ArrayTypeGroupFilter(uint? Rank)
    : BasicTypeGroupFilter(FilterableTypeGroups.Array),
        ISpecializableTypeGroupFilterIdentifier
{
    public static readonly ArrayTypeGroupFilter
        Default = new(null as uint?);

    bool ISpecializableTypeGroupFilterIdentifier.IsSpecialized
        => Rank is not null;

    public static ArrayTypeGroupFilter DefaultOrSpecialized(uint? rank)
    {
        if (rank is null)
            return Default;

        return new(rank);
    }
}
