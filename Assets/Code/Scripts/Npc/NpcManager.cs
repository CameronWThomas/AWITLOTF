using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AWITLOTF.Assets.Code.Scripts.Interface;
using TMPro;
using Unity.VisualScripting;
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

        public TeleporterSphere teleporterSphere;
        public Lever teleporterLever;
        public Coroutine dialogueCoroutine;
        WorldStateManager worldStateManager;

        [Header("World Modifiers")]
        // Body
        public float fishyChance = 0;
        public float gimpyArmChance = 0;
        public float amorphousChance = 0;
        // Mind
        public bool lisp = false;
        public bool illiterate = false;
        public bool smoothBrained = false;
        // Soul
        public float shirtlessChance = 0;
        public float phoneChance = .2f;
        public float fatChance = 0;
        public float brandedClothesChance = 0;
        public float drunkennessChance = 0;


        [Header("Dialogue Stuff")]
        public TextMeshPro dialogueText;
        public MeshRenderer speakingFaceRenderer;
        public Camera NpcFaceCamera;


        public List<TextAsset> randomDialogueAssets;

        public TextAsset phase1_dirtbag3Asset;
        public TextAsset phase1_dirtbag2Asset;

        public TextAsset phase2_dirtbag3Asset;
        public TextAsset phase2_dirtbag2Asset;


        public Npc currentSpeaker;
        public String dialogueString;
        public int currentDialogueLineIndex = 0;
        public string currentDialogueLine;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            teleporterLever = FindFirstObjectByType<Lever>();
            SetUpInitialTargets();
            dialogueText.text = "";
            speakingFaceRenderer.enabled = false;
            WorldStateManager worldStateManager = FindAnyObjectByType<WorldStateManager>();

            //advance queue on start

            if (worldStateManager != null)
            {
                //set world modifiers
                if (worldStateManager.IsCreditsRun())
                {
                    StartCoroutine(AdvanceQueueAutomatically());
                }
                else
                {

                    AdvanceQueue();
                }
            }
            else
            {

                AdvanceQueue();
            }

        }

        public bool AreThereRemainingPedestrians()
        {
            var lastPedestrians = pedestrians.LastOrDefault();
            return lastPedestrians != null && !lastPedestrians.IsDestroyed();
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

        public bool IsCurrentPedestrianReadyToTeleport()
        {
            var currentPedestrian = pedestrians[currentPedestrianIndex - 1];
            if (currentPedestrian.IsDestroyed())
                return false;

            var distance = (currentPedestrian.transform.position - teleporterPosition.transform.position).magnitude;
            if (distance > .5f)
                return false;

            return currentPedestrian.AgentSpeed < .05f;
        }

        /// <summary>
        /// Advances the queue and returns whether there are any more pedestrians in the queue
        /// </summary>
        public bool AdvanceQueue()
        {
            //destroy the old pedestrian
            if (currentPedestrianIndex != 0)
            {
                Npc currentPedestrian = pedestrians[currentPedestrianIndex - 1];
                if (currentPedestrian != null)
                {
                    Destroy(currentPedestrian.gameObject, 0.1f); //delay destroy to allow any final animations to
                    dialogueText.text = "";
                    // GameObject blob = currentPedestrian.blobInstance;
                    // if (blob != null)
                    // {
                    //     Destroy(blob, 0.1f);
                    // }
                }
                teleporterSphere.ActivateTeleporter();
                teleporterLever.ThrowLever();
            }
            if (currentPedestrianIndex < pedestrians.Count)
            {
                pedestrians[currentPedestrianIndex].SetTarget(teleporterPosition);
                currentPedestrianIndex++;

                //advance everyone else in the queue
                for (int i = currentPedestrianIndex; i < pedestrians.Count; i++)
                {
                    pedestrians[i].SetTarget(pedestrianQueues[i - currentPedestrianIndex]);
                }
                string pedName = pedestrians[currentPedestrianIndex - 1].name;

                bool db1 = pedName.Contains("dirtbag1");
                bool db2 = pedName.Contains("dirtbag2");
                if (db1)
                {
                    LoadDirtbagAsset(false);
                }
                else if (db2)
                {
                    LoadDirtbagAsset(true);
                }
                else
                    LoadRandomDialogueAsset();


                if (dialogueCoroutine != null)
                {
                    StopCoroutine(dialogueCoroutine);
                }
                dialogueCoroutine = StartCoroutine(WaitAndAdvanceDialogue(-1f));

                return true;
            }

            return false;
        }

        public IEnumerator WaitAndAdvanceDialogue(float waitTime)
        {
            if (waitTime <= 0f)
            {
                waitTime = UnityEngine.Random.Range(2f, 10f);
            }
            yield return new WaitForSeconds(waitTime);
            AdvanceDialogueLine();
        }

        public void AdvanceDialogueLine()
        {
            string[] dialogueLines = dialogueString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (currentDialogueLineIndex < dialogueLines.Length)
            {
                currentDialogueLine = dialogueLines[currentDialogueLineIndex];
                currentDialogueLineIndex++;

                if (currentSpeaker != null)
                {
                    currentSpeaker.SetTalking(false);
                }
                //try to determine current speaker.
                string[] splitLine = currentDialogueLine.Split(new[] { ':' }, 2);
                if (splitLine.Length == 2)
                {
                    string speakerTag = splitLine[0];
                    currentDialogueLine = splitLine[1];

                    //find npc with that tag
                    if (speakerTag == "current" && currentPedestrianIndex > 0 && currentPedestrianIndex <= pedestrians.Count)
                    {

                        currentSpeaker = pedestrians[currentPedestrianIndex - 1];
                    }
                    else if (speakerTag.Contains("dirtbag"))
                    {
                        //loop pedestrians and match gameobject name
                        currentSpeaker = pedestrians.FirstOrDefault(p => p != null && p.name.ToLower().Contains(speakerTag.ToLower()));
                    }
                    else if (speakerTag.StartsWith("tsa"))
                    {
                        int tsaIndex;
                        if (int.TryParse(speakerTag.Substring(3), out tsaIndex))
                        {
                            tsaIndex -= 1; //convert to 0 based index
                            if (tsaIndex >= 0 && tsaIndex < tsa.Count)
                            {
                                currentSpeaker = tsa[tsaIndex];
                            }
                            else
                            {
                                currentSpeaker = null;
                            }
                        }
                        else
                        {
                            currentSpeaker = null;
                        }
                    }
                    else
                    {
                        //panic
                        currentSpeaker = null;
                    }
                }

                if (currentSpeaker != null)
                {
                    currentSpeaker.SetTalking(true);
                    //position face camera
                    NpcFaceCamera.transform.position = currentSpeaker.camSpotTransform.position;
                    //npc face camera should look at npc head transform
                    NpcFaceCamera.transform.LookAt(currentSpeaker.headTransform);

                    //enable speaking face renderer
                    speakingFaceRenderer.enabled = true;
                }
                else
                {
                    speakingFaceRenderer.enabled = false;

                }

                dialogueText.text = ApplyWorldModifiersToDialogue(currentDialogueLine);
                if (dialogueCoroutine != null)
                {
                    StopCoroutine(dialogueCoroutine);
                }
                dialogueCoroutine = StartCoroutine(WaitAndAdvanceDialogue(-1f));
            }
            else
            {
                //Reset dialogue
                currentDialogueLineIndex++;
                dialogueText.text = "";
            }
        }

        public string ApplyWorldModifiersToDialogue(string line)
        {
            if (lisp)
            {
                line = line.Replace("s", "th").Replace("T", "Th");
            }
            if (illiterate)
            {
                string[] words = line.Split(' ');
                string newLine = "";
                for (int i = 0; i < words.Length; i++)
                {

                    // swap 2 random letters in the word
                    bool halfOds = UnityEngine.Random.value < 0.5f;
                    if (words[i].Length > 5 && halfOds)
                    {
                        char[] charArray = words[i].ToCharArray();
                        int index1 = UnityEngine.Random.Range(0, charArray.Length);
                        int index2 = UnityEngine.Random.Range(0, charArray.Length);
                        char temp = charArray[index1];
                        charArray[index1] = charArray[index2];
                        charArray[index2] = temp;
                        newLine += new string(charArray);
                    }
                    else
                    {
                        newLine += words[i];
                    }
                    //20% chance to add "uh" between words
                    if (i < words.Length - 1)
                    {
                        if (UnityEngine.Random.value < 0.2f)
                        {
                            bool halfOds2 = UnityEngine.Random.value < 0.5f;
                            if (halfOds2)
                                newLine += " uh";
                            else
                                newLine += " ...uh...";
                        }
                        newLine += " ";
                    }
                }
                line = newLine.Trim();
            }
            if (smoothBrained)
            {
                string[] options = new string[]{
                    "Uhhhhhh....",
                    "mmmmm",
                    "Ghuuummm....",
                    "Beeeeerrr",
                    "Hrrrrgh",
                    "Gun control just doesnt make any sense to me.",
                    "Shit!",
                    "Yaaaasss",
                    "gooogglle",
                    "Wuh?",
                    "What?",
                    "Bruhhh",
                    "Hmmm?"
                };
                line = options[UnityEngine.Random.Range(0, options.Length - 1)];
            }
            return line;
        }

        public void LoadRandomDialogueAsset()
        {
            int randomIndex = UnityEngine.Random.Range(0, randomDialogueAssets.Count);
            TextAsset selectedDialogue = randomDialogueAssets[randomIndex];
            dialogueString = selectedDialogue.text;
            currentDialogueLineIndex = 0;
            speakingFaceRenderer.enabled = false;
        }
        public void LoadDirtbagAsset(bool twoLeft = false)
        {
            GlobalStateManager gsm = FindAnyObjectByType<GlobalStateManager>();
            if (gsm == null)
            {
                LoadRandomDialogueAsset();
                return;
            }
            if (gsm.CurrentRunCount == 1)
            {
                TextAsset selectedDialogue = phase1_dirtbag3Asset;
                if (twoLeft)
                {
                    selectedDialogue = phase1_dirtbag2Asset;
                }
                dialogueString = selectedDialogue.text;
                currentDialogueLineIndex = 0;
                speakingFaceRenderer.enabled = false;
            }
            else
            {
                TextAsset selectedDialogue = phase2_dirtbag3Asset;
                if (twoLeft)
                {
                    selectedDialogue = phase2_dirtbag2Asset;
                }
                dialogueString = selectedDialogue.text;
                currentDialogueLineIndex = 0;
                speakingFaceRenderer.enabled = false;
            }

        }

        public IEnumerator AdvanceQueueAutomatically()
        {
            while (AreThereRemainingPedestrians())
            {
                bool advanced = AdvanceQueue();
                if (!advanced)
                    yield break;

                while (!IsCurrentPedestrianReadyToTeleport())
                {
                    yield return null;
                }

                float seconds = UnityEngine.Random.Range(2f, 10f);
                yield return new WaitForSeconds(seconds);
            }
        }




        void Update()
        {

            if (currentSpeaker != null)
            {
                //position face camera
                NpcFaceCamera.transform.position = currentSpeaker.camSpotTransform.position;
                //npc face camera should look at npc head transform
                NpcFaceCamera.transform.LookAt(currentSpeaker.headTransform);
            }

        }
    }
}