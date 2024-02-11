using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using RoseLynn;
using RoseLynn.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RossLean.GenericsAnalyzer.Core;

using DiagnosticType = TypeConstraintSystemDiagnosticType;
using InheritanceDiagnosticType = TypeConstraintSystemInheritanceDiagnosticType;

public sealed class TypeConstraintSystemDiagnostics
{
    private readonly ErroneousElementDictionary<DiagnosticType, ITypeSymbol> erroneousTypes;
    private readonly ErroneousElementDictionary<InheritanceDiagnosticType, ITypeParameterSymbol> erroneousInheritedTypeParameters;
    private readonly ErroneousElementDictionary<InheritanceDiagnosticType, INamedTypeSymbol> erroneousInheritedProfiles;

    // DiagnosticType
    private ISet<ITypeSymbol> ConflictingTypes => erroneousTypes[DiagnosticType.Conflicting];
    private ISet<ITypeSymbol> DuplicateTypes => erroneousTypes[DiagnosticType.Duplicate];
    private ISet<ITypeSymbol> InvalidTypeArgumentTypes => erroneousTypes[DiagnosticType.InvalidTypeArgument];
    private ISet<ITypeSymbol> ConstrainedTypeArgumentSubstitutionTypes => erroneousTypes[DiagnosticType.ConstrainedTypeArgumentSubstitution];
    private ISet<ITypeSymbol> RedundantlyPermittedTypes => erroneousTypes[DiagnosticType.RedundantlyPermitted];
    private ISet<ITypeSymbol> RedundantlyProhibitedTypes => erroneousTypes[DiagnosticType.RedundantlyProhibited];
    private ISet<ITypeSymbol> ReducibleToConstraintClauseTypes => erroneousTypes[DiagnosticType.ReducibleToConstraintClause];
    private ISet<ITypeSymbol> RedundantBaseTypeRuleTypes => erroneousTypes[DiagnosticType.RedundantBaseTypeRule];
    private ISet<ITypeSymbol> RedundantBoundUnboundRuleTypes => erroneousTypes[DiagnosticType.RedundantBoundUnboundRule];

    // InheritanceDiagnosticType - Type Parameters
    private ISet<ITypeParameterSymbol> ConflictingInheritedTypeParameters => erroneousInheritedTypeParameters[InheritanceDiagnosticType.Conflicting];

    // InheritanceDiagnosticType - Profiles
    private ISet<INamedTypeSymbol> ConflictingInheritedProfiles => erroneousInheritedProfiles[InheritanceDiagnosticType.Conflicting];
    private ISet<INamedTypeSymbol> MultipleOfDistinctGroupInheritedProfiles => erroneousInheritedProfiles[InheritanceDiagnosticType.MultipleOfDistinctGroup];

    public bool HasErroneousTypes => erroneousTypes.Values.AnyDeep();
    public bool HasErroneousInheritedTypeParameters => erroneousInheritedTypeParameters.Values.AnyDeep();
    public bool HasErroneousInheritedProfiles => erroneousInheritedProfiles.Values.AnyDeep();

    public TypeConstraintSystemDiagnostics()
    {
        erroneousTypes = new ErroneousElementDictionary<DiagnosticType, ITypeSymbol>();
        erroneousInheritedTypeParameters = new ErroneousElementDictionary<InheritanceDiagnosticType, ITypeParameterSymbol>();
        erroneousInheritedProfiles = new ErroneousElementDictionary<InheritanceDiagnosticType, INamedTypeSymbol>();
    }
    public TypeConstraintSystemDiagnostics(TypeConstraintSystemDiagnostics other)
        : this()
    {
        foreach (var kvp in other.erroneousTypes)
            erroneousTypes[kvp.Key].AddRange(kvp.Value);

        foreach (var kvp in other.erroneousInheritedTypeParameters)
            erroneousInheritedTypeParameters[kvp.Key].AddRange(kvp.Value);

        foreach (var kvp in other.erroneousInheritedProfiles)
            erroneousInheritedProfiles[kvp.Key].AddRange(kvp.Value);
    }

    public ISet<ITypeSymbol> GetTypesForDiagnosticType(DiagnosticType diagnosticType)
    {
        return new HashSet<ITypeSymbol>(erroneousTypes[diagnosticType], SymbolEqualityComparer.Default);
    }
    public ISet<ITypeParameterSymbol> GetTypeParametersForDiagnosticType(InheritanceDiagnosticType diagnosticType)
    {
        return new HashSet<ITypeParameterSymbol>(erroneousInheritedTypeParameters[diagnosticType], SymbolEqualityComparer.Default);
    }
    public ISet<ITypeSymbol> GetProfilesForDiagnosticType(InheritanceDiagnosticType diagnosticType)
    {
        return new HashSet<ITypeSymbol>(erroneousInheritedProfiles[diagnosticType], SymbolEqualityComparer.Default);
    }

    public void RegisterDiagnostics(TypeConstraintSystemDiagnostics typeDiagnostics)
    {
        RegisterDuplicateTypes(typeDiagnostics.DuplicateTypes);
        RegisterConflictingTypes(typeDiagnostics.ConflictingTypes);

        foreach (var kvp in typeDiagnostics.erroneousTypes)
        {
            // This should avoid directly adding elements that were previously handled
            switch (kvp.Key)
            {
                case DiagnosticType.Conflicting:
                case DiagnosticType.Duplicate:
                    continue;
            }

            erroneousTypes[kvp.Key].AddRange(kvp.Value);
        }

        erroneousInheritedTypeParameters.AddRange(typeDiagnostics.erroneousInheritedTypeParameters);
        erroneousInheritedProfiles.AddRange(typeDiagnostics.erroneousInheritedProfiles);
    }

    #region Register Type Diagnostics
    // Talk about a clusterfuck
    public void RegisterConflictingType(ITypeSymbol type)
    {
        if (DuplicateTypes.Contains(type))
            DuplicateTypes.Remove(type);

        ConflictingTypes.Add(type);
    }
    public void RegisterDuplicateType(ITypeSymbol type)
    {
        DuplicateTypes.Add(type);
    }
    public bool ConditionallyRegisterInvalidTypeArgumentType(ITypeSymbol type)
    {
        bool invalid = type.IsInvalidTypeArgument();
        if (invalid)
            InvalidTypeArgumentTypes.Add(type);
        return invalid;
    }
    public bool ConditionallyRegisterConstrainedSubstitutionType(ITypeParameterSymbol typeParameter, ITypeSymbol type, bool evaluateAsBase)
    {
        bool invalid = !typeParameter.IsValidTypeArgumentSubstitution(type, evaluateAsBase);
        if (invalid)
            ConstrainedTypeArgumentSubstitutionTypes.Add(type);
        return invalid;
    }
    public bool ConditionallyRegisterRedundantBaseTypeRuleType(ITypeSymbol type, TypeConstraintRule constraintRule)
    {
        bool redundant = constraintRule.TypeReferencePoint is TypeConstraintReferencePoint.BaseType && type.IsSealed;
        if (redundant)
            RedundantBaseTypeRuleTypes.Add(type);
        return redundant;
    }
    public void RegisterReducibleToConstraintClauseType(INamedTypeSymbol type)
    {
        ReducibleToConstraintClauseTypes.Add(type);
    }
    public void RegisterRedundantlyConstrainedType(ITypeSymbol type, ConstraintRule rule)
    {
        erroneousTypes[GetDiagnosticType(rule)].Add(type);
    }
    public void RegisterRedundantlyPermittedType(ITypeSymbol type)
    {
        RedundantlyPermittedTypes.Add(type);
    }
    public void RegisterRedundantlyProhibitedType(ITypeSymbol type)
    {
        RedundantlyProhibitedTypes.Add(type);
    }
    public void RegisterRedundantBoundUnboundRuleType(INamedTypeSymbol type)
    {
        RedundantlyPermittedTypes.Remove(type);
        RedundantlyProhibitedTypes.Remove(type);
        RedundantBoundUnboundRuleTypes.Add(type);
    }

    public void RegisterConflictingTypes(ISet<ITypeSymbol> types)
    {
        foreach (var type in types)
            RegisterConflictingType(type);
    }
    public void RegisterDuplicateTypes(ISet<ITypeSymbol> types)
    {
        DuplicateTypes.AddRange(types);
    }
    #endregion

    #region Register Inherited Type Diagnostics
    public void RegisterConflictingInheritedSymbol(ITypeSymbol type)
    {
        switch (type)
        {
            case ITypeParameterSymbol typeParameter:
                RegisterConflictingInheritedTypeParameter(typeParameter);
                break;

            case INamedTypeSymbol profileType:
                RegisterConflictingInheritedProfile(profileType);
                break;
        }
    }

    public void RegisterConflictingInheritedTypeParameter(ITypeParameterSymbol typeParameter)
    {
        ConflictingInheritedTypeParameters.Add(typeParameter);
    }

    public void RegisterConflictingInheritedProfile(INamedTypeSymbol profile)
    {
        ConflictingInheritedProfiles.Add(profile);
    }
    public void RegisterMultipleOfDistinctGroupInheritedProfile(INamedTypeSymbol profile)
    {
        MultipleOfDistinctGroupInheritedProfiles.Add(profile);
    }

    // TODO: More overloads should exist for that purpose
    public void RegisterMultipleOfDistinctGroupInheritedProfiles(IEnumerable<INamedTypeSymbol> profiles)
    {
        MultipleOfDistinctGroupInheritedProfiles.AddRange(profiles);
    }
    #endregion

    public DiagnosticType GetDiagnosticType(ITypeSymbol type)
    {
        return erroneousTypes.GetDiagnosticType(type);
    }
    public InheritanceDiagnosticType GetInheritanceDiagnosticType(ITypeParameterSymbol type)
    {
        return erroneousInheritedTypeParameters.GetDiagnosticType(type);
    }
    public InheritanceDiagnosticType GetInheritanceDiagnosticProfile(INamedTypeSymbol profile)
    {
        return erroneousInheritedProfiles.GetDiagnosticType(profile);
    }

    private static HashSet<T> NewSymbolHashSet<T>()
        where T : class, ISymbol
    {
        return new HashSet<T>(comparer: SymbolEqualityComparer.Default);
    }

    private static DiagnosticType GetDiagnosticType(ConstraintRule rule)
    {
        return rule switch
        {
            ConstraintRule.Permit => DiagnosticType.RedundantlyPermitted,
            ConstraintRule.Prohibit => DiagnosticType.RedundantlyProhibited,
            _ => throw new InvalidEnumArgumentException(),
        };
    }

    // Aliasing is not possible just yet
    private class ErroneousElementDictionary<TDiagnosticType, TElement> : Dictionary<TDiagnosticType, ISet<TElement>>
        where TDiagnosticType : struct, Enum
    {
        private static readonly TDiagnosticType[] diagnosticTypes = EnumHelpers.GetValues<TDiagnosticType>();

        public ErroneousElementDictionary()
        {
            foreach (var type in diagnosticTypes)
                Add(type, new HashSet<TElement>());

            Remove(default);
        }

        // There is no need to check for the key's value because the valid value is the default value
        public TDiagnosticType GetDiagnosticType(TElement type) => this.FirstOrDefault(kvp => kvp.Value.Contains(type)).Key;
    }
}
