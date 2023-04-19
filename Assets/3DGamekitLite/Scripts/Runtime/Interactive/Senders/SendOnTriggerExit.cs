using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Senders
{

    public class SendOnTriggerExit : TriggerCommand
    {
        public LayerMask layers;

        void OnTriggerExit(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
            {
                Send();
            }
        }
    }

}
