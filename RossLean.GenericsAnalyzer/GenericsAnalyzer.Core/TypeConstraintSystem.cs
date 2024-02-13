using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;

// Aliases like that are great
using RuleEqualityComparer = Func<KeyValuePair<ITypeSymbol, TypeConstraintRule>, bool>;

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
    public bool HasNoExplicitlyPermittedTypes => !typeConstraintRules.Any(GetRuleEqualityComparer(ConstraintRule.Permit));
    public bool HasNoPermittedTypes => OnlyPermitSpecifiedTypes && HasNoExplicitlyPermittedTypes;

    private TypeConstraintSystemDiagnostics AnalyzeFinalizedSystem()
    {
        cachedTypeConstraintsByRule = TypeConstraintsByRule;
        AnalyzeRedundantlyConstrainedTypes();
        AnalyzeConstraintClauseReducibility();
        AnalyzeRedundantBoundUnboundRuleTypes();
        return SystemDiagnostics;
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

        foreach (var rule in other.typeConstraintRules)
        {
            switch (rule.Value.Rule)
            {
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
                    .Where(list => list.Count > 1)
                    .Flatten()
                    .Select(info => info.ProfileDeclaringInterface),
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
        public bool HasNoPermittedTypes
            => OnlyPermitSpecifiedTypes
            && finalSystem.HasNoExplicitlyPermittedTypes
            && inheritedTypeParameterSystems.HasNoExplicitlyPermittedTypes
            && inheritedProfileSystems.HasNoExplicitlyPermittedTypes;

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
