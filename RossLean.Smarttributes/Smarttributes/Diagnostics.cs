using Microsoft.CodeAnalysis;
using RoseLynn;
using RoseLynn.CSharp;
using RossLean.Smarttributes.Constraints;

namespace RossLean.Smarttributes;

internal static class Diagnostics
{
    private static SmarttributesDiagnosticDescriptorStorage Storage
        => SmarttributesDiagnosticDescriptorStorage.Instance;

    #region Creators
    public static Diagnostic CreateSMTTR0001(
        ISymbol attributedSymbol,
        AttributeData sourceAttribute,
        IEnumerable<ITypeSymbol> missingAttributes)
    {
        var missingAttributeList = string.Join(
            "\n",
            missingAttributes.Select(m => m.ToDisplayString()));

        return Diagnostic.Create(
            Storage[0001]!,
            sourceAttribute.GetAttributeApplicationSyntax()?.GetLocation(),
            sourceAttribute.AttributeClass!.ToDisplayString(),
            attributedSymbol.ToDisplayString(),
            "\n" + missingAttributeList);
    }

    public static Diagnostic CreateSMTTR0005(
        AttributeData sourceAttribute,
        FunctionTargets functionTargets)
    {
        var validTargets = BuildFunctionTargetListString(functionTargets);
        return Diagnostic.Create(
            Storage[0005]!,
            sourceAttribute.GetAttributeApplicationSyntax()?.GetLocation(),
            sourceAttribute.AttributeClass!.ToDisplayString(),
            validTargets);
    }

    public static Diagnostic CreateSMTTR0010(
        AttributeData sourceAttribute,
        int parameterIndex)
    {
        var location = GetLocationOfAttributeConstructorArguments(sourceAttribute, parameterIndex);
        return Diagnostic.Create(Storage[0010]!, location);
    }

    public static Diagnostic CreateSMTTR0011(
        AttributeData sourceAttribute,
        int parameterIndex,
        int unmatchedParameterTypeIndex)
    {
        var location = GetLocationOfAttributeConstructorArrayArgument(
            sourceAttribute,
            parameterIndex,
            unmatchedParameterTypeIndex);
        return Diagnostic.Create(Storage[0011]!, location);
    }
    #endregion

    #region Location calculations
    private static Location? GetLocationOfAttributeConstructorArrayArgument(
        AttributeData attribute,
        int parameterIndex,
        int arrayArgumentIndex)
    {
        if (attribute.ConstructorArguments.Length <= parameterIndex)
        {
            return null;
        }

        var attributeSyntax = attribute.GetAttributeApplicationSyntax();

        var arguments = attributeSyntax?.ArgumentList;
        if (arguments is null)
            return null;

        if (parameterIndex == attribute.ConstructorArguments.Length - 1)
        {
            if (IsLastConstructorParameterParams(attribute))
            {
                var parameterLocation = arguments.Arguments[parameterIndex + arrayArgumentIndex].GetLocation();
                return parameterLocation;
            }
        }

        var argumentSyntax = arguments.Arguments[parameterIndex];
        var arrayInitializerExpression = argumentSyntax.Expression.GetArrayInitializerExpression();

        if (arrayInitializerExpression is null)
            return null;

        var expression = arrayInitializerExpression.Expressions[arrayArgumentIndex];
        return expression.GetLocation();
    }
    private static Location? GetLocationOfAttributeConstructorArguments(
        AttributeData attribute,
        int parameterIndex)
    {
        if (attribute.ConstructorArguments.Length <= parameterIndex)
        {
            return null;
        }

        var attributeSyntax = attribute.GetAttributeApplicationSyntax();

        var arguments = attributeSyntax?.ArgumentList;
        if (arguments is null)
            return null;

        var targetParameterLocation = arguments.Arguments[parameterIndex].GetLocation();
        if (parameterIndex == attribute.ConstructorArguments.Length - 1)
        {
            if (IsLastConstructorParameterParams(attribute))
            {
                var firstParameterLocation = targetParameterLocation;
                var lastParameterLocation = arguments.Arguments[arguments.Arguments.Count - 1].GetLocation();
                var locationStart = firstParameterLocation.SourceSpan.Start;
                var locationEnd = lastParameterLocation.SourceSpan.End;
                return LocationFactory.CreateFromNodeTreeSpanBounds(
                    attributeSyntax,
                    locationStart,
                    locationEnd);
            }
        }

        return targetParameterLocation;
    }
    private static bool IsLastConstructorParameterParams(AttributeData attribute)
    {
        // The arguments always matches the target constructor if valid,
        // otherwise ignore the invalid case and consider a constructor with 0 parameters
        if (attribute.ConstructorArguments.Length == 0)
            return false;

        var lastParameter = attribute.AttributeConstructor?.Parameters.LastOrDefault();
        if (lastParameter is null)
            return false;

        return lastParameter.IsParams;
    }
    #endregion

    #region Function targets
    private static string BuildFunctionTargetListString(FunctionTargets functionTargets)
    {
        var result = string.Empty;
        foreach (var functionTargetFlag in validFunctionTargets)
        {
            if (!functionTargets.HasFlag(functionTargetFlag))
                continue;

            if (result.Length > 0)
                result += ", ";

            result += GetFunctionTargetStringRepresentation(functionTargetFlag);
        }
        return result;
    }

    private static readonly FunctionTargets[] validFunctionTargets =
    [
        FunctionTargets.Method,
        FunctionTargets.LocalMethod,
        FunctionTargets.Lambda,
        FunctionTargets.AnonymousMethod,
    ];

    private static string GetFunctionTargetStringRepresentation(FunctionTargets functionTarget)
    {
        return functionTarget switch
        {
            FunctionTargets.Method => "method",
            FunctionTargets.LocalMethod => "local method",
            FunctionTargets.Lambda => "lambda",
            FunctionTargets.AnonymousMethod => "anonymous method",
            _ => "invalid target"
        };
    }
    #endregion
}
