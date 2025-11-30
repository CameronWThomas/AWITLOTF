using AWITLOTF.Assets.Code.Scripts;
using AWITLOTF.Assets.Code.Scripts.Npc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles one whole run of all the people through the machine. All input will come from this. Intended to be reset on every run
/// </summary>
public class RunManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float PurityChangePerFailure = .34f;

    [Header("Debug")]
    public bool AllowTeleportAtAnyTime = false;

    WorldStateManager _worldStateManager;
    NpcManager _npcManager;
    WaveManager _waveManager;

    bool _waitingForNextPedestrian = true;
    bool _noMorePedestrians = false;
    bool _runEnding = false;

    List<WaveResult> _waveResults = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _worldStateManager = FindFirstObjectByType<WorldStateManager>();
        _npcManager = FindFirstObjectByType<NpcManager>();
        _waveManager = FindFirstObjectByType<WaveManager>();

        _waitingForNextPedestrian = true;
        _noMorePedestrians = false;
        _runEnding = false;

        // Turn off debug stuff just in case.
        AllowTeleportAtAnyTime = false;

        _waveManager.HideWaves();
        _waveResults.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (_runEnding)
            return;

        if (!_npcManager.AreThereRemainingPedestrians())
        {
            _runEnding = true;
            StartCoroutine(RunEndingRoutine());
            return;
        }

        if (_noMorePedestrians)
            return;

        if (_waitingForNextPedestrian)
            HandleWaitingForPedestrian();

        if (ShouldIncrementWave())
            OnIncrementWave();
    }

    private IEnumerator RunEndingRoutine()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("End of run results:");

        var criticallyBadWaveTraits = new Dictionary<WaveTrait, float>();
        foreach (var waveTrait in EnumHelper.GetValues<WaveTrait>())
        {
            var failCount = _waveResults.Where(x => !x.IsSuccessful && x.WorstTrait == waveTrait).Count();
            var purityChange = -1f * failCount * PurityChangePerFailure;
            criticallyBadWaveTraits.Add(waveTrait, purityChange);

            stringBuilder.AppendLine($"{waveTrait}: {failCount}x failures ({purityChange})");
        }

        _worldStateManager.AdjustBodyPurity(criticallyBadWaveTraits[WaveTrait.Body]);
        _worldStateManager.AdjustMindPurity(criticallyBadWaveTraits[WaveTrait.Mind]);
        _worldStateManager.AdjustSoulPurity(criticallyBadWaveTraits[WaveTrait.Spirit]);
        bool wasCreditsRun = _worldStateManager.IsCreditsRun();
        var wasFinalRun = _worldStateManager.OnRunEnd();

        Debug.Log(stringBuilder.ToString());

        if (wasFinalRun)
            Debug.Log("Final run finished");

        yield return new WaitForSeconds(3f);

        // reload the scene if we have another run
        if (!wasCreditsRun)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        yield return null;
    }

    private void HandleWaitingForPedestrian()
    {
        if (!_npcManager.IsCurrentPedestrianReadyToTeleport())
            return;

        _waveManager.ShowWaves();
        _waitingForNextPedestrian = false;
    }

    private void OnIncrementWave()
    {
        // Record the wave results
        var isSuccesful = _waveManager.WasWaveSuccessful(out var worstWaveTrait);
        _waveResults.Add(new(isSuccesful, worstWaveTrait));

        _waveManager.HideWaves();
        _waveManager.ReinitializeWave();

        if (_npcManager.AdvanceQueue())
            _waitingForNextPedestrian = true;
        else
            _noMorePedestrians = true;
    }

    private bool ShouldIncrementWave()
    {
        var spacePressed = Input.GetKeyDown(KeyCode.Space);

        if (!spacePressed || !_npcManager.AreThereRemainingPedestrians())
            return false;

        if (AllowTeleportAtAnyTime)
            return true;

        return !_waitingForNextPedestrian && _npcManager.IsCurrentPedestrianReadyToTeleport();
    }

    private struct WaveResult
    {
        public WaveResult(bool isSuccessful, WaveTrait worstTrait)
        {
            IsSuccessful = isSuccessful;
            WorstTrait = worstTrait;
        }

        public bool IsSuccessful { get; }
        public WaveTrait WorstTrait { get; }
    }
}
