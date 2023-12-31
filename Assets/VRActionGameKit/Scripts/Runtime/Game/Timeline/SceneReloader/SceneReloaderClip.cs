using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _3DGamekitLite.Scripts.Runtime.Game.Timeline.SceneReloader
{
    [Serializable]
    public class SceneReloaderClip : PlayableAsset, ITimelineClipAsset
    {
        public SceneReloaderBehaviour template = new SceneReloaderBehaviour ();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<SceneReloaderBehaviour>.Create (graph, template);
        }
    }
}
