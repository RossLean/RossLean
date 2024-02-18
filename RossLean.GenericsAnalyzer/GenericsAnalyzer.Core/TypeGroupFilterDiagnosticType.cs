namespace RossLean.GenericsAnalyzer.Core;

public enum TypeGroupFilterDiagnosticType
{
    Safe,
    UnavailablePermissionByExclusion,
    RedundantProhibitionByExclusion,
    UnavailablePermissionByGenericConstraints,
    IncompatibleExclusiveFilter,
    IneffectiveFilterWithNoPermittedBaseTypes,
    RedundantSpecialization,
    DuplicateSpecialization,
    ConflictingExclusiveSpecialization,
    RedundantDefaultCaseByExclusiveSpecialization,
}
