using UnityEngine.Events;

namespace _3DGamekitLite.Scripts.Runtime.Interactive.Actions
{

    public class TriggerUnityEvent : GameCommandHandler
    {
        public UnityEvent unityEvent;

        public override void PerformInteraction()
        {
            unityEvent.Invoke();
        }
    }
}
