using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Senders
{
    public abstract class TriggerCommand : SendGameCommand
    {
        protected override void Reset()
        {
            if (LayerMask.LayerToName(gameObject.layer) == "Default")
                gameObject.layer = LayerMask.NameToLayer("Environment");
            var c = GetComponent<Collider>();
            if (c != null)
                c.isTrigger = true;
            
            base.Reset ();
        }
    }

}
