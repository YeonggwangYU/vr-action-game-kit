using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Senders
{

    public class SendOnTriggerStay : TriggerCommand
    {
        public LayerMask layers;

        void OnTriggerStay(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
            {
                Send();
            }
        }
    }

}
