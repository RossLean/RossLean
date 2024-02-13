using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RossLean.GenericsAnalyzer.Core.Utilities;

public static class AttributeSyntaxExtensions
{
    /// <summary>Determines whether the given attribute is a generic constraint attribute that the analyzer should take into account.</summary>
    /// <param name="attribute">The attribute that will be evaluated.</param>
    /// <param name="semanticModel">The semantic model that contains the information for the attribute's type.</param>
    /// <returns><see langword="true"/> if the given attribute is a generic constraint attribute one that is important enough, otherwise <see langword="false"/>.</returns>
    public static bool IsGenericConstraintAttribute(this AttributeSyntax attribute, SemanticModel semanticModel)
    {
        return attribute.IsGenericConstraintAttribute<IGenericTypeConstraintAttribute>(semanticModel);
    }
    /// <summary>Determines whether the given attribute is a generic constraint attribute that the analyzer should take into account.</summary>
    /// <typeparam name="T">The base type that the attribute should inherit if it's an important one.</typeparam>
    /// <param name="attribute">The attribute that will be evaluated.</param>
    /// <param name="semanticModel">The semantic model that contains the information for the attribute's type.</param>
    /// <returns><see langword="true"/> if the given attribute is a generic constraint attribute one that is important enough, otherwise <see langword="false"/>.</returns>
    public static bool IsGenericConstraintAttribute<T>(this AttributeSyntax attribute, SemanticModel semanticModel)
        where T : IGenericTypeConstraintAttribute
    {
        return semanticModel.GetTypeInfo(attribute).Type.IsGenericConstraintAttribute<T>();
    }
}
