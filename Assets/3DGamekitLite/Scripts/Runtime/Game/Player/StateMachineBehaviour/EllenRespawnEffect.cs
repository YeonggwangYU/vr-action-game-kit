using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Player.StateMachineBehaviour
{
    public class EllenRespawnEffect : UnityEngine.StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<PlayerController>().Respawn();
        }
    } 
}