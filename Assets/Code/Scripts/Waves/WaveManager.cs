using System;
using System.Linq;
using System.Linq.Expressions;
using AWITLOTF.Assets.Code.Scripts.Npc;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private const string DistortionFactor = "_DistortionFactor";
    private const string TimeValueName = "_TimeValue";
    NpcManager _npcManager;

    [Header("Wave Objects")]
    public ComponentWave ComponentWave1;
    public ComponentWave ComponentWave2;
    public ComponentWave ComponentWave3;
    
    public CombinedWave CombinedWave;
    public GoalWave GoalWave;

    [Header("Distoration related")]
    [Range(1f, 1200f)] public float PercentageChangeMaxDistoration = 300f;
    public bool EnableDistoration = true;

    [Header("Moving wave stuff")]
    [Range(0f, 50f)] public float WaveSpeedModifier = .5f;

    

    private ContinuousWaveInfo[] _continuousWaveInfos = new ContinuousWaveInfo[3];

    private void Start()
    {
        _npcManager = FindObjectOfType<NpcManager>();
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
            OnWaveSubmit();
            // GetComponent<WaveSuccessChecker>().CheckWaveSuccess(GoalWave, CombinedWave);
        if (Input.GetKeyDown(KeyCode.R))
            ReinitializeWaves();

        if (time < 0f)
            time = Time.time;

        time += Time.deltaTime * WaveSpeedModifier;
        Shader.SetGlobalFloat(TimeValueName, time);

        var percentage = GetPercentageChange() / PercentageChangeMaxDistoration;
        SetDistortionPercentage(Mathf.Clamp01(percentage));
    }    
    private void OnWaveSubmit()
    {
        GetComponent<WaveSuccessChecker>().CheckWaveSuccess(GoalWave, CombinedWave);
        _npcManager.AdvanceQueue();
        ReinitializeWaves();

    }

    private void ReinitializeWaves()
    {
        var waveInfos = ContinuousWaveInfo.Create(ComponentWave1.WaveType, ComponentWave2.WaveType, ComponentWave3.WaveType);

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

    private void SetDistortionPercentage(float percentage)
    {
        percentage = EnableDistoration ? percentage : 0f;
        
        Shader.SetGlobalFloat(DistortionFactor, percentage);
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
