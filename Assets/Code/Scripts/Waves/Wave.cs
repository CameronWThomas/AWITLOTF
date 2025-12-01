using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [Header("Display Attributes")]
    [Range(.1f, 2f)] public float WaveUpdateDuration = .25f;

    public bool IsHidden { get; set; } = false;

    public virtual IEnumerable<(WaveInfo, float)> GetWaveInfosAndDisplayVariableValues() => Array.Empty<(WaveInfo, float)>();

    public string GetWaveInfoString()
    {
        var waveInfos = GetWaveInfos().Where(x => x != null).ToArray();
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"{name} {nameof(WaveInfo)}s ({waveInfos.Length})");
        foreach (var waveInfo in waveInfos)
            stringBuilder.AppendLine($"x{waveInfo.VariableValue}: {waveInfo.WaveType.DisplayString()}");

        return stringBuilder.ToString();
    }

    protected virtual IEnumerable<WaveInfo> GetWaveInfos() => Array.Empty<WaveInfo>();

    protected void PrintWaveInfo(params WaveInfo[] waveInfos)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{name} {nameof(WaveInfo)}s ({waveInfos.Length})");
        foreach (var waveInfo in waveInfos)
            stringBuilder.AppendLine($"x{waveInfo.VariableValue}: {waveInfo.WaveType.DisplayString()}");

        Debug.Log(stringBuilder);
    }
}

// [CustomEditor(typeof(Wave))]
// public class WaveEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         if (target is Wave wave)
//         {
//             var waveInfoString = wave.GetWaveInfoString();
//             foreach (var line in waveInfoString.Split(Environment.NewLine))
//                 EditorGUILayout.LabelField(line);
//         }
//     }
// }
