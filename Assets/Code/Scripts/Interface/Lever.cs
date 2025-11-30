using UnityEngine;

namespace AWITLOTF.Assets.Code.Scripts.Interface
{
    public class Lever:MonoBehaviour
    {
        Animator animator;
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void ThrowLever()
        {
            animator.SetTrigger("throw");
        }
        
    }
}