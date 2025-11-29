using AWITLOTF.Assets.Code.Scripts.Npc;
using UnityEngine;

namespace AWITLOTF.Assets.Code.Scripts
{
    public class WorldStateManager : MonoBehaviour
    {
        NpcManager npcManager;

        [Range(1f, 3f)] public float BodyPurity = 3f;
        [Range(1f, 3f)] public float MindPurity = 3f;
        [Range(1f, 3f)] public float SoulPurity = 3f;
        void Awake()
        {
            GlobalStateManager globalStateManager = GetGlobalStateManager();
            BodyPurity = globalStateManager.BodyPurity;
            MindPurity = globalStateManager.MindPurity;
            SoulPurity = globalStateManager.SoulPurity;

            npcManager = GetComponent<NpcManager>();
            AdjustNpcManagerToWorldState();
        }

        public void AdjustBodyPurity(float amount)
        {
            BodyPurity += amount;
            BodyPurity = Mathf.Clamp(BodyPurity, 1f, 3f);
        }

        public void AdjustMindPurity(float amount)
        {
            MindPurity += amount;
            MindPurity = Mathf.Clamp(MindPurity, 1f, 3f);
        }

        public void AdjustSoulPurity(float amount)
        {
            SoulPurity += amount;
            SoulPurity = Mathf.Clamp(SoulPurity, 1f, 3f);
        }

        /// <summary>
        /// Handles saving state about the world. Returns whether this run was the final one
        /// </summary>
        public bool OnRunEnd()
        {
            GlobalStateManager globalStateManager = GetGlobalStateManager();
            var wasFinalRun = globalStateManager.IsFinalRun;

            globalStateManager.SaveState(this);
            globalStateManager.IncrementRun();

            return wasFinalRun;
        }

        private void AdjustNpcManagerToWorldState()
        {
            if (BodyPurity < 3)
            {
                npcManager.fishyChance = 0.3f * (3 - BodyPurity);
                npcManager.gimpyArmChance = 0.4f * (3 - BodyPurity);
                npcManager.amorphousChance = 0.2f * (3 - BodyPurity);
            }
            if (MindPurity < 3)
            {
                npcManager.rhotacism = MindPurity <= 2;
                npcManager.illiterate = MindPurity <= 1;
                npcManager.smoothBrained = MindPurity <= 0;
            }
            if (SoulPurity < 3)
            {
                npcManager.shirtlessChance = 0.2f * (3 - SoulPurity);
                npcManager.phoneChance = 0.2f * (3 - SoulPurity);
                npcManager.fatChance = 0.2f * (3 - SoulPurity);
                npcManager.brandedClothesChance = 0.4f * (3 - SoulPurity);
                npcManager.drunkennessChance = 0.2f * (3 - SoulPurity);
            }
        }

        private GlobalStateManager GetGlobalStateManager()
        {
            var globalStateManager = FindFirstObjectByType<GlobalStateManager>();
            if (globalStateManager != null)
                return globalStateManager;

            var newGameObject = new GameObject("GLOBAL STATE MANAGER INSTANCE");
            globalStateManager = newGameObject.AddComponent<GlobalStateManager>();
            globalStateManager.SaveState(this);

            return globalStateManager;
        }
    }
}