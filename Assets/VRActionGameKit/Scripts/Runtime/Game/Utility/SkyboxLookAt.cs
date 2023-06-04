using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Utility
{
    public class SkyboxLookAt : MonoBehaviour
    {

        public Transform target;

        void Update()
        {
            transform.LookAt(target);
        }
    } 
}
