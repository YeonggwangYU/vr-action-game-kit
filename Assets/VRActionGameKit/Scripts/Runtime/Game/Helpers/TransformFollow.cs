using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Helpers
{
    public class TransformFollow : MonoBehaviour
    {
        public Transform target;

        private void LateUpdate()
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    } 
}
