using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentWave : Wave
{
    [Header("Combined Wave")]
    [Range(1, 3)] public int WaveNum = 1;

    public WaveInfo WaveInfo { get; private set; }

    public void SetWaveInfo(WaveInfo waveInfo)
    {
        RegisterWaveInfo(waveInfo);
        WaveInfo = waveInfo;
    }

    void Update()
    {
        if (!IsWaveUpdatable(WaveInfo))
            return;

        var inputChange = GetInputChange();
        TryUpdateVariableValue(WaveInfo, inputChange);
    }

    private static readonly Dictionary<int, (KeyCode, KeyCode)> InputDict = new Dictionary<int, (KeyCode, KeyCode)>()
    {
        { 1, (KeyCode.Q, KeyCode.A) },
        { 2, (KeyCode.W, KeyCode.S) },
        { 3, (KeyCode.E, KeyCode.D) },
    };

    private int GetInputChange()
    {
        if (!InputDict.ContainsKey(WaveNum))
        {
            Debug.LogError($"{name} - Bad wave num ({WaveNum})");
            enabled = false;
            return 0;
        }

        var (upKey, downKey) = InputDict[WaveNum];

        if (Input.GetKeyDown(upKey))
            return 1;
        else if (Input.GetKeyDown(downKey))
            return -1;

        return 0;
    }
}
