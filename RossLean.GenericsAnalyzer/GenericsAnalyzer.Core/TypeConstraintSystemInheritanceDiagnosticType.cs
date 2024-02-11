namespace RossLean.GenericsAnalyzer.Core;

public enum TypeConstraintSystemInheritanceDiagnosticType
{
    Safe,
    Conflicting,
    // Although currently only used in type constraint profiles, this feature can
    // apply in any context of inheritance
    MultipleOfDistinctGroup,
}
