﻿using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Helpers
{
    public class ParticleDeactivator : MonoBehaviour
    {
        public float duration;

        protected float m_SinceActivation = 0.0f;
        protected ParticleSystem m_ParticleSystem;

        void OnEnable()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            m_SinceActivation = 0;
        }

        void Update()
        {
            m_SinceActivation += Time.deltaTime;
            if (m_SinceActivation > duration)
            {
                m_ParticleSystem.Stop(true);
                gameObject.SetActive(false);
            }
        }
    } 
}
