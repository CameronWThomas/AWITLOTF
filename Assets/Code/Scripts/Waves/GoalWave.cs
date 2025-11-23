using System.Collections.Generic;
using System.Linq;
using UnityEditor;

//TODO remove this. eventually Combined wave will be the thing that does all this
public class GoalWave : Wave
{
    private const int ComponentWavesCount = 3;

    private readonly WaveInfo[] _goalWaveInfos = new WaveInfo[ComponentWavesCount] { null, null, null };

    public override IEnumerable<(WaveInfo, float)> GetWaveInfosAndDisplayVariableValues()
    {
        foreach (var waveInfo in _goalWaveInfos)
            yield return (waveInfo, waveInfo.VariableValue);
    }

    public void Initialize(ComponentWave componentWave1, ComponentWave componentWave2, ComponentWave componentWave3)
    {
        _goalWaveInfos[0] = componentWave1.WaveInfo.Copy();
        _goalWaveInfos[1] = componentWave2.WaveInfo.Copy();
        _goalWaveInfos[2] = componentWave3.WaveInfo.Copy();
        foreach (var goalWaveInfo in _goalWaveInfos)
            goalWaveInfo.SetRandomVariableValue();
    }

    protected override IEnumerable<WaveInfo> GetWaveInfos()
    {
        foreach (var waveInfo in _goalWaveInfos.Where(x => x != null))
            yield return waveInfo;
    }
}

[CustomEditor(typeof(GoalWave))]
public class GoalWaveEditor : WaveEditor { }