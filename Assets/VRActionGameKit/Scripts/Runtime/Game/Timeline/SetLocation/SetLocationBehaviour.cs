using System;
using UnityEngine;
using UnityEngine.Playables;

namespace _3DGamekitLite.Scripts.Runtime.Game.Timeline.SetLocation
{
    [Serializable]
    public class SetLocationBehaviour : PlayableBehaviour
    {
        public Vector3 position;
        public Vector3 eulerAngles;
    }
}
