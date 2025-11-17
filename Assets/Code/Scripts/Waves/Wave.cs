using System;
using System.Linq;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [Range(.1f, 2f)] public float WaveUpdateDuration = 1f;
    public bool IsGoalWave = false;

    [Header("Debug)")]
    public WaveType WaveTypeDebug = WaveType.Sin;
    [Range(0f, 5f)] public float VariableValueDebug = 1f;
    public bool PrintWaveInfo = false;

    readonly WaveInfo[] _waveInfos = new WaveInfo[3];

    public WaveInfo[] WaveInfos => _waveInfos;

    public void SetWaveInfos(WaveInfo waveInfo1, WaveInfo waveInfo2 = null, WaveInfo waveInfo3 = null)
    {
        if (waveInfo1 != null) _waveInfos[0] = waveInfo1;
        if (waveInfo2 != null) _waveInfos[1] = waveInfo2;
        if (waveInfo3 != null) _waveInfos[2] = waveInfo3;
    }

    private void Update()
    {
        if (PrintWaveInfo)
        {
            PrintWaveInfo = false;
            foreach (var waveInfo in _waveInfos.Where(x => x != null))
                Debug.Log($"{string.Join(",", waveInfo.WaveTypes)} - {waveInfo.VariableValue}");
        }

        if (IsGoalWave)
            return;

        // probably will remove this. We shall see
        foreach (var waveInfo in _waveInfos.Where(x => x != null))
            waveInfo.VariableValue = VariableValueDebug;
    }
}
