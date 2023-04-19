using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Player.StateMachineBehaviour
{
    public class EllenStaffEffect : UnityEngine.StateMachineBehaviour
    {
        public int effectIndex;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController ctrl = animator.GetComponent<PlayerController>();

            ctrl.meleeWeapon.effects[effectIndex].Activate();
        }

    } 
}
