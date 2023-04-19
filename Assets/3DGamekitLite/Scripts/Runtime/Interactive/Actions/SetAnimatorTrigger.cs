using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Actions
{

    public class SetAnimatorTrigger : GameCommandHandler
    {
        public Animator animator;
        public string triggerName;

        void Reset()
        {
            animator = GetComponent<Animator>();
        }

        public override void PerformInteraction()
        {
            if (animator) animator.SetTrigger(triggerName);
        }
    }
}
