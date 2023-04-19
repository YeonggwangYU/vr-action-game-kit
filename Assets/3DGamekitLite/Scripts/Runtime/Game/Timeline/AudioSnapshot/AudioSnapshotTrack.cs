using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _3DGamekitLite.Scripts.Runtime.Game.Timeline.AudioSnapshot
{
    [TrackColor(0.8970588f, 0.6037406f, 0.07915227f)]
    [TrackClipType(typeof(AudioSnapshotClip))]
    public class AudioSnapshotTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<AudioSnapshotMixerBehaviour>.Create (graph, inputCount);
        }
    }
}
