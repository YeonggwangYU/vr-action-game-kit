using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Audio
{
    public class AudioPlayerOnEnable : MonoBehaviour
    {
        public RandomAudioPlayer player;
        public bool stopOnDisable = false;

        void OnEnable()
        {
            player.PlayRandomClip();
        }

        private void OnDisable()
        {
            if (stopOnDisable)
                player.audioSource.Stop();
        }
    } 
}
