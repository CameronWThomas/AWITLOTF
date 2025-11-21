using System.Collections.Generic;
using UnityEngine;

namespace AWITLOTF.Assets.Code.Scripts.Npc
{
    public class NpcManager : MonoBehaviour
    {
        public List<Npc> pedestrians;
        public List<Npc> tsa;

        public int currentPedestrianIndex = 0;

        public NpcTarget teleporterPosition;
        public List<NpcTarget> pedestrianQueues;
        public List<NpcTarget> tsaPositions;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetUpInitialTargets();

        }

        public void SetUpInitialTargets()
        {
            for (int i = 0; i < pedestrians.Count; i++)
            {
                pedestrians[i].SetTarget(pedestrianQueues[i]);
            }

            for (int i = 0; i < tsa.Count; i++)
            {
                tsa[i].SetTarget(tsaPositions[i]);
            }

        }

    public void AdvanceQueue()
    {
        if (currentPedestrianIndex < pedestrians.Count)
        {
            pedestrians[currentPedestrianIndex].SetTarget(teleporterPosition);
            currentPedestrianIndex++;

            //advance everyone else in the queue
            for (int i = currentPedestrianIndex; i < pedestrians.Count; i++)
            {
                pedestrians[i].SetTarget(pedestrianQueues[i - currentPedestrianIndex]);
            }

        }
        else
        {
            Debug.Log("All pedestrians have been advanced.");


        }
    }        // Update is called once per frame
        void Update()
        {

        }
    }
}
