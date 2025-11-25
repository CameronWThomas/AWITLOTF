using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WaveType
{
    Wave1 = 1,
    Wave2 = 1 << 1,
    Wave3 = 1 << 2,
}

public delegate float WaveFunctionDelegate(float x, float variableValue);

public static class WaveTypeExtensions
{
    public static WaveTrait ToWaveType(this WaveType waveType)
    {
        return waveType switch
        {
            WaveType.Wave1 => WaveTrait.Body,
            WaveType.Wave2 => WaveTrait.Mind,
            WaveType.Wave3 => WaveTrait.Spirit,
            _ => throw new NotImplementedException()
        };
    }

    public static int GetMask(this IEnumerable<WaveType> waveTypes)
    {
        var mask = 0;
        foreach (var waveType in waveTypes)
            mask |= waveType.GetMask();

        return mask;
    }

    public static int GetMask(this WaveType waveType) => (int)waveType;

    public static string DisplayString(this WaveType waveType)
    {
        return waveType switch
        {
            WaveType.Wave1 => "sin(x * v)",
            WaveType.Wave2 => "cos(x^v)",
            WaveType.Wave3 => "sin(x * v^2 + cos(2 * x * v))",
            _ => waveType.ToString()
        };
    }

    public static string DisplayString(this IEnumerable<WaveType> waveTypes)
    {
        var count = waveTypes.Count();

        if (count == 0) return string.Empty;
        if (count == 1) return waveTypes.First().DisplayString();

        return string.Join(", ", waveTypes.Select(x => x.DisplayString()));
    }

    public static Vector2 GetVariableValueMinMax(this WaveType waveType)
    {
        return waveType switch
        {
            WaveType.Wave1 => new(2f, 10f),
            WaveType.Wave2 => new(1f, 2.5f),
            WaveType.Wave3 => new(.75f, 1.75f),
            _ => Vector2.zero
        };
    }

    public static WaveFunctionDelegate GetWaveFunction(this WaveType waveType)
    {
        return waveType switch
        {
            WaveType.Wave1 => (x, v) => Mathf.Sin(x * v),
            WaveType.Wave2 => (x, v) => Mathf.Cos(Mathf.Pow(x, v)),
            WaveType.Wave3 => (x, v) => Mathf.Sin(Mathf.Cos(2f * x * v) + Mathf.Pow(v, 2f) * x),
            _ => throw new NotImplementedException()
        };
    }
}