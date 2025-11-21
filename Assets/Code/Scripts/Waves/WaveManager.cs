using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public ComponentWave ComponentWave1;
    public ComponentWave ComponentWave2;
    public ComponentWave ComponentWave3;
    
    public CombinedWave CombinedWave;
    public GoalWave GoalWave;

    private void Start()
    {
        var waveInfos = WaveInfo.GetRandoms(3);
        ComponentWave1.SetWaveInfo(waveInfos[0]);
        ComponentWave2.SetWaveInfo(waveInfos[1]);
        ComponentWave3.SetWaveInfo(waveInfos[2]);

        if (CombinedWave != null)
            CombinedWave.Initialize(ComponentWave1, ComponentWave2, ComponentWave3);

        //TODO eventually there will no longer be a goal wave
        if (GoalWave != null)
            GoalWave.Initialize(ComponentWave1, ComponentWave2, ComponentWave3);
    }
}
