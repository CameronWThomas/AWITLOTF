using AWITLOTF.Assets.Code.Scripts;
using UnityEngine;

public class GlobalStateManager : MonoBehaviour
{
    [Header("Purity")]
    public float BodyPurity = 3f;
    public float MindPurity = 3f;
    public float SoulPurity = 3f;

    [Header("Run Counts")]
    public int CurrentRunCount = 1;
    public int MaxRuns = 3;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool IsFinalRun => CurrentRunCount >= MaxRuns;

    public void SaveState(WorldStateManager worldStateManager)
    {
        BodyPurity = worldStateManager.BodyPurity;
        MindPurity = worldStateManager.MindPurity;
        SoulPurity = worldStateManager.SoulPurity;
    }

    public void IncrementRun()
    {
        CurrentRunCount++;
        CurrentRunCount = Mathf.Clamp(CurrentRunCount, 1, MaxRuns);
    }
}
