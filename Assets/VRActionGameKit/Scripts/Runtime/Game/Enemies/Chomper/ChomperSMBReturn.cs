﻿using _3DGamekitLite.Scripts.Runtime.Game.Core;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Enemies.Chomper
{
    public class ChomperSMBReturn : SceneLinkedSMB<ChomperBehavior>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.WalkBackToBase();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);

            m_MonoBehaviour.FindTarget();

            if(m_MonoBehaviour.target != null)
                m_MonoBehaviour.StartPursuit(); // if the player got back in our vision range, resume pursuit!
            else 
                m_MonoBehaviour.WalkBackToBase();
        }
    }
}