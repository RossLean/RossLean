namespace RossLean.Common.Base;

public static class BitOperationsEx
{
    /// <summary>
    /// Implements the POPCNT with bitwise operations, without using
    /// the hardware instruction, to support .NET Standard 2.0.
    /// </summary>
    /// <param name="i">The value whose bit count to get.</param>
    /// <returns>
    /// The number of bits set to 1 in the given value.
    /// </returns>
    // This is taken from the original C++ version
    public static int DirtyPopCount(int i)
    {
        i -= i >> 1 & 0x55555555;
        i = (i & 0x33333333) + (i >> 2 & 0x33333333);
        i = i + (i >> 4) & 0x0F0F0F0F;
        i *= 0x01010101;
        return i >> 24;
    }
}
