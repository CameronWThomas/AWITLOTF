using System.Collections.Generic;

//TODO remove this. eventually Combined wave will be the thing that does all this
public class GoalWave : Wave
{
    private const int ComponentWavesCount = 3;

    private readonly ComponentWave[] _componentWaves = new ComponentWave[ComponentWavesCount] { null, null, null };
    private readonly int[] _goalVariableValueIndexes = new int[ComponentWavesCount] { 0, 0, 0 };

    public override IReadOnlyList<(WaveInfo, float)> GetWaveInfosAndDisplayVariableValues()
    {
        var data = new List<(WaveInfo, float)>();
        for (var i = 0; i < ComponentWavesCount; i++)
        {
            var waveInfo = _componentWaves[i].WaveInfo;
            waveInfo.TryGetVariableValue(_goalVariableValueIndexes[i], out var value);
            data.Add((waveInfo, value));
        }

        return data.ToArray();
    }

    public void Initialize(ComponentWave componentWave1, ComponentWave componentWave2, ComponentWave componentWave3)
    {
        _componentWaves[0] = componentWave1;
        _componentWaves[1] = componentWave2;
        _componentWaves[2] = componentWave3;

        _goalVariableValueIndexes[0] = RandomHelper.Between(0, WaveInfo.VariableValueIndexCount - 1);
        _goalVariableValueIndexes[1] = RandomHelper.Between(0, WaveInfo.VariableValueIndexCount - 1);
        _goalVariableValueIndexes[2] = RandomHelper.Between(0, WaveInfo.VariableValueIndexCount - 1);
    }
}
