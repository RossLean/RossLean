# SMTTR0010

## Title
Provided argument count mismatch

## Category
API Restrictions

## Severity
Error

## Details
This error is emitted when an object array argument is required to have a length equal to the number of the parameters of the attributed method.

## Example
```csharp
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class SingleParameterTypeMatchAttribute : Attribute
{
    public SingleParameterTypeMatchAttribute(
        [ParameterTypeMatch] object?[] parameters)
    { }
}

public class MethodExampleClass
{
    [SingleParameterTypeMatch(
        new object[] { 1, "hello", false })]
    [SingleParameterTypeMatch(
        ↓new object[] { "hello", 1, false, null, "extra" })]
    public void MethodExample(int a, string b, bool c) { }
}
```
