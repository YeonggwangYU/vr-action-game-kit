﻿using System.Collections;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Weapon
{
    public class TimeEffect : MonoBehaviour
    {
        public Light staffLight;
        
        Animation m_Animation;

        void Awake()
        {
            m_Animation = GetComponent<Animation>();

            gameObject.SetActive(false);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            staffLight.enabled = true;

            if (m_Animation)
                m_Animation.Play();

            StartCoroutine(DisableAtEndOfAnimation());
        }

        IEnumerator DisableAtEndOfAnimation()
        {
            yield return new WaitForSeconds(m_Animation.clip.length);

            gameObject.SetActive(false);
            staffLight.enabled = false;
        }
    } 
}
