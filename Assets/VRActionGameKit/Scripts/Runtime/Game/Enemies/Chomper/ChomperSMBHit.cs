using _3DGamekitLite.Scripts.Runtime.Game.Core;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies.Chomper
{
    public class ChomperSMBHit : SceneLinkedSMB<ChomperBehavior>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger(ChomperBehavior.hashAttack);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.controller.ClearForce();
        }
    }
}