using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Core
{
    public class FPSTarget : MonoBehaviour
    {
        public int targetFPS = 60;

        // Use this for initialization
        void OnEnable()
        {
            SetTargetFPS(targetFPS);
        }

        public void SetTargetFPS(int fps)
        {
            Application.targetFrameRate = fps;
        }
    } 
}
