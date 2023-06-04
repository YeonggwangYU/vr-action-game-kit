using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Actions
{
    public class ToggleGameObjectActive : GameCommandHandler
    {
        public GameObject[] targets;

        public override void PerformInteraction()
        {
            foreach (var g in targets)
                g.SetActive(!g.activeSelf);
        }
    }
}
