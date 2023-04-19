using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Utility
{
    [ExecuteInEditMode]
    public class SnapToGround : MonoBehaviour
    {
        public LayerMask layerMask;

        void OnDrawGizmosSelected()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + (Vector3.up * 10), Vector3.down, out hit, 1000, layerMask.value))
            {
                if (hit.collider.gameObject != gameObject)
                {
                    Gizmos.DrawLine(transform.position, hit.point);
                }
            }
        }
    } 
}
