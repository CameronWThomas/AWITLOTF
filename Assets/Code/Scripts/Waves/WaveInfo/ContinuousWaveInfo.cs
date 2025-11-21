using System.Linq;
using System;
using UnityEngine;

public class ContinuousWaveInfo : WaveInfo
{
    private readonly float _startValue;
    private readonly float _valueRange;

    public ContinuousWaveInfo(float startingPercentage, WaveType waveType)
        : base(waveType)
    {
        Percentage = startingPercentage;

        var minMax = waveType.GetVariableValueMinMax();
        _startValue = minMax.x;
        _valueRange = minMax.y - minMax.x;
    }

    public static ContinuousWaveInfo[] GetRandoms(int count)
    {
        var waveTypes = EnumHelper.GetValues<WaveType>().Randomize();
        var maxCount = waveTypes.Count();
        if (count < 0 || count > maxCount)
            throw new ArgumentException($"{nameof(count)} must be between [0, {maxCount}] ", nameof(count));

        return waveTypes.Take(count).Select(x => new ContinuousWaveInfo(50f, x)).ToArray();
    }

    /// <summary>
    /// 0-100 range
    /// </summary>
    public float Percentage { get; private set; }

    public override float VariableValue => GetPercentageValue(Percentage);

    public override void SetRandomVariableValue() => SetPercentage(RandomHelper.Between(0f, 100f));

    public override WaveInfo Copy() => new ContinuousWaveInfo(Percentage, WaveType);

    public void SetPercentage(float percentage)
    {
        Percentage = Mathf.Clamp(percentage, 0f, 100f);
    }

    public void ChangePercentage(float delta)
    {
        var newPercentage = Percentage + delta;
        SetPercentage(newPercentage);
    }

    public float GetPercentageValue(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0f, 100f);
        percentage /= 100f;

        return _startValue + percentage * _valueRange;
    }
}