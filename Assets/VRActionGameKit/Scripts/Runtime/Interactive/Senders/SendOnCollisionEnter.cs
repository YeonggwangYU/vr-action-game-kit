using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Senders
{

    public class SendOnCollisionEnter : SendGameCommand
    {
        public LayerMask layers;

        void OnCollisionEnter(Collision collision)
        {
            if (0 != (layers.value & 1 << collision.gameObject.layer))
            {
                Send();
            }
        }
    }

}
