This analyzer currently provides the ability to restrict a generic type parameter's substituted types through the provided type constraint attributes. They must be directly specified at the generic type parameter.

---

## Prohibiting Specific Types
Consider for example the following class:
```csharp
public class C<T>
    where T : IComparable<T>
{
    // code
}
```
If the type T should never be substituted by `ulong`, the following adjustment must be made:
```csharp
public class C
<
    [ProhibitedTypes(typeof(ulong))]
    T
>
```
On the following code snippet, the analyzer will emit [GA0001](rules/GA0001.md) on the `ulong` word:
```csharp
new C<ulong>();
```

### Notes
It is restricted by the language to include a generic type in the `typeof` expression. This means that attempting to restrict types that contain it will not compile. The following example would result in an error:
```csharp
public class C
<
    [ProhibitedTypes(typeof(IComparable<T>))]
    T
>
```

---

## Prohibiting Base Types
There also is the ability to prohibit any type that inherits the specified prohibited base types. In a similar example, consider the following class:
```csharp
public class C<T, U>
    where T : IEnumerable<U>
{
    // code
}
```
If the type T should never inherit an `IList<>` (of any type because of the aforementioned language restriction), the following must be done:
```csharp
public class C
<
    [ProhibitedBaseTypes(typeof(IList<>))]
    T,
    U
>
```
On the following code snippet, the analyzer will emit [GA0001](rules/GA0001.md) on the `List<int>` word because `List<int>` implements `IList<int>`, which is prohibited from the analyzer's type constraints:
```csharp
new C<List<int>, int>();
```

---

## Permitting Types
Prohibitions aside, some types may be excepted from the rules by permitting them to be used. In the previous example:
```csharp
public class C
<
    [ProhibitedBaseTypes(typeof(IList<>))]
    T,
    U
>
```
If `IList<int>` can be allowed as a type argument for T, the following modification has to be made:
```csharp
public class C
<
    [PermittedBaseTypes(typeof(IList<int>))]
    [ProhibitedBaseTypes(typeof(IList<>))]
    T,
    U
>
```
As a result, the example code snippet will not emit any errors:
```csharp
new C<List<int>, int>();
```

---

## Only Permitting Specified Types
On a generic element that only supports a collection of types, there is no need to explicitly prohibit the unsupported types. For example,
```csharp
public T AddSigned
<
    [PermittedTypes(typeof(sbyte), typeof(short), typeof(int), typeof(long))]
    [OnlyPermitSpecifiedTypes]
    T
>
(T left, T right)
    where T : IComparable<T>
{
    // code
}
```
In the above function, only the types `sbyte`, `short`, `int` and `long` are supported. Using any other type would emit an error, as in the following example:
```csharp
AddSigned<ulong>(45UL, 76UL);
```
