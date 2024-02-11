# SMTTR0011

## Title
Provided argument type mismatch

## Category
API Restrictions

## Severity
Error

## Details
This error is emitted when an object array argument is required to be assignable to the parameters of the attributed method in the declared order.

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
        new object[] { 1, "hello", ↓"false" })]
    public void MethodExample(int a, string b, bool c) { }
}
```
