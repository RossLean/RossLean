# Type Group Filters

## Introduction

Type group filters is a feature intended to filter the set of types that are valid for substitution of a type parameter. In other words, this restricts or permits certain types based on their type group.

Type groups can be formed either as classification type groups, as shown below, or by defining custom type groups.

## Limitations

The feature is currently limited in certain scopes. Here is a compact list of the limitations:

- It is not possible to inherit the filters of another type parameter. This renders the inheritance feature unusable in combination with filters. The filters must be declared manually on the new type.
- It is not possible to inherit the filters via type constraint profiles. Despite being able to declare the filters in type constraint profiles, they are not inheritable when the profile is applied to a type parameter.
- It is not possible to customize the behavior of the interaction of type group filters with regular type constraints.
- Filtering ranges of arity and rank is not supported. Each distinct value must be explicitly filtered manually.

The above limitations will be lifted at later versions.

## Classification Type Groups

Classification type groups are natively supported and defined by the analyzer. They classify types based on certain properties of their declaration. Below is a list with the currently available classification type groups:

- **Interfaces**: Interface types (declared with `interface`).
- **Delegates**: Delegate types (declared with `delegate`). This **excludes** `Delegate` and `MulticastDelegate`.
- **Enums**: Enum types (declared with `enum`). This **excludes** `Enum`.
- **Abstract classes**: Abstract class types (declared with the `abstract` modifier). This also includes record classes.
- **Sealed classes**: Abstract class types (declared with the `sealed` modifier). This also includes record classes.
- **Record classes**: Record class types (declared with `record` or `record class`). Unlike record structs, types from other assemblies can be recognized as record classes.
- **Record structs**: Record struct types (declared with `record struct`). Note that only types within the same assembly are recognized as record structs, as there is no marker encoded in the metadata to denote the `record` feature.
- **Generic types**: Generic types (types with type parameters). This **excludes** non-generic types and arrays.
- **Arrays**: Array types. This **excludes** `Array`.

### Specialization

Some of the above type groups can be specialized to certain constraints. This includes:

- **Generic types**: Specializable with regards to the arity of the generic type, which is equal to the number of type parameters it has.
- **Arrays**: Specializable with regards to the rank of the generic type, which is equal to the dimensions of the array.

These specializable type groups can be used with or without a specialization to a certain value. The filtering of a type will depend on whether there exists a specialization filter for the specific case, where otherwise the unspecialized filter will be applied. More details and examples are shown below.

## Custom Type Groups

Custom type groups are not yet implemented.

The idea of custom type groups is to declare type groups based on certain properties, or explicitly adding or removing types in the same way types are permitted or prohibited directly in type constraint profiles and type parameters. The API design and the usage are not yet concrete.

Type constraint profiles already cover a large portion of this feature. The expansion to allowing custom type groups might not be necessary.

## Filtering

By default, all type groups are permitted. This is adjustable with the `OnlyPermitSpecifiedTypeGroups` attribute, where all type groups are prohibited, unless explicitly permitted.

There is also the option to filter using the `FilterType.Exclusive` filter type. This only permits types that are contained in **all** the exclusively filtered classification type groups, and prohibits **all** others. `OnlyPermitSpecifiedTypeGroups` is ineffective in exclusive filtering. Non-exclusive filters are not allowed when at least one exclusive filter is present.

Specialization filters override the unspecialized filter for the same type group. For example, given the below constraints:
```csharp
[FilterGenericTypes(FilterType.Prohibited)]
[FilterGenericTypes(FilterType.Permitted, 1)]
```

`A<T>` is permitted because it's a generic type of arity 1, but `A<T, U>` is prohibited because its arity is 2, which is handled by the unspecialized filter for generic types.

Specialized exclusive filters always render their unspecialized counterparts ineffective.

Consult the set theory chapter below for exact details on how the filters interact with each other.

### In set theory

Let the type set of all permitted types be S, and the largest possible set that S can be as M. Constraints applied in the `where` clause of a generic symbol are ignored here, since violations are handled by the compiler.

When not using `OnlyPermitSpecifiedTypeGroups`, the type set S is initially set to M. When using the attribute, the type set S is set to the empty set.

First, all **unspecialized** `FilterType.Permitted` filters are applied. Each such filter on a type group G expands the type set S to (S union G). This has no effect without `OnlyPermitSpecifiedTypeGroups`, as the set is already the largest possible.

Then, all **unspecialized** `FilterType.Prohibited` filters are applied. Each such filter prohibits **all** types in the type group G, reducing the permitted type set S to (S - G).

When **specialized** filters are applied, the above order is followed, by first applying the specialized permission filters, and then the specialized prohibition filters.

When any `FilterType.Exclusive` filters are applied, all of the above are ignored. The initial type set is set to all the available types. Each exclusive filter on a type group G reduces the permitted type set S to (S intersection G).

For example, let:
- The type groups A, B, C
- The types
  - TA that belongs only in the type group A
  - TB that belongs only in the type group B
  - TAB that belongs only in the type groups A and B
  - TC that belongs only in the type group C

where:
- intersection of A and B contains valid types
- intersection of A and C does not contain valid types
- intersection of B and C does not contain valid types

The following table shows examples of the behavior:

| Constraints | Permitted type groups | Permitted types |
|-------------|-------------|-------------|
| A : Prohibited | B union C | TB, TC |
| B : Permitted<br/>C : Permitted<br/>`OnlyPermitSpecifiedTypeGroups` | B union C | TB, TAB, TC |
| B : Exclusive | B | TB, TAB |
| A : Exclusive<br/>B : Exclusive | A intersection B | TAB |

### Valid exclusive combinations
[exclusive-combinations]: #exclusive-combinations

## Combinations with other constraints

The default behavior of the system applies the filters only on types that are not exactly specified in the type constraints of permitting/prohibiting types. This means that types that are permitted/prohibited by the following attributes, will **not** be affected by type group filters:
- PermittedTypes
- PermittedBaseTypes
- ProhibitedTypes
- ProhibitedBaseTypes

Types that construct an explicitly filtered open generic type (like `IEnumerable<>`) are also **not** affected by type group filters.

The `OnlyPermitSpecifiedTypeGroups` attribute does not affect the rules above.

This behavior is not yet customizable. Customization will be implemented in a future version.

### Example

Assume the following constraints:
```csharp
[FilterDelegates(FilterType.Prohibited)]
[FilterInterfaces(FilterType.Prohibited)]
[PermittedTypes(typeof(Action))]
[PermittedBaseTypes(typeof(IReadOnlyList<>))]
[ProhibitedBaseTypes(typeof(IEnumerable<>))]
```

And the following types:
```csharp
interface IReadOnlyList2<T> : IReadOnlyList<T>;
```

This only enables the usage of types that are **not** delegates and **not** interfaces, **except** for 'Action' and any that inherits and constructs 'IReadOnlyList<>'
Therefore:
- `IEnumerable<char>` - Prohibited
- `IReadOnlyList<char>` - Permitted
- `IReadOnlyList2<char>` - Prohibited (not exactly `IReadOnlyList<>`, interfaes are otherwise prohibited)
- `Action` - Permitted
- `Action<int>` - Prohibited
- `string` - Prohibited (via `IEnumerable<>`)
- `ImmutableArray<int>` - Permitted (via `IReadOnlyList<>`)
