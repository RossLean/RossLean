# SMTTR0001

## Title
Unmet attribute requirements from placed attribute

## Category
API Restrictions

## Severity
Error

## Details
This error is emitted when an attribute is applied to a symbol that does not have other required attributes, as denoted from the placed attribute's attribute requirements.

## Example
```csharp
[RequiresPresence<AttributeUsageAttribute>]
public sealed class ExampleAttribute : Attribute { }

// There should be an AttributeUsageAttribute on CustomAttribute
[↓Example] // SMTTR0001 will be emitted here
public sealed class CustomAttribute : Attribute { }
```
