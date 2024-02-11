using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using RoseLynn.Analyzers;
using RoseLynn.CSharp.Syntax;
using RoseLynn.Diagnostics;
using RossLean.GenericsAnalyzer.Core;
using RossLean.GenericsAnalyzer.Core.DataStructures;
using RossLean.GenericsAnalyzer.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

// The analyzer should not be run concurrently due to the state that it preserves
#pragma warning disable RS1026 // Enable concurrent execution

namespace RossLean.GenericsAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PermittedTypeArgumentAnalyzer : CSharpDiagnosticAnalyzer
{
    private readonly GenericTypeConstraintInfoCollection genericNames = new();
    private readonly TypeConstraintProfileInfoCollection constraintProfiles = new();

    protected override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => GADiagnosticDescriptorStorage.Instance;

    public override void Initialize(AnalysisContext context)
    {
        // Concurrent execution is disabled due to the stateful profile of the analyzer
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);

        RegisterNodeActions(context);
    }

    private void RegisterNodeActions(AnalysisContext context)
    {
        // Only executed on *usage* of a generic element
        context.RegisterSyntaxNodeAction(AnalyzeGenericNameOrFunctionCall, SyntaxKind.GenericName, SyntaxKind.IdentifierName);

        // Only executed on declaration of the following elements
        var genericSupportedMemberDeclarations = new SyntaxKind[]
        {
            SyntaxKind.MethodDeclaration,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.DelegateDeclaration,
        };
        context.RegisterSyntaxNodeAction(AnalyzeGenericDeclaration, genericSupportedMemberDeclarations);

        // Only executed on declaration of profiles (interfaces)
        context.RegisterSyntaxNodeAction(AnalyzeProfileRelatedDeclaration, SyntaxKind.InterfaceDeclaration);
    }

    private void AnalyzeGenericNameOrFunctionCall(SyntaxNodeAnalysisContext context)
    {
        var semanticModel = context.SemanticModel;

        var node = context.Node;
        var symbolInfo = semanticModel.GetSymbolInfo(node);
        var symbol = symbolInfo.Symbol;

        switch (node)
        {
            case IdentifierNameSyntax _
            when symbol is IMethodSymbol methodSymbol:
                if (!methodSymbol.OriginalDefinition.IsGenericMethod)
                    return;

                break;

            case GenericNameSyntax genericNameNode:
                if (genericNameNode.IsUnboundGenericName)
                    return;

                break;

            default:
                return;
        }

        var originalDefinition = symbol.OriginalDefinition;
        AnalyzeGenericNameDefinition(context, originalDefinition);
        AnalyzeGenericNameUsage(context, symbol, node as GenericNameSyntax);
    }
    private void AnalyzeGenericDeclaration(SyntaxNodeAnalysisContext context)
    {
        var semanticModel = context.SemanticModel;

        var declarationExpressionNode = context.Node as MemberDeclarationSyntax;

        if (!declarationExpressionNode.IsGeneric())
            return;

        var symbol = semanticModel.GetDeclaredSymbol(declarationExpressionNode);
        AnalyzeGenericNameDefinition(context, symbol);
    }

    private void AnalyzeProfileRelatedDeclaration(SyntaxNodeAnalysisContext context)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol;
        AnalyzeProfileRelatedDefinition(context, symbol);
    }

    private void AnalyzeProfileRelatedDefinition(SyntaxNodeAnalysisContext context, INamedTypeSymbol profileSymbol)
    {
        // Process whether the symbol is an interface because of the recursive processing
        // from given type arguments
        if (profileSymbol.TypeKind != TypeKind.Interface)
            return;

        if (constraintProfiles.ContainsDeclaringType(profileSymbol))
            return;

        var declaringSyntaxReferences = profileSymbol.DeclaringSyntaxReferences;
        if (declaringSyntaxReferences.IsEmpty)
            return;

        var typeConstraintAttributes = new List<AttributeData>();
        var templateTypes = TypeConstraintTemplateType.None;
        var templateAttributeStorage = new TypeConstraintTemplateAttributeStorage();

        var attributes = profileSymbol.GetAttributes();

        foreach (var attribute in attributes)
        {
            switch (attribute.AttributeClass.Name)
            {
                case nameof(TypeConstraintProfileAttribute):
                {
                    templateTypes |= TypeConstraintTemplateType.Profile;
                    templateAttributeStorage.ProfileAttribute = attribute;
                    break;
                }

                case nameof(TypeConstraintProfileGroupAttribute):
                {
                    templateTypes |= TypeConstraintTemplateType.ProfileGroup;
                    templateAttributeStorage.ProfileGroupAttribute = attribute;
                    break;
                }

                // If the attribute is not a profile-relevant one, it will be evaluated if it's
                // related to type constraints and cached there for GA0030 warnings
                default:
                    if (IsNonProfileTypeConstraintAttribute(attribute))
                        typeConstraintAttributes.Add(attribute);

                    continue;
            }
        }

        bool isProfile = templateTypes.HasFlag(TypeConstraintTemplateType.Profile);

        // If not a type constraint profile, the provided type constraint attributes have no effect
        if (!isProfile)
        {
            foreach (var typeConstraintAttribute in typeConstraintAttributes)
            {
                context.ReportDiagnostic(Diagnostics.CreateGA0030(typeConstraintAttribute.ApplicationSyntaxReference.GetSyntax() as AttributeSyntax));
            }
        }

        var baseTypeNodes = declaringSyntaxReferences.SelectMany(node => (node.GetSyntax() as InterfaceDeclarationSyntax).BaseList?.Types ?? default);
        foreach (var directlyInheritedInterfaceNode in baseTypeNodes)
        {
            var directlyInheritedInterface = context.SemanticModel.GetTypeInfo(directlyInheritedInterfaceNode).Type as INamedTypeSymbol;

            AnalyzeProfileRelatedDefinition(context, directlyInheritedInterface);

            bool inheritedIsProfile = constraintProfiles.ContainsProfile(directlyInheritedInterface);
            if (isProfile != inheritedIsProfile)
                context.ReportDiagnostic(Diagnostics.CreateGA0025(directlyInheritedInterfaceNode));
        }

        // Now analyze semantic information about the templates
        if (templateTypes is TypeConstraintTemplateType.None)
            return;

        // Finally analyze the type of the declared template

        var templateAttributeDeclaringNodes = new HashSet<InterfaceDeclarationSyntax>(
            templateAttributeStorage.GetAllAssociatedAttributes().Select(
                attribute => attribute.ApplicationSyntaxReference?.GetSyntax()?.GetNearestParentOfType<InterfaceDeclarationSyntax>()));
        // Must be non-generic regardless of the case
        if (profileSymbol.Arity > 0)
            context.ReportDiagnostics(templateAttributeDeclaringNodes, Diagnostics.CreateGA0023);

        switch (templateTypes)
        {
            case TypeConstraintTemplateType.Profile:
            {
                AnalyzeProfileDefinition(context, profileSymbol, templateAttributeStorage.ProfileAttribute);
                return;
            }
            case TypeConstraintTemplateType.ProfileGroup:
            {
                AnalyzeProfileGroupDefinition(context, profileSymbol, templateAttributeStorage.ProfileGroupAttribute);
                return;
            }

            // Either a mix of the flags, or an invalid value that should never exist
            default:
                context.ReportDiagnostics(templateAttributeDeclaringNodes, Diagnostics.CreateGA0029);
                return;
        }
    }

    private void AnalyzeProfileDefinition(SyntaxNodeAnalysisContext context, INamedTypeSymbol profileDeclarationType, AttributeData profileAttribute)
    {
        if (constraintProfiles.ContainsDeclaringType(profileDeclarationType))
            return;

        // Recursively also ensure that the inherited interfaces are valid
        var arguments = GetAttributeTypeArrayArgument(profileAttribute).ToArray();
        var validGroupArguments = new List<INamedTypeSymbol>();

        var attributeSyntaxReference = profileAttribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
        var attributeArgumentNodes = attributeSyntaxReference.ArgumentList?.Arguments;
        var memberDeclarationNode = attributeSyntaxReference?.GetNearestParentOfType<MemberDeclarationSyntax>();

        for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
        {
            var typeArgument = arguments[argumentIndex];
            var namedTypeArgument = typeArgument as INamedTypeSymbol;
            var attributeArgumentNode = attributeArgumentNodes?[argumentIndex];

            bool erroneousGroupType = namedTypeArgument is null;

            if (!erroneousGroupType)
            {
                // Do not assume that the given argument is an interface; let alone a profile group
                AnalyzeProfileRelatedDefinition(context, namedTypeArgument);
                erroneousGroupType |= !constraintProfiles.ContainsGroup(namedTypeArgument);
            }

            // Might wanna test this more carefully
            if (erroneousGroupType)
            {
                context.ReportDiagnostic(Diagnostics.CreateGA0027(attributeArgumentNode));
            }
            else
            {
                validGroupArguments.Add(namedTypeArgument);
            }
        }

        constraintProfiles.AddProfile(profileDeclarationType, validGroupArguments);
        var profileInfo = constraintProfiles.GetProfileInfo(profileDeclarationType);

        // Profile type declaration does not handle any attributes that are not already handled
        // for all handlers
        foreach (var attribute in profileDeclarationType.GetAttributes())
            ProcessAttribute(profileInfo.Builder, attribute, NoUnhandledAttributeProcessor);

        foreach (var inheritedInterface in profileDeclarationType.Interfaces)
            AnalyzeProfileRelatedDefinition(context, inheritedInterface);

        profileInfo.FinalizeSystem();
    }
    private void AnalyzeProfileGroupDefinition(SyntaxNodeAnalysisContext context, INamedTypeSymbol groupDeclarationType, AttributeData profileGroupAttribute)
    {
        if (constraintProfiles.ContainsDeclaringType(groupDeclarationType))
            return;

        var semanticModel = context.SemanticModel;

        // Preferably should not contain any instance members
        // Nested profile types should also be ideally avoided?
        var declaredMembers = groupDeclarationType.GetMembers();
        var declaredInstanceMembers = declaredMembers.Where(member => !member.IsStatic).ToImmutableArray();
        if (!declaredInstanceMembers.IsEmpty)
        {
            // Now that's an expression
            var declarationSet = new HashSet<InterfaceDeclarationSyntax>(
                declaredInstanceMembers.SelectMany(
                    instanceMember => instanceMember.DeclaringSyntaxReferences.Select(
                        reference => reference?.GetSyntax()?.GetNearestParentOfType<InterfaceDeclarationSyntax>())));
            foreach (var declarationSetNode in declarationSet)
                context.ReportDiagnostic(Diagnostics.CreateGA0024(declarationSetNode));
        }

        bool isDistinct = (bool)profileGroupAttribute.ConstructorArguments[0].Value;
        constraintProfiles.AddGroup(groupDeclarationType, isDistinct);
    }

    private void AnalyzeGenericNameDefinition(SyntaxNodeAnalysisContext context, ISymbol declaringSymbol)
    {
        if (genericNames.ContainsInfo(declaringSymbol))
            return;

        var typeParameters = declaringSymbol.GetTypeParameters();
        if (typeParameters.IsEmpty)
            return;

        var semanticModel = context.SemanticModel;

        var typeParameterNameIndexer = typeParameters.ToDictionary(t => t.Name);

        var constraints = new GenericTypeConstraintInfo.Builder(declaringSymbol);
        genericNames.SetBuilder(declaringSymbol, constraints);

        var typeConstraintInheritAttibuteData = new List<AttributeData>();

        for (int i = 0; i < typeParameters.Length; i++)
        {
            var parameter = typeParameters[i];
            var attributes = parameter.GetAttributes();

            var systemBuilder = constraints[i];
            InitializeSystem();

            var finiteTypeCount = systemBuilder.GetFinitePermittedTypeCount();

            // Re-iterate over the attributes to mark erroneous types
            MarkErroneousConstrainedTypes();

            void InitializeSystem()
            {
                foreach (var attribute in attributes)
                {
                    ProcessAttribute(systemBuilder, attribute, ProcessNonConstraintRuleAttribute);
                }
            }
#nullable enable
            bool ProcessNonConstraintRuleAttribute(AttributeData attributeData)
            {
                var attributeNode = attributeData.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
                var processor = GetNonConstraintRuleAttributeProcessor(attributeData);
                return processor?.Invoke(attributeData, attributeNode) ?? false;
            }
            AttributeDataSyntaxProcessor? GetNonConstraintRuleAttributeProcessor(AttributeData attributeData)
#nullable restore
            {
                return attributeData.AttributeClass.Name switch
                {
                    nameof(InheritBaseTypeUsageConstraintsAttribute) => ProcessInheritBaseTypeUsageConstraintsAttribute,
                    nameof(InheritProfileTypeConstraintsAttribute) => ProcessInheritProfileTypeConstraintsAttribute,
                    nameof(InheritTypeConstraintsAttribute) => ProcessInheritTypeConstraintsAttribute,
                    _ => null,
                };
            }
            bool ProcessInheritBaseTypeUsageConstraintsAttribute(AttributeData attributeData, AttributeSyntax attributeNode)
            {
                if (AnalyzeBaseTypeArgumentUsageAttributeUsage(attributeNode, declaringSymbol, parameter, context))
                    return true;

                var type = declaringSymbol as INamedTypeSymbol;

                var inheritedTypes = type.GetBaseTypeAndInterfaces();

                foreach (var inheritedType in inheritedTypes)
                {
                    if (!inheritedType.IsGenericType)
                        continue;

                    // Recursively analyze base type definitions
                    var inheritedTypeOriginalDefinition = inheritedType.OriginalDefinition;
                    AnalyzeGenericNameDefinition(context, inheritedTypeOriginalDefinition);

                    var inheritedTypeArguments = inheritedType.TypeArguments;
                    var inheritedTypeParameters = inheritedType.TypeParameters;

                    // TODO: Consider analyzing and reporting the source of the rule collision
                    for (int k = 0; k < inheritedTypeArguments.Length; k++)
                    {
                        if (inheritedTypeArguments[k].Name == parameter.Name)
                        {
                            var inheritedTypeParameter = inheritedTypeParameters[k];
                            genericNames.GetBuilderOrFinalizedInfo(inheritedTypeOriginalDefinition, out var finalized, out var builder);

                            bool safeInheritance;
                            if (builder is null)
                                safeInheritance = systemBuilder.InheritFrom(inheritedTypeParameter, finalized[k]);
                            else
                                safeInheritance = systemBuilder.InheritFrom(inheritedTypeParameter, builder[k]);

                            if (!safeInheritance)
                            {
                                var erroneousInheritanceSource = systemBuilder.SystemDiagnostics.GetTypeParametersForDiagnosticType(TypeConstraintSystemInheritanceDiagnosticType.Conflicting);

                                var parameterDeclaration = attributeNode.GetParentAttributeList().Parent as TypeParameterSyntax;
                                context.ReportDiagnostic(Diagnostics.CreateGA0022(parameterDeclaration));
                                return true;
                            }
                        }
                    }
                }
                return true;
            }
            bool ProcessInheritProfileTypeConstraintsAttribute(AttributeData attributeData, AttributeSyntax attributeNode)
            {
                // Validate that the arguments are correct
                var arguments = GetAttributeTypeArrayArgument(attributeData).ToArray();
                if (!arguments.Any())
                    return true;

                var argumentNodes = attributeNode.ArgumentList.Arguments;

                // I'm noticing too much copy-pasted code and it's not fun
                for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
                {
                    var typeArgument = arguments[argumentIndex];
                    var typeArgumentNode = argumentNodes[argumentIndex];

                    var namedTypeArgument = typeArgument as INamedTypeSymbol;
                    bool erroneousProfileType = namedTypeArgument is null;

                    if (!erroneousProfileType)
                    {
                        // Do not assume that the given argument is an interface; let alone a profile
                        AnalyzeProfileRelatedDefinition(context, namedTypeArgument);
                        erroneousProfileType |= !constraintProfiles.ContainsProfile(namedTypeArgument);
                    }

                    if (erroneousProfileType)
                        context.ReportDiagnostic(Diagnostics.CreateGA0026(typeArgumentNode));
                    else
                    {
                        systemBuilder.InheritFrom(constraintProfiles.GetProfileInfo(namedTypeArgument));
                    }
                }

                systemBuilder.AnalyzeFinalizedInheritedTypeProfiles();
                var multipleOfDistinctGroupsProfiles = systemBuilder.SystemDiagnostics.GetProfilesForDiagnosticType(TypeConstraintSystemInheritanceDiagnosticType.MultipleOfDistinctGroup);

                for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
                {
                    if (multipleOfDistinctGroupsProfiles.Contains(arguments[argumentIndex]))
                    {
                        context.ReportDiagnostic(Diagnostics.CreateGA0028(argumentNodes[argumentIndex]));
                    }
                }

                return true;
            }
            bool ProcessInheritTypeConstraintsAttribute(AttributeData attributeData, AttributeSyntax attributeNode)
            {
                // This will be analyzed after the first iteration to ensure all constraints are properly loaded
                typeConstraintInheritAttibuteData.Add(attributeData);
                return true;
            }

            void MarkErroneousConstrainedTypes()
            {
                var typeSystemDiagnostics = systemBuilder.AnalyzeFinalizedBaseSystem();

                foreach (var attribute in attributes)
                    MarkErroneousConstrainedTypesAttribute(typeSystemDiagnostics, attribute);
            }
            void MarkErroneousConstrainedTypesAttribute(TypeConstraintSystemDiagnostics typeSystemDiagnostics, AttributeData attribute)
            {
                if (attribute.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax attributeSyntaxNode)
                    return;

                if (!IsNonProfileTypeConstraintAttribute(attribute))
                    return;

                switch (attribute.AttributeClass.Name)
                {
                    case nameof(InheritBaseTypeUsageConstraintsAttribute):
                    {
                        // You will be soon used, don't worry
                        return;
                    }

                    // Profiles do not care about this diagnostic because they are not being directly used
                    // TODO: This will have to emit a diagnostic on the generic parameter if it can be
                    // applied but no such attribute is directly attributed to that element
                    case nameof(OnlyPermitSpecifiedTypesAttribute):
                    {
                        // TODO: Refactor this into a system diagnostic that will be accessible
                        if (systemBuilder.HasNoPermittedTypes)
                            context.ReportDiagnostic(Diagnostics.CreateGA0012(attributeSyntaxNode));
                        return;
                    }

                    // Finally, the only attributes that demand being processed are the ones applying
                    // constraints
                    case nameof(PermittedTypesAttribute):
                    case nameof(PermittedBaseTypesAttribute):
                    case nameof(ProhibitedTypesAttribute):
                    case nameof(ProhibitedBaseTypesAttribute):
                        break;

                    // This excludes the profile-related attributes from having their arguments marked
                    // as erroneous, since they are not related to constraining types and already have
                    // been processed earlier
                    // Furthermore, any other attributes should not correlate with type argument
                    // constraints, unless they are related to logic
                    default:
                        return;
                }

                if (finiteTypeCount is 1)
                    context.ReportDiagnostic(Diagnostics.CreateGA0013(attributeSyntaxNode, parameter));

                var argumentList = attributeSyntaxNode.ArgumentList;
                if (argumentList is null)
                    return;

                var argumentNodes = argumentList.Arguments;
                var typeConstants = GetAttributeTypeArrayArgument(attribute).ToArray();
                for (int argumentIndex = 0; argumentIndex < typeConstants.Length; argumentIndex++)
                {
                    var typeConstant = typeConstants[argumentIndex];
                    var argumentNode = argumentNodes[argumentIndex];

                    var type = typeSystemDiagnostics.GetDiagnosticType(typeConstant);

                    var diagnostic = CreateReportDiagnostic();
                    if (diagnostic != null)
                        context.ReportDiagnostic(diagnostic);

                    // "Using a non-static local function is fine."
                    //                              -- Rekkon, 2021
                    Diagnostic CreateReportDiagnostic()
                    {
                        switch (type)
                        {
                            case TypeConstraintSystemDiagnosticType.Conflicting:
                                return Diagnostics.CreateGA0002(argumentNode, parameter, typeConstant);

                            case TypeConstraintSystemDiagnosticType.Duplicate:
                                return Diagnostics.CreateGA0009(argumentNode, typeConstant);

                            case TypeConstraintSystemDiagnosticType.InvalidTypeArgument:
                                return Diagnostics.CreateGA0004(argumentNode, typeConstant);

                            case TypeConstraintSystemDiagnosticType.ConstrainedTypeArgumentSubstitution:
                                return Diagnostics.CreateGA0005(argumentNode, typeConstant, parameter);

                            case TypeConstraintSystemDiagnosticType.RedundantlyPermitted:
                                return Diagnostics.CreateGA0011(argumentNode, typeConstant);

                            case TypeConstraintSystemDiagnosticType.RedundantlyProhibited:
                                return Diagnostics.CreateGA0010(argumentNode, typeConstant);

                            case TypeConstraintSystemDiagnosticType.ReducibleToConstraintClause:
                                return Diagnostics.CreateGA0006(argumentNode);

                            case TypeConstraintSystemDiagnosticType.RedundantBaseTypeRule:
                                return Diagnostics.CreateGA0008(argumentNode, typeConstant);

                            case TypeConstraintSystemDiagnosticType.RedundantBoundUnboundRule:
                                return Diagnostics.CreateGA0003(argumentNode, parameter, typeConstant as INamedTypeSymbol);
                        }
                        return null;
                    }
                }
            }
        }
        // Analyze the inherited type constaints from local type parameters
        AnalyzeInheritedTypeConstraints();
        genericNames.FinalizeGenericSymbol(declaringSymbol);

        void AnalyzeInheritedTypeConstraints()
        {
            var inheritMap = new Dictionary<ITypeParameterSymbol, TypeParameterAttributeArgumentCorrelationDictionary>(SymbolEqualityComparer.Default);
            var typeParameterInheritanceArguments = new Dictionary<ITypeParameterSymbol, SeparatedSyntaxList<AttributeArgumentSyntax>>(SymbolEqualityComparer.Default);

            foreach (var p in typeParameters)
            {
                inheritMap.Add(p, new TypeParameterAttributeArgumentCorrelationDictionary());
                typeParameterInheritanceArguments.Add(p, SyntaxFactory.SeparatedList<AttributeArgumentSyntax>());
            }

            // Create inherit map from attribute data
            foreach (var attributeData in typeConstraintInheritAttibuteData)
            {
                if (attributeData is null)
                    return;

                var ctorArguments = attributeData.ConstructorArguments;
                if (ctorArguments.Length is 0)
                    return;

                var typeParameterNames = ctorArguments[0].Values.Select(c => c.Value as string).ToArray();

                // TODO: Validate that this can never be null
                var attributeNode = attributeData.ApplicationSyntaxReference.GetSyntax(context.CancellationToken) as AttributeSyntax;
                var arguments = attributeNode.ArgumentList.Arguments;

                var originalTypeParameterNode = attributeNode.Parent.Parent as TypeParameterSyntax;
                var originalTypeParameter = semanticModel.GetDeclaredSymbol(originalTypeParameterNode);

                typeParameterInheritanceArguments[originalTypeParameter] = arguments;

                for (int j = 0; j < typeParameterNames.Length; j++)
                {
                    var inheritingTypeParameterName = typeParameterNames[j];
                    var attributeArgumentNode = arguments[j];

                    if (inheritingTypeParameterName == originalTypeParameter.Name)
                    {
                        context.ReportDiagnostic(Diagnostics.CreateGA0021(attributeArgumentNode));
                        continue;
                    }

                    var inheritingTypeParameter = typeParameters.FirstOrDefault(p => p.Name == inheritingTypeParameterName);
                    if (inheritingTypeParameter is null)
                    {
                        context.ReportDiagnostic(Diagnostics.CreateGA0019(attributeArgumentNode, inheritingTypeParameterName));
                        continue;
                    }

                    var originalTypeParameterInherit = inheritMap[originalTypeParameter];
                    originalTypeParameterInherit.Add(inheritingTypeParameter, attributeArgumentNode);
                }
            }

            // Recursively inherit
            var inheritStack = new StackSet<ITypeParameterSymbol>(inheritMap.Count, SymbolEqualityComparer.Default);
            foreach (var inheritor in inheritMap)
            {
                var inheritorType = inheritor.Key;
                var correlationDictionary = inheritor.Value;

                while (correlationDictionary.Any())
                {
                    var inheritedTypeCorrelation = correlationDictionary.First();
                    var inheritedType = inheritedTypeCorrelation.Key;

                    bool isRecursiveInheritance = false;

                    // Discover the inheritance stack
                    while (inheritedType != null)
                    {
                        if (isRecursiveInheritance = !inheritStack.Push(inheritorType))
                        {
                            // The diagnostic is always emitted on the source of the recursion
                            context.ReportDiagnostics(inheritedTypeCorrelation.Value, a => Diagnostics.CreateGA0020(a, inheritStack.Reverse()));
                            break;
                        }

                        inheritorType = inheritedType;
                        inheritedTypeCorrelation = inheritMap[inheritorType].FirstOrDefault();
                        inheritedType = inheritedTypeCorrelation.Key;
                    }

                    // Consume the inheritance stack
                    bool safeInheritance = true;
                    while (!inheritStack.IsEmpty)
                    {
                        inheritedType = inheritorType;
                        inheritorType = inheritStack.Pop();
                        inheritedTypeCorrelation = inheritMap[inheritorType].GetKeyValuePair(inheritedType);

                        if (!isRecursiveInheritance && safeInheritance)
                        {
                            // Apply inheritance
                            safeInheritance = constraints[inheritorType].InheritFrom(inheritedType, constraints[inheritedType]);
                        }

                        if (!safeInheritance)
                        {
                            inheritedTypeCorrelation.Value.First().GetAttributeRelatedParents(out _, out _, out var attributeList);
                            var typeParameterDeclaration = attributeList.Parent as TypeParameterSyntax;
                            context.ReportDiagnostic(Diagnostics.CreateGA0022(typeParameterDeclaration));
                        }

                        inheritMap[inheritorType].Remove(inheritedType);
                    }
                }
            }
        }
    }

    private void AnalyzeGenericNameUsage(SyntaxNodeAnalysisContext context, ISymbol symbol, GenericNameSyntax genericNameNode)
    {
        var originalDefinition = symbol.OriginalDefinition;
        var typeArguments = symbol.GetTypeArguments();

        var typeArgumentNodes = genericNameNode?.TypeArgumentList.Arguments.ToArray();

        var constraints = genericNames[originalDefinition];
        var candidateErrorNode = context.Node;

        for (int i = 0; i < typeArguments.Length; i++)
        {
            if (typeArgumentNodes != null)
                candidateErrorNode = typeArgumentNodes[i];

            var argumentType = typeArguments[i];
            var system = constraints[i];

            if (argumentType is ITypeParameterSymbol declaredTypeParameter)
            {
                var declaringElementTypeParameters = declaredTypeParameter.GetDeclaringSymbol().GetTypeParameters();

                foreach (var declaringElementTypeParameter in declaringElementTypeParameters)
                {
                    if (declaringElementTypeParameter.Name == declaredTypeParameter.Name)
                    {
                        if (!system.IsPermitted(declaredTypeParameter, genericNames))
                            context.ReportDiagnostic(Diagnostics.CreateGA0017(candidateErrorNode, originalDefinition, argumentType));

                        break;
                    }
                }
            }
            else
            {
                if (!system.IsPermitted(argumentType))
                    context.ReportDiagnostic(Diagnostics.CreateGA0001(candidateErrorNode, originalDefinition, argumentType));
            }
        }
    }

    // Emits GA0014, GA0015, GA0016
    private bool AnalyzeBaseTypeArgumentUsageAttributeUsage(AttributeSyntax attributeNode, ISymbol symbol, ITypeParameterSymbol parameter, SyntaxNodeAnalysisContext context)
    {
        if (attributeNode is null)
            return false;

        var type = symbol as INamedTypeSymbol;

        if (symbol is IMethodSymbol || !type.TypeKind.CanExplicitlyInheritTypes())
        {
            context.ReportDiagnostic(Diagnostics.CreateGA0014(attributeNode, symbol));
            return true;
        }

        var allBaseTypes = type.GetBaseTypeAndDirectInterfaces().ToImmutableArray();
        var allGenericBaseTypes = allBaseTypes.Where(t => t.IsGenericType).ToImmutableArray();

        if (!allGenericBaseTypes.Any())
        {
            context.ReportDiagnostic(Diagnostics.CreateGA0015(attributeNode, symbol));
            return true;
        }

        bool typeUsedInBaseTypes = false;
        foreach (var baseType in allGenericBaseTypes)
        {
            // The type has type arguments substitute the base type's type parameters
            if (baseType.TypeArguments.Any(arg => arg.Name == parameter.Name))
            {
                typeUsedInBaseTypes = true;
                break;
            }
        }

        if (!typeUsedInBaseTypes)
        {
            context.ReportDiagnostic(Diagnostics.CreateGA0016(attributeNode, symbol));
            return true;
        }

        return false;
    }

    private static bool NoUnhandledAttributeProcessor(AttributeData _) => false;

    private delegate bool AttributeProcessor(AttributeData attributeData);
    private delegate bool AttributeDataSyntaxProcessor(AttributeData attributeData, AttributeSyntax attributeNode);

    private bool ProcessAttribute(TypeConstraintSystem.Builder systemBuilder, AttributeData attributeData, AttributeProcessor attributeProcessor)
    {
        if (!IsNonProfileTypeConstraintAttribute(attributeData))
            return false;

        // Using switch for when the other constraint attributes come into play
        switch (attributeData.AttributeClass.Name)
        {
            case nameof(OnlyPermitSpecifiedTypesAttribute):
                systemBuilder.OnlyPermitSpecifiedTypes = true;
                return true;

            default:
                if (attributeProcessor(attributeData))
                    return true;
                break;
        }

        // It is assured that the analyzer cares about the attribute from the base interface check
        var rule = ParseAttributeRule(attributeData).Value;

        // The arguments will be always stored as an array, regardless of their count
        // If an error is thrown here, common causes could be:
        // - having forgotten to import a namespace
        // - accidentally asserting unit test markup code as valid instead of asserting diagnostics
        // - neglecting that type constraint attributes may also be applied to interfaces and not just type params
        systemBuilder.Add(rule, GetAttributeTypeArrayArgument(attributeData));

        return true;
    }

    private static bool IsNonProfileTypeConstraintAttribute(AttributeData data)
    {
        return data.AttributeClass.IsNonProfileTypeConstraintAttribute();
    }
    private static IEnumerable<ITypeSymbol> GetAttributeTypeArrayArgument(AttributeData data)
    {
        return data.ConstructorArguments[0].Values.Select(c => c.Value as ITypeSymbol);
    }

    private static TypeConstraintRule? ParseAttributeRule(AttributeData data)
    {
        return ConstrainedTypesAttributeBase.GetConstraintRuleFromAttributeName(data.AttributeClass.Name);
    }
}
