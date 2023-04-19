using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Actions
{
    public class ParticleSystemEmit : GameCommandHandler
    {
        public ParticleSystem[] particleSystems;
        public int count;

        public override void PerformInteraction()
        {
            foreach (var ps in particleSystems)
            {
                ps.Emit(count);
            }
        }
    }
}
