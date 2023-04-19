using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Player.StateMachineBehaviour
{
    public class EllenSetTargetableSMB : UnityEngine.StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController controller = animator.GetComponent<PlayerController>();

            if (controller != null)
            {
                controller.RespawnFinished();
            }
        }
    } 
}
