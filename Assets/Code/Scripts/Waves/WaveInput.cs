using System.Collections.Generic;
using AWITLOTF.Assets.Code.Scripts.Interface;
using UnityEngine;

public class WaveInput : MonoBehaviour
{
    


    // Right now it does it by keys out of simplicity
    private static readonly Dictionary<int, (KeyCode, KeyCode)> InputDict = new Dictionary<int, (KeyCode, KeyCode)>()
    {
        { 1, (KeyCode.Q, KeyCode.A) },
        { 2, (KeyCode.W, KeyCode.S) },
        { 3, (KeyCode.E, KeyCode.D) },
    };

    public bool HasInput { get; private set; } = false;
    public float PercentChange { get; private set; }
    public int InputChange { get; private set; }

    [SerializeField]
    private Knob knob;
    private ComponentWave ComponentWave => GetComponent<ComponentWave>();
    private float WavePercentChangeSpeed => FindFirstObjectByType<GlobalWaveProperties>().WavePercentChangeSpeed;

    // Update is called once per frame
    void Update()
    {
        InputChange = 0;
        PercentChange = 0f;
        HasInput = TryGetInputChange(ComponentWave, out var inputChange);

        if (!HasInput)
            return;

        InputChange = inputChange;

        var sign = inputChange > 0f ? 1f : -1f;
        PercentChange = sign * Time.deltaTime * WavePercentChangeSpeed;

        if(knob != null)
        {
            // set knob value
            //min max
            float pct = ComponentWave.GetCurrentVariableValueNormalized();
            knob.TurnToPercent(pct);
        }
    }

    private bool TryGetInputChange(ComponentWave componentWave, out int inputChange)
    {
        inputChange = 0;
        if (componentWave == null)
            return false;

        if (!InputDict.ContainsKey((int)componentWave.WaveTrait))
        {
            Debug.LogError($"{name} - Bad wave num ({componentWave.WaveTrait})");
            enabled = false;
            return false;
        }

        var (upKey, downKey) = InputDict[(int)componentWave.WaveTrait];

        if (Input.GetKey(upKey))
            inputChange--;
        if (Input.GetKey(downKey))
            inputChange++;

        return inputChange != 0;
    }
}
