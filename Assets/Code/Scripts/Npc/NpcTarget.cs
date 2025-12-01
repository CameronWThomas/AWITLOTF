using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AWITLOTF.Assets.Code.Scripts.Npc
{
    public class NpcTarget: MonoBehaviour
    {

        public GameObject lookAtTarget;
        public List<MeshRenderer> debugRenderers;
        public void Start()
        {
            debugRenderers = GetComponentsInChildren<MeshRenderer>(true).ToList();
            foreach (var renderer in debugRenderers)
            {
                renderer.enabled = false;
            }
        }
        
    }
}