using Microsoft.CodeAnalysis;
using System;

namespace RossLean.Common.Base;

#nullable enable

public static class TypedConstantExtensions
{
    public static T? ValueOrDefault<T>(
        this TypedConstant constant, T? defaultValue = default)
    {
        if (constant.Kind is TypedConstantKind.Array)
        {
            return defaultValue;
        }

        var value = constant.Value;
        if (value is T castValue)
            return castValue;

        return defaultValue;
    }

    public static unsafe TEnum EnumValueOrDefault<TEnum, TUnderlying>(
        this TypedConstant constant, TEnum defaultValue = default)
        where TUnderlying : unmanaged
        where TEnum : unmanaged, Enum
    {
        if (constant.Kind is not TypedConstantKind.Enum)
        {
            return defaultValue;
        }

        var value = constant.Value;
        if (value is TUnderlying castValue)
            return *(TEnum*)&castValue;

        return defaultValue;
    }
}
