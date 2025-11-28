using System.Collections.Generic;
using System.Linq;

public static class WaveInfoHelper
{
    public static float Calculate(this IEnumerable<WaveInfo> waveInfos, float x)
    {
        var value = 0f;
        var waveInfosArray = waveInfos.ToArray();
        foreach (var waveInfo in waveInfosArray)
            value += waveInfo.Calculate(x);

        return value / waveInfosArray.Length;
    }

    public static TWaveInfo FirstWaveTrait<TWaveInfo>(this IEnumerable<TWaveInfo> waveInfos, WaveTrait waveTrait)
        where TWaveInfo : WaveInfo
    {
        return waveInfos.First(x => x.WaveType.ToWaveTrait() == waveTrait);
    }
}