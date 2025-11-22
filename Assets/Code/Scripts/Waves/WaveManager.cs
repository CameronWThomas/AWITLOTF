using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private const string DistortionFactor = "_DistortionFactor";

    public bool UseDiscreteWaves = false;

    [Header("Wave Objects")]
    public ComponentWave ComponentWave1;
    public ComponentWave ComponentWave2;
    public ComponentWave ComponentWave3;
    
    public CombinedWave CombinedWave;
    public GoalWave GoalWave;

    [Header("Distoration related")]
    [Range(1f, 1200f)] public float PercentageChangeMaxDistoration = 300f;

    private ContinuousWaveInfo[] _continuousWaveInfos = new ContinuousWaveInfo[3];

    private void Start()
    {
        ReinitializeWaves();
    }

    public float GetPercentageChange()
    {
        var change = 0f;
        foreach (var continuousWaveInfo in _continuousWaveInfos.Where(x => x != null))
            change += continuousWaveInfo.OverallPercentageChanges;

        return change;
    }

    private void Update()
    {
        // For now, just pressing space submits the waves
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SubmitWaves();
        }

        var percentage = GetPercentageChange() / PercentageChangeMaxDistoration;
        SetDistortionPercentage(percentage);
    }    

    private void ReinitializeWaves()
    {
        WaveInfo[] waveInfos = UseDiscreteWaves
            ? DiscreteWaveInfo.GetRandoms(3)
            : ContinuousWaveInfo.GetRandoms(3);

        ComponentWave1.SetWaveInfo(waveInfos[0]);
        ComponentWave2.SetWaveInfo(waveInfos[1]);
        ComponentWave3.SetWaveInfo(waveInfos[2]);

        _continuousWaveInfos[0] = waveInfos[0] as ContinuousWaveInfo;
        _continuousWaveInfos[1] = waveInfos[1] as ContinuousWaveInfo;
        _continuousWaveInfos[2] = waveInfos[2] as ContinuousWaveInfo;

        if (CombinedWave != null)
            CombinedWave.Initialize(ComponentWave1, ComponentWave2, ComponentWave3);

        //TODO eventually there will no longer be a goal wave
        if (GoalWave != null)
            GoalWave.Initialize(ComponentWave1, ComponentWave2, ComponentWave3);

        SetDistortionPercentage(0f);
    }

    private void SubmitWaves()
    {
        var goalWaveInfos = GoalWave.GetWaveInfosAndDisplayVariableValues()
            .Select(x => x.Item1)
            .ToArray();

        // Copy them so no more adjustments can be made
        var combinedWaveInfos = CombinedWave.GetWaveInfosAndDisplayVariableValues()
            .Select(x => x.Item1.Copy())
            .ToArray();
        
        GetResults(goalWaveInfos, combinedWaveInfos);

        ReinitializeWaves();
    }

    private static void GetResults(WaveInfo[] goalWaveInfos, WaveInfo[] combinedWaveInfos)
    {
        // TODO
        // What this should be is sampling the wave at a handful of points between 0 and 2pi. If all points are within some range, its a complete success.
        // If we are off, we look at the wave infos and figure out how far off the percentages are



        var matchingWaveTypeInfos = goalWaveInfos
            .Select(x => new { GoalWave = x, UserWave = combinedWaveInfos.First(x => x.WaveType == x.WaveType) })
            .ToArray();

        foreach (var pair in matchingWaveTypeInfos)
        {
            var goalWave = pair.GoalWave;
            var userWave = pair.UserWave;

            //TODO 
        }
    }

    private void SetDistortionPercentage(float percentage)
    {
        Shader.SetGlobalFloat(DistortionFactor, percentage);
    }
}

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (target is WaveManager waveManager)
        {
            EditorGUILayout.LabelField($"Percenteage of max distoration: {waveManager.GetPercentageChange()}/{waveManager.PercentageChangeMaxDistoration}");
        }
    }
}
