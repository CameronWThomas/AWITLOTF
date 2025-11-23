using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private const string DistortionFactor = "_DistortionFactor";
    private const string TimeValueName = "_TimeValue";

    public bool UseDiscreteWaves = false;

    [Header("Wave Objects")]
    public ComponentWave ComponentWave1;
    public ComponentWave ComponentWave2;
    public ComponentWave ComponentWave3;
    
    public CombinedWave CombinedWave;
    public GoalWave GoalWave;

    [Header("Distoration related")]
    [Range(1f, 1200f)] public float PercentageChangeMaxDistoration = 300f;

    [Header("Moving wave stuff")]
    [Range(0f, 50f)] public float WaveSpeedModifier = .5f;

    [Header("Goal checking")]
    [Range(0f, 2f)] public float SuccessDistance = .4f;
    [Range(1, 25)] public int SuccessIntervals = 11;
    [Range(0f, 1f)] public float NeededSuccessPercentage = .75f;

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

    private float time = -1f;
    private void Update()
    {
        // For now, just pressing space submits the waves
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SubmitWaves();
        }

        if (time < 0f)
            time = Time.time;

        time += Time.deltaTime * WaveSpeedModifier;
        Shader.SetGlobalFloat(TimeValueName, time);

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
        
        var results = GetResults(goalWaveInfos, combinedWaveInfos);

        Debug.Log(results);

        ReinitializeWaves();
    }

    private WaveMatchingResults GetResults(WaveInfo[] goalWaveInfos, WaveInfo[] combinedWaveInfos)
    {
        var max = 2f * Mathf.PI;
        var step = max / (SuccessIntervals - 1);

        var successCount = 0;
        var totalGoalWaves = new float[3] { 0f, 0f, 0f };
        var totalUserWaves = new float[3] { 0f, 0f, 0f };
        for (var i = 0; i < 10; i++)
        {
            var x = i * step;

            var goalValues = new float[3]
            {
                goalWaveInfos[0].Calculate(x),
                goalWaveInfos[1].Calculate(x),
                goalWaveInfos[2].Calculate(x),
            };
            var userValues = new float[3]
            {
                combinedWaveInfos[0].Calculate(x),
                combinedWaveInfos[1].Calculate(x),
                combinedWaveInfos[2].Calculate(x),
            };

            // This may seem weird, but adding 1 to each should give useful values since we will have no negatives
            totalGoalWaves[0] += goalValues[0] + 1f;
            totalGoalWaves[1] += goalValues[1] + 1f;
            totalGoalWaves[2] += goalValues[2] + 1f;

            totalUserWaves[0] += userValues[0] + 1f;
            totalUserWaves[1] += userValues[1] + 1f;
            totalUserWaves[2] += userValues[2] + 1f;

            var goalValueTotal = goalValues.Sum(x => x);
            var userValueTotal = userValues.Sum(x => x);

            if (Mathf.Abs(goalValueTotal - userValueTotal) <= SuccessDistance)
                successCount++;
        }

        var isSuccess = successCount >= SuccessIntervals * NeededSuccessPercentage;

        var wave1Diff = Mathf.Abs(totalGoalWaves[0] - totalUserWaves[0]);
        var wave2Diff = Mathf.Abs(totalGoalWaves[1] - totalUserWaves[1]);
        var wave3Diff = Mathf.Abs(totalGoalWaves[2] - totalUserWaves[2]);

        return new WaveMatchingResults(isSuccess)
        {
            Wave1MatchPercentage = wave1Diff / totalGoalWaves[0],
            Wave2MatchPercentage = wave2Diff / totalGoalWaves[1],
            Wave3MatchPercentage = wave3Diff / totalGoalWaves[2],
            SuccessCount = successCount
        };
    }

    private void SetDistortionPercentage(float percentage)
    {
        Shader.SetGlobalFloat(DistortionFactor, percentage);
    }

    private class WaveMatchingResults
    {
        public WaveMatchingResults(bool isAbsoluteSuccess)
        {
            IsAbsoluteSuccess = isAbsoluteSuccess;
        }

        public bool IsAbsoluteSuccess { get; }
        public float Wave1MatchPercentage { get; set; } = 1f;
        public float Wave2MatchPercentage { get; set; } = 1f;
        public float Wave3MatchPercentage { get; set; } = 1f;
        public int SuccessCount { get; set; } = 0;

        public override string ToString()
        {
            var wave1PercentString = Wave1MatchPercentage.ToString("F2");
            var wave2PercentString = Wave2MatchPercentage.ToString("F2");
            var wave3PercentString = Wave3MatchPercentage.ToString("F2");

            return $"{(IsAbsoluteSuccess ? "SUCCESS" : "FAILED")} ({SuccessCount}) - {wave1PercentString}%|{wave2PercentString}%|{wave3PercentString}%";
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
}
