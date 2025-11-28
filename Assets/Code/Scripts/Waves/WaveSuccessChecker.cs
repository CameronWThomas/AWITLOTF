using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WaveSuccessChecker : MonoBehaviour
{
    [Range(0f, 1f)] public float SuccessPecentage = .05f;
    [Range(1, 100)] public int SuccessIntervals = 11;
    [Range(0f, 1f)] public float NeededSuccessPercentage = .75f;
    [Range(0, 10)] public int CriticallyBadCount = 4;

    [Header("Debug")]
    public bool ShowIntevals = false;

    private Dictionary<WaveTrait, int> _failedWaveTraitCounts = new Dictionary<WaveTrait, int>();
    private StringBuilder _resultOutputBuilder = new StringBuilder();

    private float SuccessDistance => SuccessPecentage * 2f;

    private void Start()
    {
        ResetFailureCount();
    }

    private void Update()
    {
        Shader.SetGlobalFloat("_IntervalCount", SuccessIntervals);
    }

    public void ResetFailureCount()
    {
        _failedWaveTraitCounts = GetWaveFailureCountDictionary();
    }

    public void RecordWaveSuccess(GoalWave goalWave, CombinedWave combinedWave)
    {
        var goalWaveInfos = goalWave.GetWaveInfosAndDisplayVariableValues()
            .Select(x => x.Item1)
            .OfType<ContinuousWaveInfo>()
            .ToArray();

        // Copy them so no more adjustments can be made
        var combinedWaveInfos = combinedWave.GetWaveInfosAndDisplayVariableValues()
            .Select(x => x.Item1.Copy())
            .OfType<ContinuousWaveInfo>()
            .ToArray();

        RecordResults(goalWaveInfos, combinedWaveInfos);
    }

    public IEnumerable<WaveTrait> GetCriticallyBadWaveTraits()
    {
        foreach (var keyValue in _failedWaveTraitCounts)
        {
            if (keyValue.Value >= CriticallyBadCount)
                yield return keyValue.Key;
        }
    }

    private void RecordResults(ContinuousWaveInfo[] goalWaveInfos, ContinuousWaveInfo[] combinedWaveInfos)
    {
        var intervalPoints = GetIntervalPoint().ToArray();
        ResetResultOutputBuilder(goalWaveInfos, combinedWaveInfos, intervalPoints);

        // Count how many points are within range of the goal value
        int successCount = 0;
        foreach (var intervalPoint in intervalPoints)
        {
            var isSuccessful = IsPointSuccessful(goalWaveInfos, combinedWaveInfos, intervalPoint);
            if (isSuccessful)
                successCount++;
        }
        _resultOutputBuilder.AppendLine();

        // If there is at neast NeededSuccessPercentage percent of successful samplings, then it is a success and we don't check which trait is worse
        var isOverallSuccessful = successCount >= intervalPoints.Length * NeededSuccessPercentage;
        var overallSuccesssString = isOverallSuccessful ? "SUCCESS" : "FAILURE";
        _resultOutputBuilder.AppendLine($"({overallSuccesssString}: {successCount}/{intervalPoints.Length}");

        if (!isOverallSuccessful)
        {
            // Figure out which wave was the furthest away from the percentage we found with the goals. This will be the wave we mark as being the worst
            var worstTrait = FindWorstTrait(goalWaveInfos, combinedWaveInfos);
            _failedWaveTraitCounts[worstTrait]++;
        }

        Debug.Log(_resultOutputBuilder.ToString());
    }

    private bool IsPointSuccessful(ContinuousWaveInfo[] goalWaveInfos, ContinuousWaveInfo[] combinedWaveInfos, float intervalPoint)
    {
        var goalValue = goalWaveInfos.Calculate(intervalPoint);
        var userValue = combinedWaveInfos.Calculate(intervalPoint);
        var diff = goalValue - userValue;

        var isSuccess = Mathf.Abs(diff) <= SuccessDistance;
        var successChar = isSuccess ? '+' : '-';

        _resultOutputBuilder.AppendLine($"({successChar})|{intervalPoint:F2}: g={goalValue:F2} u={userValue:F2} ({Mathf.Abs(diff):F2})");

        return isSuccess;
    }

    private WaveTrait FindWorstTrait(ContinuousWaveInfo[] goalWaveInfos, ContinuousWaveInfo[] userWaveInfos)
    {
        var worstWaveTrait = WaveTrait.Body;
        var largestPercentageDiff = float.MinValue;

        _resultOutputBuilder.AppendLine("Trait percentage diffs:");

        foreach (var waveTrait in EnumHelper.GetValues<WaveTrait>())
        {
            var goalPercentage = goalWaveInfos.FirstWaveTrait(waveTrait).Percentage;
            var userPercentage = userWaveInfos.FirstWaveTrait(waveTrait).Percentage;

            var percentageDiff = Mathf.Abs(goalPercentage - userPercentage);

            _resultOutputBuilder.AppendLine($"\t{waveTrait}: {percentageDiff:F2}");

            if (percentageDiff > largestPercentageDiff)
            {
                worstWaveTrait = waveTrait;
                largestPercentageDiff = percentageDiff;
            }
        }

        _resultOutputBuilder.AppendLine($"Worst trait: {worstWaveTrait}");
        _resultOutputBuilder.AppendLine();
        return worstWaveTrait;
    }

    private void ResetResultOutputBuilder(WaveInfo[] goalWaveInfos, WaveInfo[] combinedWaveInfos, float[] intervalPoints)
    {
        _resultOutputBuilder.Clear();
        _resultOutputBuilder.AppendLine($"Checking at {intervalPoints.Length} points of waves...");
        _resultOutputBuilder.AppendLine($"goal variable values: {string.Join(", ", goalWaveInfos.Select(x => x.VariableValue.ToString("F2")))}");
        _resultOutputBuilder.AppendLine($"user variable values: {string.Join(", ", combinedWaveInfos.Select(x => x.VariableValue.ToString("F2")))}");
        _resultOutputBuilder.AppendLine();
    }

    private void GetResults_old(WaveInfo[] goalWaveInfos, WaveInfo[] combinedWaveInfos)
    {
        var intervalPoints = GetIntervalPoint().ToArray();

        var resultOutput = new StringBuilder();
        resultOutput.AppendLine($"Checking at {intervalPoints.Length} points of waves...");
        resultOutput.AppendLine($"goal variable values: {string.Join(", ", goalWaveInfos.Select(x => x.VariableValue.ToString("F2")))}");
        resultOutput.AppendLine($"user variable values: {string.Join(", ", combinedWaveInfos.Select(x => x.VariableValue.ToString("F2")))}");
        resultOutput.AppendLine();
        
        var waveTraitFailureCountDict = GetWaveFailureCountDictionary();
        var successCount = 0;
        foreach (var intervalPoint in intervalPoints)
        {
            var goalValue = goalWaveInfos.Calculate(intervalPoint);
            var userValue = combinedWaveInfos.Calculate(intervalPoint);
            var diff = goalValue - userValue;

            var isSuccess = Mathf.Abs(diff) <= SuccessDistance;            
            var successChar = isSuccess ? '+' : '-';

            resultOutput.AppendLine($"({successChar})|{intervalPoint:F2}: g={goalValue:F2} u={userValue:F2} ({Mathf.Abs(diff):F2})");
            if (isSuccess)
            {
                successCount++;
                continue;
            }

            var worstWaveTrait = GetWorstWaveTrait(goalWaveInfos, combinedWaveInfos, intervalPoint);
            waveTraitFailureCountDict[worstWaveTrait]++;
        }

        Debug.Log(resultOutput.ToString());
        resultOutput.Clear();

        var overallSuccess = successCount >= intervalPoints.Length * NeededSuccessPercentage;
        var overallSuccessChar = overallSuccess ? '+' : '-';
        resultOutput.AppendLine($"({overallSuccessChar})|Successes: {successCount}/{intervalPoints.Length}");

        if (!overallSuccess)
        {
            var worstOverallTrait = waveTraitFailureCountDict
                .First(x => waveTraitFailureCountDict.All(y => x.Value >= y.Value))
                .Key;
            _failedWaveTraitCounts[worstOverallTrait]++;

            resultOutput.AppendLine($"Worst wave trait: {worstOverallTrait}");
            foreach (var keyValue in waveTraitFailureCountDict)
                resultOutput.AppendLine($"\t({keyValue.Value}) {keyValue.Key}");
        }

        Debug.Log(resultOutput.ToString());
    }

    private IEnumerable<float> GetIntervalPoint()
    {
        var intervalSize = (2f * Mathf.PI) / (SuccessIntervals - 1);

        var steps = SuccessIntervals;
        for (var i = 0; i < steps; i++)
            yield return i * intervalSize;
    }

    private WaveTrait GetWorstWaveTrait(WaveInfo[] goalWaveInfos, WaveInfo[] combinedWaveInfos, float intervalPoint)
    {
        var worstWaveTrait = WaveTrait.Body;
        var largestValueDiff = float.MinValue;

        foreach (var waveTrait in EnumHelper.GetValues<WaveTrait>())
        {
            var goalValue = goalWaveInfos.FirstWaveTrait(waveTrait).Calculate(intervalPoint);
            var combinedValue = combinedWaveInfos.FirstWaveTrait(waveTrait).Calculate(intervalPoint);

            var valueDiff = Mathf.Abs(goalValue - combinedValue);
            if (valueDiff > largestValueDiff)
            {
                worstWaveTrait = waveTrait;
                largestValueDiff = valueDiff;
            }
        }

        return worstWaveTrait;
    }

    private static Dictionary<WaveTrait, int> GetWaveFailureCountDictionary()
    {
        var waveFailureCountsDictionary = new Dictionary<WaveTrait, int>();
        foreach (var waveTrait in EnumHelper.GetValues<WaveTrait>())
            waveFailureCountsDictionary.Add(waveTrait, 0);
        return waveFailureCountsDictionary;
    }
}
