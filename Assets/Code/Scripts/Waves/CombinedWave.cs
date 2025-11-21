using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombinedWave : Wave
{
    private readonly ComponentWave[] _componentWaves = new ComponentWave[3] { null, null, null };

    public override IReadOnlyList<(WaveInfo, float)> GetWaveInfosAndDisplayVariableValues()
    {
        return _componentWaves
            .Where(x => x != null)
            .SelectMany(x => x.GetWaveInfosAndDisplayVariableValues())
            .ToArray();
    }
    
    public void Initialize(ComponentWave componentWave1, ComponentWave componentWave2, ComponentWave componentWave3)
    {
        _componentWaves[0] = componentWave1;
        _componentWaves[1] = componentWave2;
        _componentWaves[2] = componentWave3;
    }
}
