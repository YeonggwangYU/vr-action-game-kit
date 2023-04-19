using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Senders
{

    public class SendOnCollisionExit : SendGameCommand
    {
        public LayerMask layers;

        void OnCollisionExit(Collision collision)
        {
            if (0 != (layers.value & 1 << collision.gameObject.layer))
            {
                Send();
            }
        }
    }

}
