using Microsoft.CodeAnalysis;

namespace RossLean.GenericsAnalyzer.Core.Utilities;

public static class AttributeDataExtensions
{
    public static TypedConstant? GetNamedArgument(this AttributeData data, string name)
    {
        foreach (var (key, value) in data.NamedArguments)
        {
            if (key == name)
                return value;
        }

        return null;
    }
}
