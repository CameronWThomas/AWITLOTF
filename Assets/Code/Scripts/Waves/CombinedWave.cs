using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CombinedWave : Wave
{
    private readonly ComponentWave[] _componentWaves = new ComponentWave[3] { null, null, null };

    public override IEnumerable<(WaveInfo, float)> GetWaveInfosAndDisplayVariableValues()
    {
        return _componentWaves
            .Where(x => x != null)
            .SelectMany(x => x.GetWaveInfosAndDisplayVariableValues());
    }

    public void Initialize(ComponentWave componentWave1, ComponentWave componentWave2, ComponentWave componentWave3)
    {
        _componentWaves[0] = componentWave1;
        _componentWaves[1] = componentWave2;
        _componentWaves[2] = componentWave3;
    }

    protected override IEnumerable<WaveInfo> GetWaveInfos()
    {
        foreach (var wave in _componentWaves.Where(x => x != null && x.WaveInfo != null))
            yield return wave.WaveInfo;
    }
}

[CustomEditor(typeof(CombinedWave))]
public class CombinedWaveEditor : WaveEditor { }
