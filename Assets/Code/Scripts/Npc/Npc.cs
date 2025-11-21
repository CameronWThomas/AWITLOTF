using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

namespace AWITLOTF.Assets.Code.Scripts.Npc
{
    public class Npc: MonoBehaviour
    {
       NavMeshAgent agent;
       Animator anim;

       public bool uniqueSkin = false;
       public NpcTarget currentTarget;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();

            //TODO: random skin if not unique
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

    }
}