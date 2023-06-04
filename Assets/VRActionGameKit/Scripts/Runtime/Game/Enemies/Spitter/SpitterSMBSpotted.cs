using _3DGamekitLite.Scripts.Runtime.Game.Core;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies.Spitter
{
    public class SpitterSMBSpotted : SceneLinkedSMB<SpitterBehaviour>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.Spotted();
        }
    }
}