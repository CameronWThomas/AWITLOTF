using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// TODO a lot of stuff is here is really only used by the component wave. It will need to be adjusted eventually
public class Wave : MonoBehaviour
{
    [Header("Display Attributes")]
    [Range(.1f, 2f)] public float WaveUpdateDuration = 1f;

    private readonly Dictionary<WaveInfo, WaveUpdatingInfo> _registeredWaveInfos = new();

    public virtual IReadOnlyList<(WaveInfo, float)> GetWaveInfosAndDisplayVariableValues() => _registeredWaveInfos.Select(x => (x.Key, x.Value.DisplayVariableValue)).ToArray();

    protected void RegisterWaveInfo(WaveInfo waveInfo)
    {
        if (_registeredWaveInfos.ContainsKey(waveInfo))
        {
            Debug.LogWarning($"{name}: Unable to register a {nameof(WaveInfo)} that already exists");
            return;
        }

        _registeredWaveInfos.Add(waveInfo, new(waveInfo));
    }

    protected bool TryUpdateVariableValue(WaveInfo waveInfo, int variableIndexChange)
    {
        // Validating we can actually update the variable
        if (variableIndexChange == 0)
            return false;

        if (!_registeredWaveInfos.ContainsKey(waveInfo))
        {
            Debug.LogWarning($"{name}: Unable to update the wave variable values. The {nameof(WaveInfo)} must be registered first");
            return false;
        }

        if (!IsWaveUpdatable(waveInfo))
        {
            Debug.LogWarning($"{name}: Unable to update the wave variable values. The {nameof(WaveInfo)} is in the process of being updated already");
            return false;
        }

        var newIndex = waveInfo.VariableValueIndex + variableIndexChange;
        if (waveInfo.TryGetVariableValue(waveInfo.VariableValueIndex + variableIndexChange, out var newVariableValue))
        {
            Debug.LogWarning($"{name}: Unable to update the wave variable values. The index change ({variableIndexChange}) is not valid");
            return false;
        }

        // We have validated it, so start the update
        StartCoroutine(UpdateVariableValueRoutine(waveInfo, newIndex));
        return true;
    }

    protected bool IsWaveUpdatable(WaveInfo waveInfo)
    {
        if (waveInfo == null)
            return false;

        if (_registeredWaveInfos.ContainsKey(waveInfo))
            return !_registeredWaveInfos[waveInfo].IsUpdating;
        return false;
    }

    protected void PrintWaveInfo(params WaveInfo[] waveInfos)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{name} {nameof(WaveInfo)}s ({waveInfos.Length})");
        foreach (var waveInfo in waveInfos)
            stringBuilder.AppendLine($"x{waveInfo.VariableValue}: {string.Join(",", waveInfo.WaveTypes)}");

        Debug.Log(stringBuilder);
    }

    private IEnumerator UpdateVariableValueRoutine(WaveInfo waveInfo, int newIndex)
    {
        // Should be checked ahead of time, but just in case
        if (!IsWaveUpdatable(waveInfo))
            yield break;

        try
        {
            // Firstly, mark the wave as updating
            _registeredWaveInfos[waveInfo].IsUpdating = true;

            var originalVariableValue = waveInfo.VariableValue;
            waveInfo.TryGetVariableValue(newIndex, out var newVariableValue);

            var duration = WaveUpdateDuration;
            var startTime = Time.time;
            var endTime = startTime + duration;

            while (Time.time < endTime)
            {
                var t = Mathf.Clamp01((Time.time - startTime) / duration);
                var displayVariableValue = Mathf.SmoothStep(originalVariableValue, newVariableValue, t);

                UpdateDisplayVariableValue(waveInfo, displayVariableValue);

                yield return null;
            }

            waveInfo.TryUpdateVariableValue(newIndex);
            UpdateDisplayVariableValue(waveInfo, waveInfo.VariableValue);
        }
        finally
        {
            // Always mark the wave as no longer updating when we finish
            _registeredWaveInfos[waveInfo].IsUpdating = false;
        }
    }

    private void UpdateDisplayVariableValue(WaveInfo waveInfo, float displayVariableValue)
    {
        if (!_registeredWaveInfos.ContainsKey(waveInfo))
        {
            Debug.LogWarning($"{name}: Unable to find the registed {nameof(WaveInfo)}");
            return;
        }

        _registeredWaveInfos[waveInfo].DisplayVariableValue = displayVariableValue;
    }

    private class WaveUpdatingInfo
    {
        public WaveUpdatingInfo(WaveInfo waveInfo)
        {
            DisplayVariableValue = waveInfo.VariableValue;
            IsUpdating = false;
        }

        public float DisplayVariableValue { get; set; }
        public bool IsUpdating { get; set; }
    }
}
