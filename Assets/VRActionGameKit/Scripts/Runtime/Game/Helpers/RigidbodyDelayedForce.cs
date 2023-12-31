﻿using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Helpers
{
    public class RigidbodyDelayedForce : MonoBehaviour
    {
        public Vector3 forceToAdd;

        private void Start()
        {
            Rigidbody[] rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();

            for (int i = 0; i < rigidbodies.Length; ++i)
            {
                rigidbodies[i].maxAngularVelocity = 45;
                rigidbodies[i].angularVelocity = transform.right * -45.0f;
                rigidbodies[i].velocity = forceToAdd;

            }
        }
    } 
}
