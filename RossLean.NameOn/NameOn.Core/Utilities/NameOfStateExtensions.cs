namespace RossLean.NameOn.Core.Utilities;

public static class NameOfStateExtensions
{
    public static bool ValidForRestrictions(this NameOfState state, NameOfRestrictions restrictions)
    {
        if (restrictions is NameOfRestrictions.None)
            return true;

        return state switch
        {
            NameOfState.None => restrictions.IsProhibition(),
            NameOfState.Contained => restrictions is NameOfRestrictions.ForceContained,
            NameOfState.Whole => restrictions is NameOfRestrictions.Force,
            _ => false,
        };
    }
}
