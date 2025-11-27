using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

namespace AWITLOTF.Assets.Code.Scripts.Npc
{
    public class Npc : MonoBehaviour
    {
        NavMeshAgent agent;
        Animator anim;
        NpcManager npcManager;

        public NpcTarget currentTarget;

        [Header("Face Camera Stuff")]
        public Transform headTransform;
        public Transform camSpotTransform;

        [Header("Animation Settings")]
        public AnimationType animationType = AnimationType.Normal;
        public enum AnimationType
        {
            Normal,
            Lean,
            WalkWithPhone,
            Pockets,
            Fat,
            Fishy,
            Drunk

        }

        [Header("Appearance Settings")]
        public NpcType npcType = NpcType.Random;
        public enum NpcType
        {
            Random,
            Dirtbag1,
            Dirtbag2,
            Dirtbag3,
            TSA
        }
        public bool male = true;

        public List<Material> fabricMaterials;
        public List<Material> brandedClothingMaterials;

        public SkinnedMeshRenderer cellphone;

        //mens
        public List<Material> M_skinMaterials;
        public SkinnedMeshRenderer M_shirtless;
        public List<SkinnedMeshRenderer> M_longSleeveTops;
        public List<SkinnedMeshRenderer> M_shortSleeveTops;
        public List<SkinnedMeshRenderer> M_pants;
        public SkinnedMeshRenderer M_shoes;
        public SkinnedMeshRenderer M_shoesFishy;
        public SkinnedMeshRenderer M_head;
        public SkinnedMeshRenderer M_headFishy;

        public SkinnedMeshRenderer M_hands;
        public SkinnedMeshRenderer M_handsFishy;
        public SkinnedMeshRenderer M_arms;
        public SkinnedMeshRenderer M_armsFishy;

        // womens
        public List<Material> F_skinMaterials;
        public List<SkinnedMeshRenderer> F_longSleeveTops;
        public List<SkinnedMeshRenderer> F_shortSleeveTops;
        public List<SkinnedMeshRenderer> F_pants;
        public SkinnedMeshRenderer F_shoes;
        public SkinnedMeshRenderer F_shoesFishy;
        public SkinnedMeshRenderer F_head;
        public SkinnedMeshRenderer F_headFishy;
        public SkinnedMeshRenderer F_hands;
        public SkinnedMeshRenderer F_handsFishy;
        public SkinnedMeshRenderer F_arms;
        public SkinnedMeshRenderer F_armsFishy;

        //dirtbag 1
        public List<SkinnedMeshRenderer> Dirtbag1_Skins;

        //dirtbag 2
        public List<SkinnedMeshRenderer> Dirtbag2_Skins;

        //dirtbag 3
        public List<SkinnedMeshRenderer> Dirtbag3_Skins;

        //TSA
        public List<SkinnedMeshRenderer> TSA_Skins;


        [Header("Fishy Scale")]
        public GameObject head;
        public Vector3 headFishyScale = new Vector3(2f, 1f, 1f);
        public GameObject[] shoulders;
        public Vector3 shouldersFishyScale = new Vector3(0.7f, 0.7f, 0.7f);
        public GameObject[] forearms;
        public Vector3 forearmsFishyScale = new Vector3(2, 2, 3);
        public GameObject[] thighs;
        public Vector3 thighsFishyScale = new Vector3(1, 1.4f, 1);
        public GameObject[] calves;
        public Vector3 calvesFishyScale = new Vector3(0.9f, 0.6f, 0.9f);





        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            npcManager = FindObjectOfType<NpcManager>();

            SkinSetup();

            switch (animationType)
            {
                case AnimationType.Normal:
                    anim.SetBool("isFat", false);
                    anim.SetBool("leaning", false);
                    anim.SetBool("walkingWithPhone", false);
                    anim.SetBool("handsInPockets", false);
                    anim.SetBool("isFish", false);
                    anim.SetBool("isDrunk", false);
                    break;
                case AnimationType.Lean:
                    anim.SetBool("isFat", false);
                    anim.SetBool("leaning", true);
                    anim.SetBool("walkingWithPhone", false);
                    anim.SetBool("handsInPockets", false);
                    anim.SetBool("isFish", false);
                    anim.SetBool("isDrunk", false);
                    break;
                case AnimationType.WalkWithPhone:
                    anim.SetBool("isFat", false);
                    anim.SetBool("leaning", false);
                    anim.SetBool("walkingWithPhone", true);
                    anim.SetBool("handsInPockets", false);
                    anim.SetBool("isFish", false);
                    anim.SetBool("isDrunk", false);
                    cellphone.enabled = true;
                    break;
                case AnimationType.Pockets:
                    anim.SetBool("isFat", false);
                    anim.SetBool("leaning", false);
                    anim.SetBool("walkingWithPhone", false);
                    anim.SetBool("handsInPockets", true);
                    anim.SetBool("isFish", false);
                    anim.SetBool("isDrunk", false);
                    break;
                case AnimationType.Fat:
                    anim.SetBool("isFat", true);
                    anim.SetBool("leaning", false);
                    anim.SetBool("walkingWithPhone", false);
                    anim.SetBool("handsInPockets", false);
                    anim.SetBool("isFish", false);
                    anim.SetBool("isDrunk", false);
                    break;
                case AnimationType.Fishy:
                    anim.SetBool("isFat", false);
                    anim.SetBool("leaning", false);
                    anim.SetBool("walkingWithPhone", false);
                    anim.SetBool("handsInPockets", false);
                    anim.SetBool("isFish", true);
                    anim.SetBool("isDrunk", false);
                    ApplyFishyChanges();
                    break;
                case AnimationType.Drunk:
                    anim.SetBool("isFat", false);
                    anim.SetBool("leaning", false);
                    anim.SetBool("walkingWithPhone", false);
                    anim.SetBool("handsInPockets", false);
                    anim.SetBool("isFish", false);
                    anim.SetBool("isDrunk", true);
                    break;
                default:
                    anim.SetBool("isFat", false);
                    anim.SetBool("leaning", false);
                    anim.SetBool("walkingWithPhone", false);
                    anim.SetBool("handsInPockets", false);
                    anim.SetBool("isFish", false);
                    anim.SetBool("isDrunk", false);
                    break;
            }
        }


        private void ApplyFishyChanges()
        {
            head.transform.localScale = headFishyScale;
            foreach (GameObject shoulder in shoulders)
            {
                shoulder.transform.localScale = shouldersFishyScale;
            }
            foreach (GameObject forearm in forearms)
            {
                forearm.transform.localScale = forearmsFishyScale;
            }
            foreach (GameObject thigh in thighs)
            {
                thigh.transform.localScale = thighsFishyScale;
            }
            foreach (GameObject calf in calves)
            {
                calf.transform.localScale = calvesFishyScale;
            }
            if (male)
            {
                M_shoes.enabled = false;
                M_shoesFishy.enabled = true;
                M_head.enabled = false;
                M_headFishy.enabled = true;
                M_hands.enabled = false;
                M_handsFishy.enabled = true;
                M_arms.enabled = false;
                M_armsFishy.enabled = true;
            }
            else
            {
                F_shoes.enabled = false;
                F_shoesFishy.enabled = true;
                F_head.enabled = false;
                F_headFishy.enabled = true;
                F_hands.enabled = false;
                F_handsFishy.enabled = true;
                F_arms.enabled = false;
                F_armsFishy.enabled = true;

            }
        }



        private void SkinSetup()
        {
            List<SkinnedMeshRenderer> allRenderers = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            foreach (SkinnedMeshRenderer smr in allRenderers)
            {
                smr.enabled = false;
            }
            switch (npcType)
            {
                case NpcType.Random:
                    CreateRandomNpc();
                    break;
                case NpcType.Dirtbag1:
                    foreach (SkinnedMeshRenderer smr in Dirtbag1_Skins)
                    {
                        smr.enabled = true;
                    }
                    break;
                case NpcType.Dirtbag2:
                    foreach (SkinnedMeshRenderer smr in Dirtbag2_Skins)
                    {
                        smr.enabled = true;
                    }
                    break;
                case NpcType.Dirtbag3:
                    foreach (SkinnedMeshRenderer smr in Dirtbag3_Skins)
                    {
                        smr.enabled = true;
                    }
                    break;
                case NpcType.TSA:
                    foreach (SkinnedMeshRenderer smr in TSA_Skins)
                    {
                        smr.enabled = true;
                    }
                    break;
                default:
                    break;

            }

        }
        private void SetRandomAnimationState()
        {

                animationType = AnimationType.Normal;
                float random = UnityEngine.Random.value;

                if(random < npcManager.amorphousChance)
                {
                    //TODO: blob them.
                    return;
                }
                random = UnityEngine.Random.value;
                if(random < npcManager.fishyChance)
                {
                    animationType = AnimationType.Fishy;
                    return;
                }
                random = UnityEngine.Random.value;
                if (random < npcManager.drunkennessChance)
            {
                animationType = AnimationType.Drunk;
                return;
            }
            random = UnityEngine.Random.value;
            if(random < npcManager.fatChance)
            {
                animationType = AnimationType.Fat;
                return;
            }
            random = UnityEngine.Random.value;
            if(random < npcManager.phoneChance)
            {
                animationType = AnimationType.WalkWithPhone;
                return;
            }

            //if we still dont have any, random between normal, lean, pockets
            int animType = Random.Range(0, 3);
            switch (animType)
            {
                case 0:
                    animationType = AnimationType.Normal;
                    break;
                case 1:
                    animationType = AnimationType.Lean;
                    break;
                case 2:
                    animationType = AnimationType.Pockets;
                    break;
                default:
                    animationType = AnimationType.Normal;
                    break;
            }
        }
        private void CreateRandomNpc()
        {
            // Randomly choose gender
            male = Random.value > 0.5f;

            SetRandomAnimationState();

            if (male)
            {
                // Enable male body parts
                M_head.enabled = true;
                M_hands.enabled = true;
                M_shoes.enabled = true;

                // Randomly choose skin material
                Material skinMat = M_skinMaterials[Random.Range(0, M_skinMaterials.Count)];
                M_head.material = skinMat;
                M_hands.material = skinMat;
                M_arms.material = skinMat;

                // Randomly choose between long or short sleeve
                bool longSleeve = Random.value > 0.5f;
                SkinnedMeshRenderer selectedTop;
                if (longSleeve)
                {
                    selectedTop = M_longSleeveTops[Random.Range(0, M_longSleeveTops.Count)];
                }
                else
                {
                    selectedTop = M_shortSleeveTops[Random.Range(0, M_shortSleeveTops.Count)];
                    M_arms.enabled = true;
                }
                float shirtlessRandom = Random.value;
                if (shirtlessRandom < npcManager.shirtlessChance)
                {
                    M_arms.enabled = false;
                    selectedTop = M_shirtless;
                }
                selectedTop.enabled = true;

                // Randomly choose pants
                SkinnedMeshRenderer selectedPants = M_pants[Random.Range(0, M_pants.Count)];
                selectedPants.enabled = true;


                bool shirtBranded = Random.value < npcManager.brandedClothesChance;
                bool pantsBranded = Random.value < npcManager.brandedClothesChance;
                if (shirtBranded)
                {
                    selectedTop.material = brandedClothingMaterials[Random.Range(0, brandedClothingMaterials.Count)];
                }
                else
                {
                    selectedTop.material = fabricMaterials[Random.Range(0, fabricMaterials.Count)];
                }
                if (pantsBranded)
                {
                    selectedPants.material = brandedClothingMaterials[Random.Range(0, brandedClothingMaterials.Count)];
                }
                else
                {
                    selectedPants.material = fabricMaterials[Random.Range(0, fabricMaterials.Count)];
                }

                // Apply random fabric materials to clothing
                M_shoes.material = fabricMaterials[Random.Range(0, fabricMaterials.Count)];
            }
            else
            {
                // Enable female body parts
                F_head.enabled = true;
                F_hands.enabled = true;
                F_shoes.enabled = true;

                // Randomly choose skin material
                Material skinMat = F_skinMaterials[Random.Range(0, F_skinMaterials.Count)];
                F_head.material = skinMat;
                F_hands.material = skinMat;
                F_arms.material = skinMat;

                // Randomly choose between long or short sleeve
                bool longSleeve = Random.value > 0.5f;
                SkinnedMeshRenderer selectedTop;
                if (longSleeve)
                {
                    selectedTop = F_longSleeveTops[Random.Range(0, F_longSleeveTops.Count)];
                }
                else
                {
                    selectedTop = F_shortSleeveTops[Random.Range(0, F_shortSleeveTops.Count)];
                    F_arms.enabled = true;
                }
                selectedTop.enabled = true;

                // Randomly choose pants
                SkinnedMeshRenderer selectedPants = F_pants[Random.Range(0, F_pants.Count)];
                selectedPants.enabled = true;

                // Apply random fabric materials to clothing
                bool shirtBranded = Random.value < npcManager.brandedClothesChance;
                bool pantsBranded = Random.value < npcManager.brandedClothesChance;
                if (shirtBranded)
                {
                    selectedTop.material = brandedClothingMaterials[Random.Range(0, brandedClothingMaterials.Count)];
                }
                else
                {
                    selectedTop.material = fabricMaterials[Random.Range(0, fabricMaterials.Count)];
                }
                if (pantsBranded)
                {
                    selectedPants.material = brandedClothingMaterials[Random.Range(0, brandedClothingMaterials.Count)];
                }
                else
                {
                    selectedPants.material = fabricMaterials[Random.Range(0, fabricMaterials.Count)];
                }
                F_shoes.material = fabricMaterials[Random.Range(0, fabricMaterials.Count)];
            }
        }

        public void SetTarget(NpcTarget target)
        {
            currentTarget = target;
            agent.SetDestination(target.transform.position);
        }

        void Update()
        {
            anim.SetFloat("moveSpeed", agent.velocity.magnitude);
            //face direction of target- lookAtTarget
            if (currentTarget != null && currentTarget.lookAtTarget != null)
            {
                Vector3 lookDirection = currentTarget.lookAtTarget.transform.position - transform.position;
                lookDirection.y = 0; // keep only horizontal rotation
                if (lookDirection != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }
        }

        public void SetTalking(bool isTalking)
        {
            anim.SetBool("talking", isTalking);   
        }

    }
}