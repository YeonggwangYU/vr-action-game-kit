using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Senders
{

    public class SendOnCollisionStay : SendGameCommand
    {
        public LayerMask layers;

        void OnCollisionStay(Collision collision)
        {
            if (0 != (layers.value & 1 << collision.gameObject.layer))
            {
                Send();
            }
        }
    }

}
