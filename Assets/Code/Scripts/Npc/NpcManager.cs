using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        [Header("Dialogue Stuff")]
        public TextMeshPro dialogueText;

        public List<TextAsset> randomDialogueAssets;

        public String dialogueString;
        public int currentDialogueLineIndex = 0;
        public string currentDialogueLine;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetUpInitialTargets();
            dialogueText.text = "";

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
                LoadRandomDialogueAsset();

                StartCoroutine(WaitAndAdvanceDialogue(-1f));

            }
        }    

        public IEnumerator WaitAndAdvanceDialogue(float waitTime)
        {
            if(waitTime <= 0f)
            {
                waitTime = UnityEngine.Random.Range(2f, 10f);
            }
            yield return new WaitForSeconds(waitTime);
            AdvanceDialogueLine();
        }

        public void AdvanceDialogueLine()
        {
            string[] dialogueLines = dialogueString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (currentDialogueLineIndex < dialogueLines.Length)
            {
                currentDialogueLine = dialogueLines[currentDialogueLineIndex];
                currentDialogueLineIndex++;

                dialogueText.text = currentDialogueLine;

                StartCoroutine(WaitAndAdvanceDialogue(-1f));
            }
            else
            {
                //Reset dialogue
                currentDialogueLineIndex++;
                dialogueText.text = "";
            }
        }

        public void LoadRandomDialogueAsset()
        {
            int randomIndex = UnityEngine.Random.Range(0, randomDialogueAssets.Count);
            TextAsset selectedDialogue = randomDialogueAssets[randomIndex];
            dialogueString = selectedDialogue.text;

        }


        

        void Update()
        {

        }
    }
}
