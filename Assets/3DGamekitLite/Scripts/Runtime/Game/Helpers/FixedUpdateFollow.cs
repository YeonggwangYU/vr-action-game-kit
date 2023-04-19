using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Helpers
{
    [DefaultExecutionOrder(9999)]
    public class FixedUpdateFollow : MonoBehaviour
    {
        public Transform toFollow;

        private void FixedUpdate()
        {
            transform.position = toFollow.position;
            transform.rotation = toFollow.rotation;
        }
    } 
}
