This analyzer provides the ability to specify rules regarding usage of `nameof` in values assigned to symbols, including fields, properties, function parameters, and more.

---

## Enforcing usage of nameof in the value

Consider for example the following class:
```csharp
public class C
{
    [ForceNameOf]
    public string SomeName { get; set; }
}
```

The `ForceNameOf` attribute denotes that when setting the value of `SomeName`, it must be a `nameof` expression.

Here are some examples of setting the value:
```csharp
var c = new C();
c.SomeName = nameof(Function); // valid
c.SomeName = "A"; // error
c.SomeName = nameof(Function) + nameof(C); // error
```

The last line is an error because the entire expression assigned to the symbol must be a `nameof` expression, not just consist of such. However, it can be allowed by using another rule, explained below.

## Enforcing containing at least one nameof expression in the value

Assume that the `SomeName` property from the above example now bears a `ForceContainedNameOf` attribute. In that case, the example's results are the following:

```csharp
var c = new C();
c.SomeName = nameof(Function); // still valid
c.SomeName = "A"; // still error
c.SomeName = nameof(Function) + nameof(C); // now valid
```

## Prohibiting usage of nameof in the value

Like enforcing presence of `nameof` expressions in the value, there also is the ability to prohibit them. Similarly, there are the attributes `ProhibitNameOf` and `ProhibitContainedNameOf`.

### ProhibitNameOf

```csharp
var c = new C();
c.SomeName = nameof(Function); // error
c.SomeName = "A"; // valid
c.SomeName = nameof(Function) + nameof(C); // valid
```

The last example shows that the concatenation of the two `nameof` expressions is still valid, despite prohibiting their usage. This is because the rule follows the same pattern as its enforcement equivalent; only caring about the whole expression being a `nameof` expression.

### ProhibitContainedNameOf

```csharp
var c = new C();
c.SomeName = nameof(Function); // still error
c.SomeName = "A"; // still valid
c.SomeName = nameof(Function) + nameof(C); // now error
```

To nobody's surprise, `ProhibitContainedNameOf` prohibits usage of `nameof` in the whole experssion entirely.

## Where can these attributes be applied?

In every symbol that can be passed a string value. This includes:
- fields
- properties
- function (or constructor, or delegate) parameters
- return values in functions or delegates