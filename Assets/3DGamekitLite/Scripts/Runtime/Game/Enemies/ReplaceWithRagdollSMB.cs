using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies
{
    public class ReplaceWithRagdollSMB : UnityEngine.StateMachineBehaviour
    {
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            ReplaceWithRagdoll replacer = animator.GetComponent<ReplaceWithRagdoll>();
            replacer.Replace();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ReplaceWithRagdoll replacer = animator.GetComponent<ReplaceWithRagdoll>();
            replacer.Replace();
        }
    }
}