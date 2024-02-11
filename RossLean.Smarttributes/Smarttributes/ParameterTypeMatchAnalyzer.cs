using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using RoseLynn.CSharp.Syntax;
using RossLean.Smarttributes.Constraints;
using Smarttributes;

namespace RossLean.Smarttributes;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ParameterTypeMatchAnalyzer : BaseAttributeAnalyzer
{
    protected override void AnalyzeAttributedSymbol(AttributeSyntaxNodeAnalysisContext attributeContext)
    {
        var (context, attributeNode, _, declaredSymbol) = attributeContext;

        if (declaredSymbol is not IMethodSymbol declaredMethod)
            return;

        var declaredSymbolAttribute = attributeNode.GetAttributeData(context.SemanticModel);

        var declaredSymbolAttributeClass = declaredSymbolAttribute?.AttributeClass;
        if (declaredSymbolAttributeClass is null)
            return;

        // False CS8602 when removing the `!`
        var attributeConstructor = declaredSymbolAttribute!.AttributeConstructor;
        var comparedAttributeParameter = attributeConstructor?.Parameters
            .FirstOrDefault(f => f.HasAttributeNamedFully<ParameterTypeMatchAttribute>());

        if (comparedAttributeParameter is null)
            return;

        int comparedAttributeOrdinal = comparedAttributeParameter.Ordinal;
        // Ensure the argument is provided
        if (comparedAttributeOrdinal >= declaredSymbolAttribute.ConstructorArguments.Length)
            return;

        var comparedAttributeArgument = declaredSymbolAttribute.ConstructorArguments[comparedAttributeOrdinal];
        if (comparedAttributeArgument.Kind != TypedConstantKind.Array)
            return;

        var comparedValues = comparedAttributeArgument.Values;

        var parameters = declaredMethod.Parameters;

        int comparedLength = Math.Min(comparedValues.Length, parameters.Length);
        if (comparedValues.Length != parameters.Length)
        {
            var diagnostic = Diagnostics.CreateSMTTR0010(
                declaredSymbolAttribute!,
                comparedAttributeOrdinal);
            context.ReportDiagnostic(diagnostic);
        }

        for (int i = 0; i < comparedLength; i++)
        {
            var comparedValue = comparedValues[i];
            var parameter = parameters[i];

            var comparedValueType = comparedValue.Type;
            if (comparedValue.IsNull)
            {
                comparedValueType = NullTypeSymbol.Instance;
            }

            if (!IsAssignableTo(comparedValueType, parameter.Type))
            {
                var diagnostic = Diagnostics.CreateSMTTR0011(
                    declaredSymbolAttribute!,
                    comparedAttributeOrdinal,
                    i);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static bool IsAssignableTo(ITypeSymbol? argumentType, ITypeSymbol parameterType)
    {
        // Exempt error types from reporting errors
        if (argumentType is null)
            return true;

        // Custom implementation to distinct null
        if (argumentType is INullTypeSymbol)
            return AcceptsNullRuntimeValue(parameterType);

        if (SymbolEqualityComparer.Default.Equals(argumentType, parameterType))
            return true;

        if (parameterType is INamedTypeSymbol namedParameterType)
        {
            if (argumentType.Inherits(namedParameterType))
                return true;
        }

        if (parameterType is ITypeParameterSymbol)
        {
            // Do not introduce rules about type parameters
            return true;
        }

        // Edge cases are not being covered here
        return false;
    }

    private static bool AcceptsNullRuntimeValue(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol named)
        {
            bool isNullableStruct = named.GetFullSymbolName()
                !.Matches(CommonFullSymbolNames.SystemNullable, SymbolNameMatchingLevel.Namespace);

            return isNullableStruct || named.IsReferenceType;
        }

        if (typeSymbol is ITypeParameterSymbol typeParameter)
        {
            return !typeParameter.HasUnmanagedTypeConstraint
                && !typeParameter.HasValueTypeConstraint;
        }

        return typeSymbol
            is IPointerTypeSymbol
            or IFunctionPointerTypeSymbol
            or IArrayTypeSymbol
            or INullTypeSymbol;
    }
}
