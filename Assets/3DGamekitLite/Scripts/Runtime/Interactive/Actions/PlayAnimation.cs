using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Actions
{
    public class PlayAnimation : GameCommandHandler
    {
        public Animation[] animations;

        public override void PerformInteraction()
        {
            foreach (var a in animations)
                a.Play();
        }
    }
}
