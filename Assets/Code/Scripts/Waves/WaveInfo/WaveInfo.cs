using System;
using System.Linq;

public abstract class WaveInfo
{
    public WaveInfo(WaveType waveType)
    {
        WaveType = waveType;
    }

    public WaveType WaveType { get; }
    public abstract float VariableValue { get; }

    public abstract void SetRandomVariableValue();
    public abstract WaveInfo Copy();

    public float Calculate(float x)
    {
        var waveFunction = WaveType.GetWaveFunction();
        return waveFunction(x, VariableValue);
    }
}

public class DiscreteWaveInfo : WaveInfo
{
    public const int VariableValueIndexCount = 3;
    private static readonly float[] _variableValues = new float[VariableValueIndexCount]
    {
        .5f, 2f, 7.3f
    };

    public DiscreteWaveInfo(int startingVariableValueIndex, WaveType waveType)
        : base(waveType)
    {
        VariableValueIndex = startingVariableValueIndex;
    }

    public static DiscreteWaveInfo[] GetRandoms(int count)
    {
        var waveTypes = EnumHelper.GetValues<WaveType>().Randomize();
        var maxCount = waveTypes.Count();
        if (count < 0 || count > maxCount)
            throw new ArgumentException($"{nameof(count)} must be between [0, {maxCount}] ", nameof(count));

        return waveTypes.Take(count).Select(x => new DiscreteWaveInfo(1, x)).ToArray();
    }

    public int VariableValueIndex { get; private set; }
    public override float VariableValue => _variableValues[VariableValueIndex];

    public override void SetRandomVariableValue() => VariableValueIndex = RandomHelper.Between(0, VariableValueIndexCount - 1);

    public override WaveInfo Copy() => new DiscreteWaveInfo(VariableValueIndex, WaveType);


    public bool TryGetVariableValue(int index, out float value)
    {
        value = float.NaN;
        if (IsIndexValid(index))
            value = _variableValues[index];

        return float.IsNaN(value);
    }

    public bool TryUpdateVariableValue(int newIndex)
    {
        if (!IsIndexValid(newIndex))
            return false;

        VariableValueIndex = newIndex;
        return true;
    }


    private bool IsIndexValid(int index) => index >= 0 && index < _variableValues.Length;
}