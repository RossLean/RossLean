# SMTTR0005

## Title
The attribute can only be placed on functions of certain kinds

## Category
API Restrictions

## Severity
Error

## Details
This error is emitted when an attribute is applied to a function of a kind that is not permitted by the API design.

## Example
```csharp
public class Test
{
    [Custom1]
    // Custom4 is only permitted on anonymous methods and lambdas, so an error is thrown here
    [↓Custom4]
    public void Method()
    {
        // Custom1 is only applicable on normal methods, not local methods
        [↓Custom1]
        void Local()
        {

        }

        Func<int, int> lambda = x => x + 1;
        // Custom2 is only applicable to methods and local methods, not lambdas
        var lambda2 = [↓Custom2] (int x) => x + 1;

        var lambda3 =
            // Likewise, Custom1 not applicable on lambdas
            [Custom3, ↓Custom1]
            [Custom4]
            (int x) => x + 1;
    }
}

[RestrictFunctions(FunctionTargets.Method)]
public sealed class Custom1Attribute : Attribute { }

[RestrictFunctions(FunctionTargets.Method | FunctionTargets.LocalMethod)]
public sealed class Custom2Attribute : Attribute { }

[RestrictFunctions(FunctionTargets.All)]
public sealed class Custom3Attribute : Attribute { }

[RestrictFunctions(FunctionTargets.AnonymousMethod | FunctionTargets.Lambda)]
public sealed class Custom4Attribute : Attribute { }
```
