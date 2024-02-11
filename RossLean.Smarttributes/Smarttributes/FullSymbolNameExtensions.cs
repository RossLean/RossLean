using RoseLynn;

namespace RossLean.Smarttributes;

public static class FullSymbolNameExtensions
{
    public static bool MatchesAny(
        this FullSymbolName fullName,
        SymbolNameMatchingLevel matchingLevel,
        params FullSymbolName[] others)
    {
        foreach (var other in others)
        {
            bool matches = fullName.Matches(other, matchingLevel);
            if (matches)
                return true;
        }
        return false;
    }
}
