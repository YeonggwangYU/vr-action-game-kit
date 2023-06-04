using _3DGamekitLite.Scripts.Runtime.Game.Core;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies.Grenadier
{
    public class GrenadierSMBPunch : SceneLinkedSMB<GrenadierBehaviour>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_MonoBehaviour.punchAudioPlayer)
                m_MonoBehaviour.punchAudioPlayer.PlayRandomClip();
        }
    }
}