using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Wave ComponentWave1;
    public Wave ComponentWave2;
    public Wave ComponentWave3;
    
    public Wave CombinedWave;
    public Wave GoalWave;

    private void Start()
    {
        var waveInfos = WaveInfo.GetRandoms(3);
        ComponentWave1.SetWaveInfos(waveInfos[0]);
        ComponentWave2.SetWaveInfos(waveInfos[1]);
        ComponentWave3.SetWaveInfos(waveInfos[2]);

        if (CombinedWave != null)
            CombinedWave.SetWaveInfos(waveInfos[0], waveInfos[1], waveInfos[2]);

        if (GoalWave != null)
        {
            var waveInfoCopies = waveInfos.Select(x => x.Copy()).ToArray();
            foreach (var waveInfoCopy in waveInfoCopies)
                waveInfoCopy.VariableValue = RandomHelper.Between(0.1f, 5f);

            GoalWave.SetWaveInfos(waveInfoCopies[0], waveInfoCopies[1], waveInfoCopies[2]);
        }
    }
}
