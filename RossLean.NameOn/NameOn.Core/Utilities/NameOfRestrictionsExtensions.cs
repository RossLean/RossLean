namespace RossLean.NameOn.Core.Utilities;

public static class NameOfRestrictionsExtensions
{
    public static bool IsEnforcement(this NameOfRestrictions restrictions)
    {
        return restrictions is > NameOfRestrictions.None and < NameOfRestrictions.Prohibit;
    }
    public static bool IsProhibition(this NameOfRestrictions restrictions)
    {
        return restrictions is >= NameOfRestrictions.Prohibit;
    }
}
