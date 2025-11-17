using System;
using System.Linq;

public delegate float WaveFunction(float x, float v);

public class WaveInfo
{
    public WaveInfo(params WaveType[] waveTypes)
    {
        WaveTypes = waveTypes ?? Array.Empty<WaveType>();
    }

    public static WaveInfo[] GetRandoms(int count)
    {
        var waveTypes = EnumHelper.GetValues<WaveType>().Randomize();
        var maxCount = waveTypes.Count();
        if (count < 0 || count > maxCount)
            throw new ArgumentException($"{nameof(count)} must be between [0, {maxCount}] ", nameof(count));

        return waveTypes.Take(count).Select(x => new WaveInfo(x)).ToArray();
    }

    public float VariableValue { get; set; } = 1f;
    public WaveType[] WaveTypes { get; }

    public WaveInfo Copy() => new WaveInfo(WaveTypes) { VariableValue = VariableValue };
}

public class OLD_WaveInfo
{
    private const float DefaultVariableValue = 1f;

    public OLD_WaveInfo(WaveType waveType, WaveFunction waveFunction)
    {
        WaveType = waveType;
        WaveFunction = waveFunction;
    }

    public WaveType WaveType { get; }
    public WaveFunction WaveFunction { get; }
    public float VariableValue { get; set; } = DefaultVariableValue;
}