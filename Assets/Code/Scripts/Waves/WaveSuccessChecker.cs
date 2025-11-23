using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WaveSuccessChecker : MonoBehaviour
{
    [Range(0f, 1f)] public float SuccessPecentage = .05f;
    [Range(1, 100)] public int SuccessIntervals = 11;
    [Range(0f, 1f)] public float NeededSuccessPercentage = .75f;

    [Header("Debug")]
    public bool ShowIntevals = false;

    private float SuccessDistance => SuccessPecentage * 2f;

    private void Update()
    {
        Shader.SetGlobalFloat("_IntervalCount", SuccessIntervals);
        Shader.SetGlobalFloat("_ShowIntervals", ShowIntevals ? 1f : 0f);
        Shader.SetGlobalFloat("_GoalWidth", SuccessDistance);
    }

    public void CheckWaveSuccess(GoalWave goalWave, CombinedWave combinedWave)
    {
        var goalWaveInfos = goalWave.GetWaveInfosAndDisplayVariableValues()
            .Select(x => x.Item1)
            .ToArray();

        // Copy them so no more adjustments can be made
        var combinedWaveInfos = combinedWave.GetWaveInfosAndDisplayVariableValues()
            .Select(x => x.Item1.Copy())
            .ToArray();

        GetResults(goalWaveInfos, combinedWaveInfos);
    }

    private void GetResults(WaveInfo[] goalWaveInfos, WaveInfo[] combinedWaveInfos)
    {
        var intervalPoints = GetIntervalPoint().ToArray();

        var resultOutput = new StringBuilder();
        resultOutput.AppendLine($"Checking at {intervalPoints.Length} points of waves...");
        resultOutput.AppendLine($"goal variable values: {string.Join(", ", goalWaveInfos.Select(x => x.VariableValue.ToString("F2")))}");
        resultOutput.AppendLine($"user variable values: {string.Join(", ", combinedWaveInfos.Select(x => x.VariableValue.ToString("F2")))}");
        resultOutput.AppendLine();

        var successCount = 0;
        foreach (var intervalPoint in intervalPoints)
        {
            var goalValue = goalWaveInfos.Calculate(intervalPoint);
            var userValue = combinedWaveInfos.Calculate(intervalPoint);
            var diff = goalValue - userValue;

            var isSuccess = Mathf.Abs(diff) <= SuccessDistance;
            if (isSuccess)
                successCount++;
            var successChar = isSuccess ? '+' : '-';

            resultOutput.AppendLine($"({successChar})|{intervalPoint:F2}: g={goalValue:F2} u={userValue:F2} ({Mathf.Abs(diff):F2})");
        }

        resultOutput.AppendLine($"Successes: {successCount}/{intervalPoints.Length}");

        Debug.Log(resultOutput.ToString());
    }

    private IEnumerable<float> GetIntervalPoint()
    {
        var intervalSize = (2f * Mathf.PI) / (SuccessIntervals - 1);

        var steps = SuccessIntervals;
        for (var i = 0; i < steps; i++)
            yield return i * intervalSize;
    }

    private WaveMatchingResults GetResults_Old(WaveInfo[] goalWaveInfos, WaveInfo[] combinedWaveInfos)
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

            if (Mathf.Abs(goalValueTotal - userValueTotal) <= SuccessPecentage)
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
}
