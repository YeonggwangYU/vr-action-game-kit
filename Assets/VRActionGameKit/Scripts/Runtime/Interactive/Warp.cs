using System;
using _3DGamekitLite.Scripts.Runtime.Game.Player;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive
{
    public class Warp : MonoBehaviour
    {
        /// <Summary>
        /// use to move player after beating enemy. 
        /// </Summary>
        [SerializeField] private GameObject player;

        /// <Summary>
        /// player move point after beat enemy. 
        /// </Summary>
        [SerializeField] private GameObject clearWarpPoint;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<XROriginController>(out XROriginController _xrOriginControllerControllerIdentification))
            {
                player.transform.position = clearWarpPoint.transform.position;
                player.transform.rotation = clearWarpPoint.transform.rotation;
            }
        }

    }
}