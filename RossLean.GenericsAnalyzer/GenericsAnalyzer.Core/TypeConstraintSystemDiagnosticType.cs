namespace RossLean.GenericsAnalyzer.Core;

public enum TypeConstraintSystemDiagnosticType
{
    Valid,
    Conflicting,
    Duplicate,
    InvalidTypeArgument,
    ConstrainedTypeArgumentSubstitution,
    RedundantlyPermitted,
    RedundantlyProhibited,
    ReducibleToConstraintClause,
    RedundantBaseTypeRule,
    RedundantBoundUnboundRule,
}
