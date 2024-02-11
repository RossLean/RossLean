using Microsoft.CodeAnalysis;

namespace RossLean.Smarttributes;

public static class TypedConstantExtensions
{
    public static T? ValueOrDefault<T>(this TypedConstant constant, T? defaultValue = default)
    {
        var value = constant.Value;
        if (value is T castValue)
            return castValue;

        return defaultValue;
    }
}
