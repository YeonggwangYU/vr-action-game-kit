﻿using _3DGamekitLite.Scripts.Runtime.Game.Core;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies.Spitter
{
    public class SpitterSMBFleeing : SceneLinkedSMB<SpitterBehaviour>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.controller.SetFollowNavmeshAgent(true);
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.FindTarget();
            m_MonoBehaviour.CheckNeedFleeing();
        }
    }
}