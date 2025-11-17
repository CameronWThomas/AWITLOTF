using System.Collections;
using System.Collections.Generic;

public enum WaveType
{
    Sin = 1,
    Cos = 1 << 1,
    Saw = 1 << 2,
}

public static class WaveTypeExtensions
{
    public static int GetMask(this IEnumerable<WaveType> waveTypes)
    {
        var mask = 0;
        foreach (var waveType in waveTypes)
            mask |= (int)waveType;

        return mask;
    }
}