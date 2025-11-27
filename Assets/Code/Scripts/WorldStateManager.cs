using AWITLOTF.Assets.Code.Scripts.Npc;
using UnityEngine;

namespace AWITLOTF.Assets.Code.Scripts
{
    public class WorldStateManager : MonoBehaviour
    {
        NpcManager npcManager;

        public int BodyPurity = 3;
        public int MindPurity = 3;
        public int SoulPurity = 3;

        void Start()
        {
            npcManager = FindObjectOfType<NpcManager>();
        }
        public void AdjustBodyPurity(int amount)
        {
            BodyPurity += amount;
            BodyPurity = Mathf.Clamp(BodyPurity, 1, 3);
        }
        public void AdjustMindPurity(int amount)
        {
            MindPurity += amount;
            MindPurity = Mathf.Clamp(MindPurity, 1, 3);
        }
        public void AdjustSoulPurity(int amount)
        {
            SoulPurity += amount;
            SoulPurity = Mathf.Clamp(SoulPurity, 1, 3);
        }

        public void AdjustNpcManagerToWorldState()
        {
            if (BodyPurity < 3)
            {
                npcManager.fishyChance = 0.2f * (3 - BodyPurity);
                npcManager.gimpyArmChance = 0.3f * (3 - BodyPurity);
                npcManager.amorphousChance = 0.1f * (3 - BodyPurity);
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

    }
}