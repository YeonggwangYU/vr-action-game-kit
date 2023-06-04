using System;
using _3DGamekitLite.Scripts.Runtime.Game.Player;
using UnityEngine.Playables;

namespace _3DGamekitLite.Scripts.Runtime.Game.Timeline.CutsceneScriptControl
{
    [Serializable]
    public class CutsceneScriptControlBehaviour : PlayableBehaviour
    {
        public bool playerInputEnabled;
        public bool useRootMotion;
        public PlayerInput playerInput;

        public override void OnGraphStart (Playable playable)
        {
        
        }
    }
}
