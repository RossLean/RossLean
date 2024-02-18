using Garyon.Extensions;
using Garyon.Functions;
using Garyon.Reflection;
using Microsoft.CodeAnalysis;
using RoseLynn;
using RoseLynn.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;

// Aliases like that are great
using RuleEqualityComparer = Func<KeyValuePair<ITypeSymbol, TypeConstraintRule>, bool>;

// Resolving ambiguity with Garyon.Reflection.TypeKind
using TypeKind = Microsoft.CodeAnalysis.TypeKind;

/// <summary>Represents a system that contains a set of rules about type constraints.</summary>
public class TypeConstraintSystem
{
    private readonly Dictionary<ITypeSymbol, TypeConstraintRule> typeConstraintRules = new(SymbolEqualityComparer.Default);
    private readonly HashSet<ITypeParameterSymbol> inheritedTypes = new(SymbolEqualityComparer.Default);
    private readonly HashSet<INamedTypeSymbol> inheritedProfiles = new(SymbolEqualityComparer.Default);

    // TODO: Use IKeyedObject and KeyedObjectDictionary
    private readonly Dictionary<INamedTypeSymbol, TypeConstraintProfileInfo> inheritedProfileInfos = new(SymbolEqualityComparer.Default);
    private readonly DistinctGroupDictionary inheritedProfileDistinctGroups = new();

    private readonly TypeConstraintSystemDiagnostics systemDiagnostics = new();

    private Dictionary<TypeConstraintRule, HashSet<ITypeSymbol>> cachedTypeConstraintsByRule;

    public TypeConstraintSystemDiagnostics SystemDiagnostics => new(systemDiagnostics);

    public INamedTypeSymbol ProfileInterface { get; }
    public ITypeParameterSymbol TypeParameter { get; }

    public bool OnlyPermitSpecifiedTypes { get; set; }
    public bool OnlyPermitSpecifiedTypeGroups { get; set; }
    public TypeGroupFilters Filters { get; } = new();

    public Dictionary<TypeConstraintRule, HashSet<ITypeSymbol>> TypeConstraintsByRule
    {
        get
        {
            var result = new Dictionary<TypeConstraintRule, HashSet<ITypeSymbol>>();
            foreach (var rule in TypeConstraintRule.AllValidRules)
                result.Add(rule, new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default));

            foreach (var r in typeConstraintRules)
                result[r.Value].Add(r.Key);

            return result;
        }
    }

    public TypeConstraintSystem(INamedTypeSymbol profileInterface)
    {
        ProfileInterface = profileInterface;
    }
    public TypeConstraintSystem(ITypeParameterSymbol parameter)
    {
        TypeParameter = parameter;
    }

    #region Rule Equality Comparer Creators
    private static RuleEqualityComparer GetRuleEqualityComparer(TypeConstraintRule rule) => kvp => kvp.Value == rule;
    private static RuleEqualityComparer GetRuleEqualityComparer(ConstraintRule rule) => kvp => kvp.Value.Rule == rule;
    private static RuleEqualityComparer GetRuleEqualityComparer(TypeConstraintReferencePoint rule) => kvp => kvp.Value.TypeReferencePoint == rule;
    #endregion

    #region Diagnostics
    public bool HasNoExplicitlyPermittedTypes
    {
        get
        {
            return !typeConstraintRules
                .Any(GetRuleEqualityComparer(ConstraintRule.Permit));
        }
    }

    public bool HasNoPermittedTypes
    {
        get
        {
            if (OnlyPermitSpecifiedTypes)
                return HasNoExplicitlyPermittedTypes;

            if (HasNoPermittedTypesFromTypeGroupFilters)
            {
                return true;
            }

            return false;
        }
    }

    public bool HasNoPermittedTypesFromTypeGroupFilters
    {
        get
        {
            if (OnlyPermitSpecifiedTypeGroups)
            {
                var typeGroups = Filters.AllPermittedOrExclusiveFilteredTypeGroups();
                if (typeGroups is FilterableTypeGroups.None)
                    return true;
            }

            return false;
        }
    }

    private TypeConstraintSystemDiagnostics AnalyzeFinalizedSystem()
    {
        cachedTypeConstraintsByRule = TypeConstraintsByRule;
        AnalyzeRedundantlyConstrainedTypes();
        AnalyzeConstraintClauseReducibility();
        AnalyzeRedundantBoundUnboundRuleTypes();
        AnalyzeTypeGroupFilters();
        return SystemDiagnostics;
    }

    private void AnalyzeTypeGroupFilters()
    {
        var filterInstances = Filters.GetFilterInstances();
        var filterInstanceDictionary = filterInstances
            .GroupBy(s => s.FilterType)
            .ToDictionary(d => d.Key);

        var exclusives = filterInstanceDictionary.ValueOrDefault(FilterType.Exclusive);
        var permitted = filterInstanceDictionary.ValueOrDefault(FilterType.Permitted);
        var prohibited = filterInstanceDictionary.ValueOrDefault(FilterType.Prohibited);

        if (exclusives is not null)
        {
            // Incompatible exclusive filter analysis

            var totalExclusiveFilters = FilterableTypeGroups.None;
            foreach (var group in exclusives)
            {
                totalExclusiveFilters |= group.Identifier.TypeGroup;
            }

            bool isValidCombination = FilterableTypeGroupFacts.IsValidCombination(
                totalExclusiveFilters);

            if (!isValidCombination)
            {
                var exclusiveIdentifiers = exclusives.Select(static s => s.Identifier);
                systemDiagnostics.RegisterIncompatibleExclusionFilters(exclusiveIdentifiers);
            }

            // Unavailable and redundant non-exclusive filter analysis

            if (permitted is not null)
            {
                foreach (var group in permitted)
                {
                    systemDiagnostics.RegisterUnavailablePermissionByExclusion(group.Identifier);
                }
            }

            if (prohibited is not null)
            {
                foreach (var group in prohibited)
                {
                    var prohibitedGroupFlag = group.Identifier.TypeGroup;
                    var hypotheticalCombination = totalExclusiveFilters | prohibitedGroupFlag;
                    // Only report the diagnostic when the prohibited group does not form
                    // a viable filtering combination, in which case the set of allowed
                    // types is unaffected
                    if (!FilterableTypeGroupFacts.IsValidCombination(hypotheticalCombination))
                    {
                        systemDiagnostics.RegisterRedundantProhibitionByExclusion(group.Identifier);
                    }
                }
            }

            if (OnlyPermitSpecifiedTypeGroups)
            {
                systemDiagnostics.SetHasRedundantOnlyPermitAttributeByExclusion();
            }
        }

        // Specializable type group filter instance analysis

        var generics = SpecializableTypeGroupFilterInstanceCollection
            .FromInstances(filterInstances
                .Where(static s => s.Identifier is GenericTypeGroupFilter)
            );
        var arrays = SpecializableTypeGroupFilterInstanceCollection
            .FromInstances(filterInstances
                .Where(static s => s.Identifier is ArrayTypeGroupFilter)
            );

        AnalyzeSpecializableCollection(generics);
        AnalyzeSpecializableCollection(arrays);

        // Duplicate specialization registration analysis

        ReportDuplicateSpecializationIdentifiers(
            Filters.GenericTypes,
            GenericTypeGroupFilter.DefaultOrSpecialized);
        ReportDuplicateSpecializationIdentifiers(
            Filters.Arrays,
            ArrayTypeGroupFilter.DefaultOrSpecialized);

        // Invalid specialization analysis

        ReportInvalidSpecializationIdentifiers(
            Filters.GenericTypes,
            static s => s is 0,
            GenericTypeGroupFilter.Specialized);
        ReportInvalidSpecializationIdentifiers(
            Filters.Arrays,
            static s => s is 0,
            ArrayTypeGroupFilter.Specialized);
    }

    private void ReportInvalidSpecializationIdentifiers<T>(
        CaseCollectionFilters caseCollectionFilters,
        Func<uint, bool> invalidSpecializationFilter,
        Func<uint, T> identifierSelector)
        where T : ISpecializableTypeGroupFilterIdentifier
    {
        var specializations = caseCollectionFilters.Remaining.Keys;
        var invalidSpecializations = specializations
            .Where(invalidSpecializationFilter)
            .Select(identifierSelector);
        
        foreach (var identifier in invalidSpecializations)
        {
            systemDiagnostics.RegisterInvalidSpecialization(identifier);
        }
    }

    private void ReportDuplicateSpecializationIdentifiers<T>(
        CaseCollectionFilters caseCollectionFilters,
        Func<uint?, T> identifierSelector)
        where T : ISpecializableTypeGroupFilterIdentifier
    {
        var identifiers = SelectDuplicateSpecializationIdentifiers(
            caseCollectionFilters,
            identifierSelector);

        foreach (var identifier in identifiers)
        {
            systemDiagnostics.RegisterDuplicateSpecialization(identifier);
        }
    }

    private IReadOnlyList<T> SelectDuplicateSpecializationIdentifiers<T>(
        CaseCollectionFilters caseCollectionFilters,
        Func<uint?, T> identifierSelector)
        where T : ISpecializableTypeGroupFilterIdentifier
    {
        var values = caseCollectionFilters.GetDuplicateSpecializationValues();
        if (values.Count is 0)
            return [];

        return values.Select(identifierSelector)
            .ToArray();
    }

    private void AnalyzeSpecializableCollection(
        SpecializableTypeGroupFilterInstanceCollection filters)
    {
        AnalyzePotentialRedundantSpecialization(filters);
        AnalyzeRedundantExclusionOfDefaultSpecializable(filters);
        AnalyzeIncompatibleExclusiveSpecializations(filters);
        AnalyzeRedundantDefaultCaseByExclusiveSpecialization(filters);
    }

    private void AnalyzePotentialRedundantSpecialization(
        SpecializableTypeGroupFilterInstanceCollection filters)
    {
        var defaultCase = filters.DefaultCase;

        if (defaultCase is null)
            return;

        if (defaultCase.FilterType is FilterType.Exclusive)
            return;

        foreach (var specializedInstance in filters.SpecializedCases)
        {
            var identifier = specializedInstance.Identifier
                as ISpecializableTypeGroupFilterIdentifier
                ?? throw new InvalidOperationException("Expected specializable type group filter");

            if (specializedInstance.FilterType == defaultCase.FilterType)
            {
                systemDiagnostics.RegisterRedundantSpecialization(identifier);
            }
        }
    }

    private void AnalyzeRedundantExclusionOfDefaultSpecializable(
        SpecializableTypeGroupFilterInstanceCollection filters)
    {
        var defaultCase = filters.DefaultCase;

        if (defaultCase is null)
            return;

        if (defaultCase.FilterType is not FilterType.Exclusive)
            return;

        foreach (var specializedInstance in filters.SpecializedCases)
        {
            if (specializedInstance.FilterType is FilterType.Exclusive)
            {
                systemDiagnostics.RegisterRedundantSpecialization(defaultCase.Identifier);
            }
        }
    }

    private void AnalyzeIncompatibleExclusiveSpecializations(
        SpecializableTypeGroupFilterInstanceCollection filters)
    {
        var exclusives = filters.SpecializedCases
            .Where(specializedInstance => specializedInstance.FilterType is FilterType.Exclusive)
            .Select(specializedInstance => specializedInstance.Identifier)
            .Cast<ISpecializableTypeGroupFilterIdentifier>()
            .ToList();

        if (exclusives.Count > 1)
        {
            systemDiagnostics.RegisterConflictingExclusiveSpecializationFilters(exclusives);
        }
    }

    private void AnalyzeRedundantDefaultCaseByExclusiveSpecialization(
        SpecializableTypeGroupFilterInstanceCollection filters)
    {
        var defaultCase = filters.DefaultCase;

        if (defaultCase is null)
            return;

        foreach (var specializedCase in filters.SpecializedCases)
        {
            if (specializedCase.FilterType is FilterType.Exclusive)
            {
                systemDiagnostics.RegisterRedundantDefaultCaseByExclusiveSpecialization(
                    defaultCase.Identifier);
                return;
            }
        }
    }

    private void AnalyzeConstraintClauseReducibility()
    {
        if (!OnlyPermitSpecifiedTypes)
            return;

        var symbol = cachedTypeConstraintsByRule[TypeConstraintRule.PermitBaseType].OnlyOrDefault();
        if (symbol is null)
            return;

        if (symbol is not INamedTypeSymbol named)
            return;

        switch (named.TypeKind)
        {
            case TypeKind.Class when !named.IsSealed:
            case TypeKind.Interface:
                break;
            default:
                return;
        }

        if (named.IsUnboundGenericTypeSafe())
            return;

        foreach (var type in cachedTypeConstraintsByRule[TypeConstraintRule.PermitExactType])
        {
            if (!type.GetAllBaseTypesAndInterfaces().Contains(named, SymbolEqualityComparer.Default))
                return;
        }

        systemDiagnostics.RegisterReducibleToConstraintClauseType(named);
    }

    private void AnalyzeRedundantlyConstrainedTypes()
    {
        foreach (var rule in typeConstraintRules)
        {
            var type = rule.Key;
            var constraintRule = rule.Value.Rule;
            var isPermission = constraintRule is ConstraintRule.Permit;

            bool isRedundant = IsPermitted(type, out _, false) == isPermission;
            if (isRedundant)
                systemDiagnostics.RegisterRedundantlyConstrainedType(type, constraintRule);
        }
    }

    private void AnalyzeRedundantBoundUnboundRuleTypes()
    {
        foreach (var rule in typeConstraintRules)
        {
            var type = rule.Key;

            if (type is not INamedTypeSymbol named)
                continue;

            if (!named.IsBoundGenericTypeSafe())
                continue;

            var unbound = named.ConstructUnboundGenericType();
            if (!typeConstraintRules.ContainsKey(unbound))
                continue;

            var boundConstraintRule = rule.Value;
            var unboundConstraintRule = typeConstraintRules[unbound];

            if (unboundConstraintRule.FullySatisfies(boundConstraintRule))
                systemDiagnostics.RegisterRedundantBoundUnboundRuleType(named);
        }
    }
    #endregion

    /// <summary>Calculates the finite permitted type count.</summary>
    /// <param name="onlyEvaluateIfAny">If <see langword="true"/>, 0 will be returned if there are no type constraint rules.</param>
    /// <returns>Returns the count of finite permitted types. If there are unbound generic types, or non-exact type permission rules, this returns <see langword="null"/>, also affected by <paramref name="onlyEvaluateIfAny"/>.</returns>
    public int? GetFinitePermittedTypeCount(bool onlyEvaluateIfAny)
    {
        if (onlyEvaluateIfAny && typeConstraintRules.Count is 0)
            return 0;

        if (!OnlyPermitSpecifiedTypes)
            return null;

        int count = 0;

        foreach (var typeRule in typeConstraintRules)
        {
            var type = typeRule.Key;
            var rule = typeRule.Value;

            if (rule.Rule is ConstraintRule.Prohibit)
                continue;

            // There is no need to check whether the rule is a permission

            // Only exact permitted types can make for finite permitted type count
            if (rule.TypeReferencePoint is TypeConstraintReferencePoint.ExactType)
            {
                if (type is INamedTypeSymbol named && named.IsUnboundGenericTypeSafe())
                    return null;

                count++;
            }
            else
                return null;
        }

        return count;
    }

    public bool SupersetOf(TypeConstraintSystem other) => other.SubsetOf(this);
    public bool SubsetOf(TypeConstraintSystem other)
    {
        if (!OnlyPermitSpecifiedTypes)
            if (other.OnlyPermitSpecifiedTypes)
                return false;

        bool subsetFilters = Filters.IsSubsetOf(other.Filters);
        if (!subsetFilters)
            return false;

        foreach (var rule in other.typeConstraintRules)
        {
            switch (rule.Value.Rule)
            {
                // TODO: Evaluate correctness
                case ConstraintRule.Permit:
                    continue;
                case ConstraintRule.Prohibit:
                    if (IsPermitted(rule.Key))
                        return false;
                    break;
            }
        }

        return true;
    }

    public bool IsPermitted(ITypeParameterSymbol typeParameter, GenericTypeConstraintInfoCollection infos)
    {
        if (inheritedTypes.Contains(typeParameter))
            return true;

        var declaringElementTypeParameterSystems = infos[typeParameter.GetDeclaringSymbol()];
        var system = declaringElementTypeParameterSystems[typeParameter];
        return SupersetOf(system);
    }

    public bool IsPermitted(ITypeSymbol type) => IsPermitted(type, out _);
    public bool IsPermitted(
        ITypeSymbol type,
        out TypeConstraintRule? rule)
    {
        return IsPermitted(type, out rule, true);
    }

    private bool IsPermitted(
        ITypeSymbol type,
        out TypeConstraintRule? rule,
        bool checkInitialType)
    {
        rule = null;

        if (type is null)
            return false;

        var typeFilterPermission = CheckTypeGroupFilters(type);
        // Do not consider the type permitted immediately
        if (typeFilterPermission is PermissionResult.Prohibited)
            return false;

        if (OnlyPermitSpecifiedTypeGroups)
        {
            // Without explicit permission of the given type group,
            // immediately reject the type
            if (typeFilterPermission is PermissionResult.Unknown)
                return false;
        }

        var permission = IsPermittedWithUnbound(
            type,
            checkInitialType,
            out rule,
            TypeConstraintReferencePoint.ExactType,
            TypeConstraintReferencePoint.BaseType);

        if (permission is not PermissionResult.Unknown)
            return permission is PermissionResult.Permitted;

        var interfaceQueue = new Queue<INamedTypeSymbol>(type.Interfaces);
        while (interfaceQueue.Any())
        {
            var @interface = interfaceQueue.Dequeue();

            permission = IsPermittedWithUnbound(
                @interface,
                true,
                out rule,
                TypeConstraintReferencePoint.BaseType);
            if (permission is not PermissionResult.Unknown)
                return permission is PermissionResult.Permitted;

            foreach (var indirectInterface in @interface.Interfaces)
                interfaceQueue.Enqueue(indirectInterface);
        }

        type = type.BaseType;
        while (type != null)
        {
            permission = IsPermittedWithUnbound(
                type,
                true,
                out rule,
                TypeConstraintReferencePoint.BaseType);
            if (permission is not PermissionResult.Unknown)
                return permission is PermissionResult.Permitted;

            type = type.BaseType;
        }

        return !OnlyPermitSpecifiedTypes;
    }

    private PermissionResult CheckTypeGroupFilters(ITypeSymbol type)
    {
        var typeKind = type.TypeKind;

        bool explicitlyPermitted = false;

        var permission = GetFinalPermissionResult(
            type,
            ref explicitlyPermitted,
            [
                new(
                    static (type, typeKind)
                        => typeKind is TypeKind.Enum,
                    Filters.Enums
                ),
                new(
                    static (type, typeKind)
                        => typeKind is TypeKind.Delegate,
                    Filters.Delegates
                ),
                new(
                    static (type, typeKind)
                        => typeKind is TypeKind.Interface,
                    Filters.Interfaces
                ),
                new(
                    static (type, typeKind)
                        => typeKind is TypeKind.Class
                        && type.IsRecord,
                    Filters.RecordClasses
                ),
                new(
                    static (type, typeKind)
                        => typeKind is TypeKind.Struct
                        && type.IsRecord,
                    Filters.RecordStructs
                ),
                new(
                    static (type, typeKind)
                        => typeKind is TypeKind.Class
                        && type.IsAbstract,
                    Filters.AbstractClasses
                ),
                new(
                    static (type, typeKind)
                        => typeKind is TypeKind.Class
                        && type.IsSealed,
                    Filters.SealedClasses
                ),
            ]);

        if (permission is PermissionResult.Prohibited)
            return permission;

        // We assume that the rules have been validated beforehand and the appropriate
        // errors will have been reported to the user about mixing invalid specialized
        // generic arity and array rank filters
        int arity = type.GetArity();
        bool isGeneric = arity > 0;
        permission = CalculatePermissionResult(arity, isGeneric, Filters.GenericTypes);

        if (permission is PermissionResult.Prohibited)
            return permission;

        if (permission is PermissionResult.Permitted)
            explicitlyPermitted = true;

        var arrayType = type as IArrayTypeSymbol;
        bool isArray = arrayType is not null;
        int rank = arrayType?.Rank ?? 0;
        permission = CalculatePermissionResult(rank, isArray, Filters.Arrays);

        if (permission is PermissionResult.Prohibited)
            return permission;

        if (permission is PermissionResult.Permitted)
            explicitlyPermitted = true;

        if (explicitlyPermitted)
            return PermissionResult.Permitted;

        return PermissionResult.Unknown;
    }

    private PermissionResult CalculatePermissionResult(
        int value, bool matches,
        CaseCollectionFilters filters)
    {
        if (matches)
        {
            var filter = filters.FilterTypeForValue(value);
            return PermissionOfFilter(filter, true);
        }
        else
        {
            bool hasAnyExclusive = filters.HasAnyExclusiveCase;
            if (hasAnyExclusive)
                return PermissionResult.Prohibited;
        }

        return PermissionResult.Unknown;
    }

    private PermissionResult GetFinalPermissionResult(
        ITypeSymbol type,
        ref bool explicitlyPermitted,
        params FilterTypeAndMatcher[] filterMatchers)
    {
        var typeKind = type.TypeKind;
        foreach (var (matcher, filterType) in filterMatchers)
        {
            var result = matcher(type, typeKind);
            var permission = PermissionOfFilter(filterType, result);

            if (permission is PermissionResult.Prohibited)
                return permission;

            if (permission is PermissionResult.Permitted)
                explicitlyPermitted = true;
        }

        if (explicitlyPermitted)
            return PermissionResult.Permitted;

        return PermissionResult.Unknown;
    }

    private delegate bool FilterMatcher(ITypeSymbol type, TypeKind typeKind);

    private readonly record struct FilterTypeAndMatcher(
        FilterMatcher Matcher, FilterType Type);

    private PermissionResult PermissionOfFilter(FilterType type, bool matches)
    {
        if (type is FilterType.Exclusive)
            return matches ? PermissionResult.Permitted : PermissionResult.Prohibited;

        if (type is FilterType.Prohibited)
            return matches ? PermissionResult.Prohibited : PermissionResult.Unknown;

        if (type is FilterType.Permitted)
            return matches ? PermissionResult.Permitted : PermissionResult.Unknown;

        return PermissionResult.Unknown;
    }

    private PermissionResult IsPermittedWithUnbound(
        ITypeSymbol type,
        bool checkInitialType,
        out TypeConstraintRule? rule,
        params TypeConstraintReferencePoint[] referencePoints)
    {
        rule = null;

        PermissionResult permission;
        if (checkInitialType)
        {
            permission = IsPermitted(type, out rule, referencePoints);
            if (permission is not PermissionResult.Unknown)
                return permission;
        }

        if (type is INamedTypeSymbol namedType)
        {
            if (namedType.IsBoundGenericTypeSafe())
            {
                var unbound = namedType.ConstructUnboundGenericType();
                permission = IsPermitted(unbound, out rule, referencePoints);
                if (permission is not PermissionResult.Unknown)
                    return permission;
            }
        }

        return PermissionResult.Unknown;
    }

    private PermissionResult IsPermitted(
        ITypeSymbol type,
        out TypeConstraintRule? rule,
        params TypeConstraintReferencePoint[] referencePoints)
    {
        rule = null;
        bool contained = typeConstraintRules.TryGetValue(type, out var foundRule);
        if (!contained)
            return PermissionResult.Unknown;

        rule = foundRule;
        if (referencePoints.Contains(foundRule.TypeReferencePoint))
            return (PermissionResult)foundRule.Rule;

        return PermissionResult.Unknown;
    }

    public override string ToString()
    {
        return TypeParameter.ToDisplayString();
    }

    public static TypeConstraintSystem FromSymbol(ITypeSymbol typeSymbol)
    {
        return typeSymbol switch
        {
            ITypeParameterSymbol typeParameter => new TypeConstraintSystem(typeParameter),
            INamedTypeSymbol profileType => new TypeConstraintSystem(profileType),
            _ => null,
        };
    }

    public class DistinctGroupDictionary
    {
        private readonly HashSet<TypeConstraintProfileInfo> usedProfiles = new();
        private readonly Dictionary<TypeConstraintProfileGroupInfo, List<TypeConstraintProfileInfo>> distinctGroupUsages = new();

        // Why did I overengineer this again?
        /// <summary>Adds a profile's distinct groups to the dictionary, if it is not already added.</summary>
        /// <param name="profile">The profile whose distinct groups to add to the dictionary, linking them to the profile.</param>
        /// <returns><see langword="true"/> if all distinct groups were successfully added and are unique, otherwise <see langword="false"/>. In other words, <see langword="false"/> determines that a distinct group is used by more than one profile.</returns>
        public bool AddProfile(TypeConstraintProfileInfo profile)
        {
            if (usedProfiles.Contains(profile))
                return true;

            bool allDistinct = true;
            foreach (var group in profile.Groups)
            {
                if (!group.Distinct)
                    continue;

                allDistinct &= Add(group, profile);
            }
            return allDistinct;
        }

        private bool Add(TypeConstraintProfileGroupInfo distinctGroup, TypeConstraintProfileInfo usingProfile)
        {
            bool stillDistinct = !distinctGroupUsages.TryGetValue(distinctGroup, out var usageList);
            if (stillDistinct)
            {
                usageList = new();
                distinctGroupUsages.Add(distinctGroup, usageList);
            }

            usageList.Add(usingProfile);
            return stillDistinct;
        }

        public void Clear() => distinctGroupUsages.Clear();

        public bool ContainsDistinctGroup(TypeConstraintProfileGroupInfo distinctGroup) => distinctGroupUsages.ContainsKey(distinctGroup);

        public bool DetermineDistinctGroupUsage(TypeConstraintProfileGroupInfo distinctGroup) => GetDistinctGroupUsageCount(distinctGroup) < 2;
        public int GetDistinctGroupUsageCount(TypeConstraintProfileGroupInfo distinctGroup)
        {
            if (!distinctGroupUsages.TryGetValue(distinctGroup, out var value))
                return 0;
            return value.Count;
        }
        public IEnumerable<TypeConstraintProfileInfo> GetDistinctGroupUsageProfiles(TypeConstraintProfileGroupInfo distinctGroup) => distinctGroupUsages[distinctGroup];

        public ISet<INamedTypeSymbol> GetCollidingDistinctGroupProfileUsages()
        {
            return new HashSet<INamedTypeSymbol>(
                distinctGroupUsages.Values
                    .Where(static list => list.Count > 1)
                    .Flatten()
                    .Select(static info => info.ProfileDeclaringInterface),
                SymbolEqualityComparer.Default);
        }
    }

    public class Builder
    {
        private TypeConstraintSystem finalSystem;
        private TypeConstraintSystem inheritedTypeParameterSystems;
        private TypeConstraintSystem inheritedProfileSystems;

        private SystemBuildState buildState;

        public ITypeParameterSymbol TypeParameter => finalSystem.TypeParameter;
        public INamedTypeSymbol ProfileInterface => finalSystem.ProfileInterface;

        public TypeConstraintSystemDiagnostics SystemDiagnostics => finalSystem.SystemDiagnostics;

        // Flags will be accordingly adjusted for the new features' needs
        public bool OnlyPermitSpecifiedTypes
        {
            get => finalSystem.OnlyPermitSpecifiedTypes
                || inheritedTypeParameterSystems.OnlyPermitSpecifiedTypes
                || inheritedProfileSystems.OnlyPermitSpecifiedTypes;

            set => finalSystem.OnlyPermitSpecifiedTypes = value;
        }
        public bool OnlyPermitSpecifiedTypeGroups
        {
            get => finalSystem.OnlyPermitSpecifiedTypeGroups
                || inheritedTypeParameterSystems.OnlyPermitSpecifiedTypeGroups
                || inheritedProfileSystems.OnlyPermitSpecifiedTypeGroups;

            set => finalSystem.OnlyPermitSpecifiedTypeGroups = value;
        }
        public bool HasNoPermittedTypes
        {
            get
            {
                if (OnlyPermitSpecifiedTypes)
                {
                    return finalSystem.HasNoExplicitlyPermittedTypes
                        && inheritedTypeParameterSystems.HasNoExplicitlyPermittedTypes
                        && inheritedProfileSystems.HasNoExplicitlyPermittedTypes;
                }

                if (finalSystem.HasNoPermittedTypesFromTypeGroupFilters)
                {
                    return true;
                }

                return false;
            }
        }

        // IMPORTANT: For the time the filters are not inherited,
        // and will not be accounted for when merging the declarations
        public TypeGroupFilters Filters => finalSystem.Filters;

        public Builder(ITypeParameterSymbol typeParameter)
        {
            InitializeSystemsFromSymbol(typeParameter);
        }
        public Builder(INamedTypeSymbol profileType)
        {
            InitializeSystemsFromSymbol(profileType);
        }

        private void InitializeSystemsFromSymbol(ITypeSymbol typeSymbol)
        {
            finalSystem = FromSymbol(typeSymbol);
            inheritedTypeParameterSystems = FromSymbol(typeSymbol);
            inheritedProfileSystems = FromSymbol(typeSymbol);
        }

        public int? GetFinitePermittedTypeCount()
        {
            return finalSystem.GetFinitePermittedTypeCount(false)
                 + inheritedTypeParameterSystems.GetFinitePermittedTypeCount(true)
                 + inheritedProfileSystems.GetFinitePermittedTypeCount(true);
        }

        public bool InheritFrom(ITypeParameterSymbol baseTypeParameter, TypeConstraintSystem baseSystem)
        {
            return InheritFrom(baseTypeParameter, baseSystem, inheritedTypeParameterSystems, finalSystem.inheritedTypes);
        }
        public bool InheritFrom(ITypeParameterSymbol baseTypeParameter, Builder baseSystemBuilder)
        {
            return InheritFrom(baseTypeParameter, baseSystemBuilder, inheritedTypeParameterSystems, finalSystem.inheritedTypes);
        }

        public bool InheritFrom(TypeConstraintProfileInfo profileInfo)
        {
            finalSystem.inheritedProfileInfos.Add(profileInfo.ProfileDeclaringInterface, profileInfo);
            // Do not report the diagnostic yet, allow every other inheritance to take place
            // and then mark all colliding profiles' distinct groups
            finalSystem.inheritedProfileDistinctGroups.AddProfile(profileInfo);

            return InheritFrom(profileInfo.ProfileDeclaringInterface, profileInfo.Builder, inheritedProfileSystems, finalSystem.inheritedProfiles);
        }

        private bool InheritFrom<T>(T baseType, TypeConstraintSystem baseSystem, TypeConstraintSystem inheritedSystems, ISet<T> inheritedTypes)
            where T : ITypeSymbol
        {
            if (buildState is SystemBuildState.FinalizedWhole)
                return false;

            inheritedSystems.OnlyPermitSpecifiedTypes |= baseSystem.OnlyPermitSpecifiedTypes;

            bool independent = inheritedSystems.typeConstraintRules.TryAddPreserveRange(baseSystem.typeConstraintRules);
            if (!independent)
                inheritedSystems.systemDiagnostics.RegisterConflictingInheritedSymbol(baseType);

            inheritedTypes.Add(baseType);

            return independent;
        }
        private bool InheritFrom<T>(T baseType, Builder baseSystemBuilder, TypeConstraintSystem affectedInheritedSystems, ISet<T> inheritedTypes)
            where T : ITypeSymbol
        {
            if (buildState is SystemBuildState.FinalizedWhole)
                return false;

            affectedInheritedSystems.OnlyPermitSpecifiedTypes |= baseSystemBuilder.OnlyPermitSpecifiedTypes;

            // Prefer & over && to always add the type constraint rules
            bool independent = affectedInheritedSystems.typeConstraintRules.TryAddPreserveRange(baseSystemBuilder.inheritedTypeParameterSystems.typeConstraintRules)
                & affectedInheritedSystems.typeConstraintRules.TryAddPreserveRange(baseSystemBuilder.inheritedProfileSystems.typeConstraintRules)
                & affectedInheritedSystems.typeConstraintRules.TryAddPreserveRange(baseSystemBuilder.finalSystem.typeConstraintRules);
            if (!independent)
                affectedInheritedSystems.systemDiagnostics.RegisterConflictingInheritedSymbol(baseType);

            inheritedTypes.Add(baseType);

            return independent;
        }

        public void Add(TypeConstraintRule rule, params ITypeSymbol[] types)
        {
            Add(rule, (IEnumerable<ITypeSymbol>)types);
        }

        public void Add(TypeConstraintRule rule, IEnumerable<ITypeSymbol> types)
        {
            if (buildState.HasFlag(SystemBuildState.FinalizedBase))
                return;

            var systemDiagnostics = finalSystem.systemDiagnostics;
            var typeConstraintRules = finalSystem.typeConstraintRules;

            foreach (var t in types)
            {
                if (systemDiagnostics.ConditionallyRegisterInvalidTypeArgumentType(t))
                    continue;

                if (TypeParameter != null)
                {
                    var registered = systemDiagnostics.ConditionallyRegisterConstrainedSubstitutionType(
                        TypeParameter, t,
                        rule.TypeReferencePoint is TypeConstraintReferencePoint.BaseType);

                    if (registered)
                        continue;
                }

                if (typeConstraintRules.ContainsKey(t))
                {
                    if (typeConstraintRules[t] == rule)
                        systemDiagnostics.RegisterDuplicateType(t);
                    else
                        systemDiagnostics.RegisterConflictingType(t);

                    continue;
                }

                var localRule = rule;

                if (systemDiagnostics.ConditionallyRegisterRedundantBaseTypeRuleType(t, rule))
                    localRule.TypeReferencePoint = TypeConstraintReferencePoint.ExactType;

                typeConstraintRules.Add(t, localRule);
            }
        }

        public TypeConstraintSystemDiagnostics AnalyzeFinalizedBaseSystem()
        {
            if (buildState.HasFlag(SystemBuildState.FinalizedBase))
                return SystemDiagnostics;

            buildState |= SystemBuildState.FinalizedBase;
            return finalSystem.AnalyzeFinalizedSystem();
        }

        public void AnalyzeFinalizedInheritedTypeProfiles()
        {
            if (buildState.HasFlag(SystemBuildState.FinalizedInheritedProfiles))
                return;

            var multipleDistinctGroupProfiles = finalSystem.inheritedProfileDistinctGroups.GetCollidingDistinctGroupProfileUsages();
            finalSystem.systemDiagnostics.RegisterMultipleOfDistinctGroupInheritedProfiles(multipleDistinctGroupProfiles);

            buildState |= SystemBuildState.FinalizedInheritedProfiles;
        }

        public TypeConstraintSystem FinalizeSystem()
        {
            if (buildState is SystemBuildState.FinalizedWhole)
                return finalSystem;

            AnalyzeFinalizedBaseSystem();

            AnalyzeFinalizedInheritedTypeProfiles();

            // Copy inherited rules to the final system
            finalSystem.typeConstraintRules.TryAddPreserveRange(inheritedTypeParameterSystems.typeConstraintRules);
            finalSystem.typeConstraintRules.TryAddPreserveRange(inheritedProfileSystems.typeConstraintRules);
            // The flags system will be improved:tm:
            finalSystem.OnlyPermitSpecifiedTypes
                |= inheritedTypeParameterSystems.OnlyPermitSpecifiedTypes
                || inheritedProfileSystems.OnlyPermitSpecifiedTypes;

            // The system diagnostics for base type systems are not copied over since they will have already appeared

            buildState = SystemBuildState.FinalizedWhole;

            return finalSystem;
        }

        [Flags]
        private enum SystemBuildState : uint
        {
            Building,
            FinalizedBase = 1,
            FinalizedInheritedTypeParameters = 1 << 1,
            FinalizedInheritedProfiles = 1 << 2,

            FinalizedWhole = FinalizedBase
                | FinalizedInheritedTypeParameters
                | FinalizedInheritedProfiles,
        }
    }

    public sealed class TypeGroupFilters
    {
        public FilterType Interfaces { get; set; }
        public FilterType Delegates { get; set; }
        public FilterType Enums { get; set; }
        public FilterType AbstractClasses { get; set; }
        public FilterType SealedClasses { get; set; }
        public FilterType RecordClasses { get; set; }
        public FilterType RecordStructs { get; set; }
        public CaseCollectionFilters GenericTypes { get; set; } = new();
        public CaseCollectionFilters Arrays { get; set; } = new();

        public bool HasAnyExplicitlyPermittedGroup
        {
            get
            {
                return Interfaces is FilterType.Permitted
                    || Delegates is FilterType.Permitted
                    || Enums is FilterType.Permitted
                    || AbstractClasses is FilterType.Permitted
                    || SealedClasses is FilterType.Permitted
                    || RecordClasses is FilterType.Permitted
                    || RecordStructs is FilterType.Permitted
                    || GenericTypes.HasAnyExplicitlyPermittedCase
                    || Arrays.HasAnyExplicitlyPermittedCase
                    ;
            }
        }

        public FilterableTypeGroups AllExclusiveFilteredTypeGroups()
        {
            var groups = FilterableTypeGroups.None;

            if (Interfaces is FilterType.Exclusive)
                groups |= FilterableTypeGroups.Interface;
            if (Delegates is FilterType.Exclusive)
                groups |= FilterableTypeGroups.Delegate;
            if (Enums is FilterType.Exclusive)
                groups |= FilterableTypeGroups.Enum;
            if (AbstractClasses is FilterType.Exclusive)
                groups |= FilterableTypeGroups.AbstractClass;
            if (SealedClasses is FilterType.Exclusive)
                groups |= FilterableTypeGroups.SealedClass;
            if (RecordClasses is FilterType.Exclusive)
                groups |= FilterableTypeGroups.RecordClass;
            if (RecordStructs is FilterType.Exclusive)
                groups |= FilterableTypeGroups.RecordStruct;
            if (GenericTypes.HasAnyExclusiveCase)
                groups |= FilterableTypeGroups.Generic;
            if (Arrays.HasAnyExclusiveCase)
                groups |= FilterableTypeGroups.Array;

            return groups;
        }

        public FilterableTypeGroups AllPermittedFilteredTypeGroups()
        {
            var groups = FilterableTypeGroups.None;

            if (Interfaces is FilterType.Permitted)
                groups |= FilterableTypeGroups.Interface;
            if (Delegates is FilterType.Permitted)
                groups |= FilterableTypeGroups.Delegate;
            if (Enums is FilterType.Permitted)
                groups |= FilterableTypeGroups.Enum;
            if (AbstractClasses is FilterType.Permitted)
                groups |= FilterableTypeGroups.AbstractClass;
            if (SealedClasses is FilterType.Permitted)
                groups |= FilterableTypeGroups.SealedClass;
            if (RecordClasses is FilterType.Permitted)
                groups |= FilterableTypeGroups.RecordClass;
            if (RecordStructs is FilterType.Permitted)
                groups |= FilterableTypeGroups.RecordStruct;
            if (GenericTypes.HasAnyExplicitlyPermittedCase)
                groups |= FilterableTypeGroups.Generic;
            if (Arrays.HasAnyExplicitlyPermittedCase)
                groups |= FilterableTypeGroups.Array;

            return groups;
        }

        public FilterableTypeGroups AllPermittedOrExclusiveFilteredTypeGroups()
        {
            return AllPermittedFilteredTypeGroups()
                | AllExclusiveFilteredTypeGroups();
        }

        public bool IsSubsetOf(TypeGroupFilters other)
        {
            return other.IsSupersetOf(this);
        }
        public bool IsSupersetOf(TypeGroupFilters other)
        {
            return FilterIsSupersetOf(Interfaces, other.Interfaces)
                && FilterIsSupersetOf(Delegates, other.Delegates)
                && FilterIsSupersetOf(Enums, other.Enums)
                && FilterIsSupersetOf(AbstractClasses, other.AbstractClasses)
                && FilterIsSupersetOf(SealedClasses, other.SealedClasses)
                && FilterIsSupersetOf(RecordClasses, other.RecordClasses)
                && FilterIsSupersetOf(RecordStructs, other.RecordStructs)
                && GenericTypes.IsSupersetOf(other.GenericTypes)
                && Arrays.IsSupersetOf(other.Arrays)
                ;
        }

        public IReadOnlyList<TypeGroupFilterInstance> GetFilterInstances()
        {
            var result = new List<TypeGroupFilterInstance>();

            Add(BasicTypeGroupFilter.Interface, Interfaces);
            Add(BasicTypeGroupFilter.Delegate, Delegates);
            Add(BasicTypeGroupFilter.Enum, Enums);
            Add(BasicTypeGroupFilter.AbstractClass, AbstractClasses);
            Add(BasicTypeGroupFilter.SealedClass, SealedClasses);
            Add(BasicTypeGroupFilter.RecordClass, RecordClasses);
            Add(BasicTypeGroupFilter.RecordStruct, RecordStructs);

            Add(GenericTypeGroupFilter.Default, GenericTypes.Default);
            Add(ArrayTypeGroupFilter.Default, Arrays.Default);

            foreach (var other in GenericTypes.Remaining)
            {
                var (value, filterType) = other.Value;
                if (filterType is FilterType.None)
                    continue;

                var identifier = new GenericTypeGroupFilter(value);
                Add(identifier, filterType);
            }

            foreach (var other in Arrays.Remaining)
            {
                var (value, filterType) = other.Value;
                if (filterType is FilterType.None)
                    continue;

                var identifier = new ArrayTypeGroupFilter(value);
                Add(identifier, filterType);
            }

            return result;

            void Add(ITypeGroupFilterIdentifier identifier, FilterType filterType)
            {
                if (filterType is FilterType.None)
                    return;

                result.Add(new(identifier, filterType));
            }
        }
    }

    // TODO: Perhaps make this an extension
    public static bool FilterIsSupersetOf(FilterType left, FilterType right)
    {
        switch (left)
        {
            case FilterType.None:
                return true;

            case FilterType.Prohibited:
                return right
                    is FilterType.Prohibited;

            case FilterType.Exclusive:
                return right
                    is FilterType.Exclusive;

            case FilterType.Permitted:
                return right
                    is FilterType.Permitted
                    or FilterType.Exclusive;

            // Invalid values are simply ignored
            default:
                return false;
        }
    }

    public class CaseCollectionFilters
    {
        // This structure, although slightly questionable, is reasonable and
        // helpful for the IsSubsetOf and IsSupersetOf implementations

        public FilterType Default { get; private set; } = FilterType.None;

        // We should not need anything too specialized for this one
        public SortedList<uint, SpecificCaseFilter> Remaining { get; } = [];

        private readonly SortedList<uint?, int> _setterCounters = [];

        public bool HasAnyExplicitlyPermittedCase
        {
            get
            {
                return Default is FilterType.Permitted
                    || Remaining.Values.Any(static s => s.FilterType is FilterType.Permitted);
            }
        }

        public bool HasAnyExclusiveCase
        {
            get
            {
                return Default is FilterType.Exclusive
                    || Remaining.Values.Any(static s => s.FilterType is FilterType.Exclusive);
            }
        }

        public IReadOnlyList<uint?> GetDuplicateSpecializationValues()
        {
            return _setterCounters
                .Where(s => s.Value > 1)
                .Select(Selectors.KeyReturner)
                .ToArray();
        }

        private void RegisterValueAssignment(uint? value)
        {
            int counter = _setterCounters.ValueOrDefault(value);
            _setterCounters[value] = counter + 1;
        }

        public void Set(uint? value, FilterType filterType)
        {
            Set(new(value, filterType));
        }
        public void Set(SpecificCaseFilter filter)
        {
            var value = filter.Value;
            if (value is null)
            {
                Default = filter.FilterType;
                return;
            }

            Remaining[value!.Value] = filter;
            RegisterValueAssignment(value);
        }

        public FilterType FilterTypeForValue(int value)
        {
            return FilterTypeForValue((uint)value);
        }
        public FilterType FilterTypeForValue(uint value)
        {
            bool found = Remaining.TryGetValue(value, out var filter);
            if (found)
                return filter.FilterType;

            return Default;
        }

        public bool IsSubsetOf(CaseCollectionFilters other)
        {
            return other.IsSupersetOf(this);
        }
        public bool IsSupersetOf(CaseCollectionFilters other)
        {
            var defaultLeft = Default;
            var defaultRight = other.Default;
            var isSuperset = FilterIsSupersetOf(defaultLeft, defaultRight);
            if (!isSuperset)
                return false;

            var allNumbers = Remaining.Keys.ToSetOrExisting();
            allNumbers.AddRange(other.Remaining.Keys);

            foreach (var number in allNumbers)
            {
                Remaining.TryGetValue(number, out var leftFilter);
                var left = leftFilter.FilterType;

                other.Remaining.TryGetValue(number, out var rightFilter);
                var right = rightFilter.FilterType;

                isSuperset = FilterIsSupersetOf(defaultLeft, defaultRight);
                if (!isSuperset)
                    return false;
            }

            return true;
        }
    }

    public record struct SpecificCaseFilter(uint? Value, FilterType FilterType);

    public class EqualityComparer : IEqualityComparer<TypeConstraintSystem>
    {
        public static readonly EqualityComparer Default = new();

        private EqualityComparer() { }

        public bool Equals(TypeConstraintSystem a, TypeConstraintSystem b)
        {
            return SymbolEqualityComparer.Default.Equals(a.TypeParameter, b.TypeParameter);
        }

        public int GetHashCode(TypeConstraintSystem system)
        {
            return SymbolEqualityComparer.Default.GetHashCode(system.TypeParameter);
        }
    }
}
