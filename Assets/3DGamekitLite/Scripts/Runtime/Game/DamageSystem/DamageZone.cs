using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class DamageZone : MonoBehaviour
    {
        public int damageAmount = 1;
        public bool stopCamera = false;

        private void Reset()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            var d = other.GetComponent<Damageable>();

            //process for vr 
            var d_vr = other.GetComponentInChildren<Damageable>();

            var msg = new Damageable.DamageMessage()
            {
                amount = damageAmount,
                damager = this,
                direction = Vector3.up,
                stopCamera = stopCamera
            };

            if (d != null)
            {
                d.ApplyDamage(msg);
            }
            //process for vr 
            else if (d_vr != null)
            {
                d_vr.ApplyDamage(msg);
            }
        }
    } 
}
