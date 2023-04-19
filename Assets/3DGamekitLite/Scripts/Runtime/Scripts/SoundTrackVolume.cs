﻿using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Scripts
{
    public class SoundTrackVolume : MonoBehaviour
    {
        public LayerMask layers;
        SoundTrack soundTrack;

        void OnEnable()
        {
            soundTrack = GetComponentInParent<SoundTrack>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
                soundTrack.PushTrack(this.name);
        }

        void OnTriggerExit(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
                soundTrack.PopTrack();
        }

    }
}
