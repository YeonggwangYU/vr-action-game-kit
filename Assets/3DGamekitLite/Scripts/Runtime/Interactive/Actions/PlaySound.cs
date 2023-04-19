using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Actions
{
    public class PlaySound : GameCommandHandler
    {
        public AudioSource[] audioSources;

        public override void PerformInteraction()
        {
            foreach (var a in audioSources)
                a.Play();
        }

    }
}
