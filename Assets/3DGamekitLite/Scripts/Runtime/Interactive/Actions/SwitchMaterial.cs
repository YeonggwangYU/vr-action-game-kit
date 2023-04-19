using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Actions
{
    public class SwitchMaterial : GameCommandHandler
    {
        public Renderer target;
        public Material[] materials;
        int count;

        public override void PerformInteraction()
        {
            count++;
            target.material = materials[count % materials.Length];
        }
    }
}
