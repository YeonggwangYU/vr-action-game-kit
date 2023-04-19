using _3DGamekitLite.Scripts.Runtime.Game.Player;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.DamageSystem
{
    [RequireComponent(typeof(Collider))]
    public class DeathVolume : MonoBehaviour
    {
        public new AudioSource audio;


        void OnTriggerEnter(Collider other)
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.Die(new Damageable.DamageMessage());
            }

            //process for vr 
            var pc_vr = other.GetComponentInChildren<PlayerController>();
            if (pc_vr != null)
            {
                pc_vr.Die(new Damageable.DamageMessage());
            }
            
            if (audio != null)
            {
                audio.transform.position = other.transform.position;
                if (!audio.isPlaying)
                    audio.Play();
            }
            
            
        }

        void Reset()
        {
            if (LayerMask.LayerToName(gameObject.layer) == "Default")
                gameObject.layer = LayerMask.NameToLayer("Environment");
            var c = GetComponent<Collider>();
            if (c != null)
                c.isTrigger = true;
        }

    }
}
