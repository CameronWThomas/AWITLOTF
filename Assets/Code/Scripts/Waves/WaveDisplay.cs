using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[ExecuteInEditMode]
public class WaveDisplay : MonoBehaviour
{
    const string WaveTypesName = "_WaveTypes";
    const string WaveVariableValuesName = "_WaveVariableValues";
    const string GoalVariableValues = "_GoalVariableValues";
    const string DisplayGoalVariableValues = "_DisplayGoalVariableValues";

    [Header("Debug")]
    public WaveType DebugWaveType = WaveType.Wave1;
    [Range(-5f, 5f)] public float DebugVariableValue = 1f;
    [Range(-5f, 5f)] public float DebugGoalVariableValue = 1f;

    // Update is called once per frame
    void Update()
    {
        var material = GetMaterial();

        // TODO Ehhh
        Vector3 waveTypes = Vector3.zero, variableValues = Vector3.zero, goalVariableValues = Vector3.zero;
        var hasGoalWave = false;
        foreach (var wave in GetComponents<Wave>())
        {
            if (wave is GoalWave)
            {
                GetWaveTypesAndVariableValues(wave, out waveTypes, out goalVariableValues);
                hasGoalWave = true;
            }
            else
                GetWaveTypesAndVariableValues(wave, out waveTypes, out variableValues);
        }
        
        material.SetVector(WaveTypesName, waveTypes);
        material.SetVector(WaveVariableValuesName, variableValues);
        material.SetVector(GoalVariableValues, goalVariableValues);
        material.SetInt(DisplayGoalVariableValues, hasGoalWave ? 1 : 0);
    }

    private void GetWaveTypesAndVariableValues(Wave wave, out Vector3 waveTypes, out Vector3 variableValues)
    {
        waveTypes = Vector3Int.zero;
        variableValues = Vector3.zero;

        var waveInfoAndDisplayVariableValues = wave.GetWaveInfosAndDisplayVariableValues().ToArray();

        var values = new List<(int, float)>();
        for (var i = 0; i < 3; i++)
        {
            var displayVariableValue = 0f;
            var mask = 0;
            if (waveInfoAndDisplayVariableValues.Length > i)
            {
                WaveInfo waveInfo;
                (waveInfo, displayVariableValue) = waveInfoAndDisplayVariableValues[i];
                mask = waveInfo.WaveType.GetMask();
            }
            
            values.Add((mask, displayVariableValue));
        }

        waveTypes = new Vector3Int(values[0].Item1, values[1].Item1, values[2].Item1);
        variableValues = new Vector3(values[0].Item2, values[1].Item2, values[2].Item2);
    }

    private void GetDebugWaveTypesAndVariableValues(out Vector3 waveTypes, out Vector3 variableValues, out Vector3 goalVariableValues)
    {
        waveTypes = new Vector3Int((int)DebugWaveType, 0, 0);
        variableValues = new Vector3(DebugVariableValue, 0f, 0f);
        goalVariableValues = new Vector3(DebugGoalVariableValue, 0f, 0f);
    }

    private Material GetMaterial()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        return Application.isPlaying ? meshRenderer.material : meshRenderer.sharedMaterial;
    }
}
