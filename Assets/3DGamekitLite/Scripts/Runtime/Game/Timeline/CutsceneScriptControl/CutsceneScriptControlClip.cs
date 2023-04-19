using System;
using _3DGamekitLite.Scripts.Runtime.Game.Player;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _3DGamekitLite.Scripts.Runtime.Game.Timeline.CutsceneScriptControl
{
    [Serializable]
    public class CutsceneScriptControlClip : PlayableAsset, ITimelineClipAsset
    {
        public ExposedReference<PlayerInput> playerInput;
        public CutsceneScriptControlBehaviour template = new CutsceneScriptControlBehaviour();
    
        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CutsceneScriptControlBehaviour>.Create (graph, template);
            CutsceneScriptControlBehaviour clone = playable.GetBehaviour ();
            clone.playerInput = playerInput.Resolve (graph.GetResolver ());
            return playable;
        }
    }
}
