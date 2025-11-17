using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class OLD_Wave : MonoBehaviour
{
    public WaveType WaveType = WaveType.Sin;
    public WaveType[] WaveTypes;

    [Range(.1f, 2f)] public float WaveUpdateDuration = 1f;

    [Header("Debug")]
    public bool Increase = false;
    public bool Decrease = false;
    public float SetVariableValue = 0f;
    public bool TriggerSetValue = false;

    public float VariableValue { get; private set; } = 1f;

    private int _variableState = 0;
    private int _variableMaxState = 5;
    private float _variableMinValue = 1f;
    private float _variableMaxValue = 30f;

    private bool _isChanging;

    public void UpdateWaveTypes(params WaveType[] waveTypes)
    {
        WaveType = waveTypes.First();
        WaveTypes = waveTypes.ToArray();
    }

    private void Start()
    {
        VariableValue = GetVariableValueFromState(_variableState);
    }

    private float GetVariableValueFromState(int variableState)
    {
        var step = (_variableMaxValue - _variableMinValue) / _variableMaxState;
        return _variableMinValue + step * variableState;
    }

    private void Update()
    {
        if (!_isChanging)
        {
            if (TriggerSetValue)
            {
                TriggerSetValue = false;
                StartCoroutine(ChangeWaveRoutine(VariableValue, SetVariableValue));
            }
            else if (Increase)
                StartWaveChange(1);
            else if (Decrease)
                StartWaveChange(-1);
        }

        Increase = false;
        Decrease = false;
    }

    private void StartWaveChange(int delta)
    {
        //TODO should check the wave types?


        var newState = _variableState + delta;
        if (newState < 0 || newState >= _variableMaxState)
            return;

        var currentValue = VariableValue;
        var nextValue = GetVariableValueFromState(newState);
        _variableState = newState;

        StartCoroutine(ChangeWaveRoutine(currentValue, nextValue));
    }

    private IEnumerator ChangeWaveRoutine(float startValue, float endValue)
    {
        _isChanging = true;

        try
        {
            var duration = WaveUpdateDuration;
            var startTime = Time.time;
            var endTime = startTime + duration;

            while (Time.time < endTime)
            {
                var t = Mathf.Clamp01((Time.time - startTime) / duration);
                VariableValue = Mathf.SmoothStep(startValue, endValue, t);

                yield return null;
            }

            VariableValue = endValue;
        }
        finally
        {
            _isChanging = false;
        }
    }
}
