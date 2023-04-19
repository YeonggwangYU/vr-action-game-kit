using _3DGamekitLite.Scripts.Runtime.Game.Core;
using _3DGamekitLite.Scripts.Runtime.Game.Enemies.Chomper;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies.Spitter
{
    public class SpitterSMBHit : SceneLinkedSMB<SpitterBehaviour>
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