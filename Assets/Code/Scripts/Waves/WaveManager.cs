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

        //if (CombinedWave != null)
        //    CombinedWave.SetWaveInfos(waveInfos[0], waveInfos[1], waveInfos[2]);

        //if (GoalWave != null)
        //{
        //    var waveInfoCopies = waveInfos.Select(x => x.Copy()).ToArray();
        //    foreach (var waveInfoCopy in waveInfoCopies)
        //        waveInfoCopy.VariableValue = RandomHelper.Between(0.1f, 5f);

        //    GoalWave.SetWaveInfos(waveInfoCopies[0], waveInfoCopies[1], waveInfoCopies[2]);
        //}
    }
}
