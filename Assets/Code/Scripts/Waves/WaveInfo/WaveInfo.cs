using System;
using System.Linq;

public class WaveInfo
{
    public const int VariableValueIndexCount = 3;
    private static readonly float[] _variableValues = new float[VariableValueIndexCount]
    {
        .5f, 2f, 7.3f
    };

    public WaveInfo(int startingVariableValueIndex, params WaveType[] waveTypes)
    {
        VariableValueIndex = startingVariableValueIndex;
        WaveTypes = waveTypes ?? Array.Empty<WaveType>();
    }

    public static WaveInfo[] GetRandoms(int count)
    {
        var waveTypes = EnumHelper.GetValues<WaveType>().Randomize();
        var maxCount = waveTypes.Count();
        if (count < 0 || count > maxCount)
            throw new ArgumentException($"{nameof(count)} must be between [0, {maxCount}] ", nameof(count));

        return waveTypes.Take(count).Select(x => new WaveInfo(1, x)).ToArray();
    }

    public int VariableValueIndex { get; private set; }
    public float VariableValue => _variableValues[VariableValueIndex];
    public WaveType[] WaveTypes { get; }

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

    public WaveInfo Copy() => new WaveInfo(VariableValueIndex, WaveTypes);

    private bool IsIndexValid(int index) => index >= 0 && index < _variableValues.Length;
}