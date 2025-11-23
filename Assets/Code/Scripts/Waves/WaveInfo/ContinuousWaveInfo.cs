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

    public static ContinuousWaveInfo[] Create(params WaveType[] waveTypes)
        => waveTypes.Select(x => new ContinuousWaveInfo(50f, x)).ToArray();

    public static ContinuousWaveInfo[] GetRandoms(int count)
    {
        var waveTypes = EnumHelper.GetValues<WaveType>().Randomize();
        var maxCount = waveTypes.Count();
        if (count < 0 || count > maxCount)
            throw new ArgumentException($"{nameof(count)} must be between [0, {maxCount}] ", nameof(count));

        return Create(waveTypes.Take(count).ToArray());
    }

    /// <summary>
    /// 0-100 range
    /// </summary>
    public float Percentage { get; private set; }

    public float OverallPercentageChanges { get; private set; } = 0f;

    public override float VariableValue => GetPercentageValue(Percentage);

    public override void SetRandomVariableValue() => SetPercentage(RandomHelper.Between(0f, 100f));

    public override WaveInfo Copy() => new ContinuousWaveInfo(Percentage, WaveType);

    public void SetPercentage(float percentage)
    {
        var lastPercentage = Percentage;
        Percentage = Mathf.Clamp(percentage, 0f, 100f);

        OverallPercentageChanges += Mathf.Abs(Percentage - lastPercentage);
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