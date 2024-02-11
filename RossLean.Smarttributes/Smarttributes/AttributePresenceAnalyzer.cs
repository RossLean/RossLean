using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using System.Collections.Immutable;

namespace RossLean.Smarttributes;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AttributePresenceAnalyzer : BaseAttributeAnalyzer
{
    protected override void AnalyzeAttributedSymbol(AttributeSyntaxNodeAnalysisContext attributeContext)
    {
        var (context, _, _, declaredSymbol) = attributeContext;

        var declaredSymbolAttributes = declaredSymbol!.GetAttributes();

        var missingAttributes = new List<ITypeSymbol>();

        foreach (var declaredSymbolAttribute in declaredSymbolAttributes)
        {
            var declaredSymbolAttributeClass = declaredSymbolAttribute.AttributeClass;
            if (declaredSymbolAttributeClass is null)
                continue;

            var attributePresenceAttributes = GetAttributePresenceAttributes(declaredSymbolAttributeClass);
            if (attributePresenceAttributes.Length is 0)
                continue;

            missingAttributes.Clear();

            foreach (var attributePresenceAttribute in attributePresenceAttributes)
            {
                foreach (var type in attributePresenceAttribute.Types)
                {
                    bool containsRequiredAttribute = declaredSymbolAttributes.Any(a => type.Equals(a.AttributeClass, SymbolEqualityComparer.Default));

                    if (!containsRequiredAttribute)
                    {
                        missingAttributes.Add(type);
                    }
                }
            }

            var diagnostic = Diagnostics.CreateSMTTR0001(
                declaredSymbol,
                declaredSymbolAttribute,
                missingAttributes);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static ImmutableArray<RequiresPresenceAttributeData> GetAttributePresenceAttributes(ISymbol symbol)
    {
        var attributes = symbol.GetAttributes();
        return RequiresPresenceAttributeData.ParseRange(attributes);
    }

    private sealed record RequiresPresenceAttributeData(AttributeData Attribute, ImmutableArray<ITypeSymbol> Types)
        : CustomAttributeData(Attribute)
    {
        public static ImmutableArray<RequiresPresenceAttributeData> ParseRange(ImmutableArray<AttributeData> attributes)
        {
            return attributes
                .Select(Parse)
                .Where(r => r is not null)
                .ToImmutableArray()!;
        }

        public static RequiresPresenceAttributeData? Parse(AttributeData attribute)
        {
            if (attribute.AttributeClass is null)
                return null;

            var attributeClassFullName = attribute.AttributeClass.GetFullSymbolName()!;

            bool matchesGeneric = attributeClassFullName.MatchesAny(
                SymbolNameMatchingLevel.Namespace,
                KnownSymbolNames.Full.RequiresPresenceAttributeT1,
                KnownSymbolNames.Full.RequiresPresenceAttributeT2,
                KnownSymbolNames.Full.RequiresPresenceAttributeT3,
                KnownSymbolNames.Full.RequiresPresenceAttributeT4);

            if (matchesGeneric)
            {
                var types = attribute.AttributeClass.TypeArguments;
                return new(attribute, types);
            }

            bool matchesBaseAttribute = attributeClassFullName.Matches(
                KnownSymbolNames.Full.RequiresPresenceAttribute,
                SymbolNameMatchingLevel.Namespace);

            if (matchesBaseAttribute)
            {
                var types = attribute.ConstructorArguments[0].Values
                    .Select(d => d.Type!)
                    .ToImmutableArray();

                return new(attribute, types);
            }

            return null;
        }
    }
}
