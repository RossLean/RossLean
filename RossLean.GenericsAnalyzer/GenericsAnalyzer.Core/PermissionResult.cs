namespace RossLean.GenericsAnalyzer.Core;

public enum PermissionResult
{
    Permitted = ConstraintRule.Permit,
    Prohibited = ConstraintRule.Prohibit,
    Unknown,
}
