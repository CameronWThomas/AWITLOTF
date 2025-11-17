using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaveDisplay : MonoBehaviour
{
    const string WaveTypesName = "_WaveTypes";
    const string WaveVariableValuesName = "_WaveVariableValues";

    const string VariableValueName = "_VariableValue";
    const string WaveTypeName = "_WaveType";


    // Update is called once per frame
    void Update()
    {
        var material = GetMaterial();
        var wave = GetComponent<Wave>();
        
        Vector3 waveTypes;
        Vector3 variableValues;
        if (Application.isPlaying)
            GetWaveTypesAndVariableValues(wave, out waveTypes, out variableValues);
        else
            GetDebugWaveTypesAndVariableValues(wave, out waveTypes, out variableValues);

        material.SetVector(WaveTypesName, waveTypes);
        material.SetVector(WaveVariableValuesName, variableValues);
    }

    private void GetWaveTypesAndVariableValues(Wave wave, out Vector3 waveTypes, out Vector3 variableValues)
    {
        waveTypes = Vector3Int.zero;
        variableValues = Vector3.zero;

        var values = new List<(int, float)>();
        foreach (var waveInfo in wave.WaveInfos)
        {
            if (waveInfo == null)
            {
                values.Add((0, 0f));
                continue;
            }

            var mask = waveInfo.WaveTypes.GetMask();
            values.Add((mask, waveInfo.VariableValue));
        }

        waveTypes = new Vector3Int(values[0].Item1, values[1].Item1, values[2].Item1);
        variableValues = new Vector3(values[0].Item2, values[1].Item2, values[2].Item2);
    }

    private void GetDebugWaveTypesAndVariableValues(Wave wave, out Vector3 waveTypes, out Vector3 variableValues)
    {
        waveTypes = new Vector3Int((int)wave.WaveTypeDebug, 0, 0);
        variableValues = new Vector3(wave.VariableValueDebug, 0f, 0f);
    }

    private Material GetMaterial()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        return Application.isPlaying ? meshRenderer.material : meshRenderer.sharedMaterial;
    }
}
