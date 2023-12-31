﻿using UnityEngine;
using UnityEngine.Events;

namespace _3DGamekitLite.Scripts.Runtime.Game.Core
{
    [RequireComponent(typeof(Collider))]
    public class InteractOnCollision : MonoBehaviour
    {
        public LayerMask layers;
        public UnityEvent OnCollision;

        void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
        }

        void OnCollisionEnter(Collision c)
        {
            Debug.Log(c);
            if (0 != (layers.value & 1 << c.transform.gameObject.layer))
            {
                OnCollision.Invoke();
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InteractionTrigger", false);
        }

        void OnDrawGizmosSelected()
        {
            //need to inspect events and draw arrows to relevant gameObjects.
        }

    } 
}
