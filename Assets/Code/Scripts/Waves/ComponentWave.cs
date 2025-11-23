using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ComponentWave : Wave
{
    [Header("Component Wave")]
    public WaveTrait WaveTrait = WaveTrait.Body;

    [Header("Continuous Wave Settings")]
    [Range(.001f, 1f)]public float PercentageChangeFactor = .25f;

    private bool _isDiscreteWaveUpdating = false;
    private float _discreteDisplayVariableValue = 0f;

    public WaveInfo WaveInfo { get; private set; }
    public WaveType WaveType => WaveTrait.ToWaveType();

    public override IEnumerable<(WaveInfo, float)> GetWaveInfosAndDisplayVariableValues()
    {
        var variableValue = WaveInfo.VariableValue;
        if (WaveInfo is DiscreteWaveInfo && _isDiscreteWaveUpdating)
            variableValue = _discreteDisplayVariableValue;

        yield return (WaveInfo, variableValue);
    }

    public void SetWaveInfo(WaveInfo waveInfo)
    {
        WaveInfo = waveInfo;
        _discreteDisplayVariableValue = waveInfo.VariableValue;
    }

    protected override IEnumerable<WaveInfo> GetWaveInfos()
    {
        yield return WaveInfo;
    }

    void Update()
    {
        if (_isDiscreteWaveUpdating)
            return;

        var inputChange = GetInputChange();
        if (inputChange == 0)
            return;

        if (WaveInfo is DiscreteWaveInfo discreteWaveInfo)
            TryUpdateVariableValue(discreteWaveInfo, inputChange);
        else if (WaveInfo is ContinuousWaveInfo continuousWaveInfo)
            TryUpdateVariableValue(continuousWaveInfo, inputChange);
    }

    private void TryUpdateVariableValue(ContinuousWaveInfo waveInfo, int percentageChange)
    {
        // TODO probably track the speed of change?
        waveInfo.ChangePercentage(percentageChange * PercentageChangeFactor);

        // TODO trigger some event on a successful change?
    }

    private void TryUpdateVariableValue(DiscreteWaveInfo waveInfo, int indexChange)
    {
        // Check if the next value is valid
        var newIndex = waveInfo.VariableValueIndex + indexChange;
        if (!waveInfo.TryGetVariableValue(newIndex, out var value))
            return;

        StartCoroutine(UpdateDiscreteVariableValueRoutine(waveInfo, newIndex));
    }

    private IEnumerator UpdateDiscreteVariableValueRoutine(DiscreteWaveInfo waveInfo, int newIndex)
    {
        if (_isDiscreteWaveUpdating)
            yield break;

        try
        {
            // Firstly, mark the wave as updating
            _isDiscreteWaveUpdating = true;

            var originalVariableValue = waveInfo.VariableValue;
            waveInfo.TryGetVariableValue(newIndex, out var newVariableValue);

            var duration = WaveUpdateDuration;
            var startTime = Time.time;
            var endTime = startTime + duration;

            while (Time.time < endTime)
            {
                var t = Mathf.Clamp01((Time.time - startTime) / duration);
                var displayVariableValue = Mathf.SmoothStep(originalVariableValue, newVariableValue, t);

                _discreteDisplayVariableValue = displayVariableValue;

                yield return null;
            }

            waveInfo.TryUpdateVariableValue(newIndex);
            _discreteDisplayVariableValue = waveInfo.VariableValue;
        }
        finally
        {
            // Always mark the wave as no longer updating when we finish
            _isDiscreteWaveUpdating = false;
        }
    }


    private static readonly Dictionary<int, (KeyCode, KeyCode)> InputDict = new Dictionary<int, (KeyCode, KeyCode)>()
    {
        { 1, (KeyCode.Q, KeyCode.A) },
        { 2, (KeyCode.W, KeyCode.S) },
        { 3, (KeyCode.E, KeyCode.D) },
    };

    private int GetInputChange()
    {
        if (!InputDict.ContainsKey((int)WaveTrait))
        {
            Debug.LogError($"{name} - Bad wave num ({WaveTrait})");
            enabled = false;
            return 0;
        }

        var (upKey, downKey) = InputDict[(int)WaveTrait];

        var change = 0;
        if (Input.GetKey(upKey))
            change++;
        if (Input.GetKey(downKey))
            change--;

        return change;
    }
}

[CustomEditor(typeof(ComponentWave))]
public class ComponentWaveEditor : WaveEditor { }
