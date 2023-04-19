using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.UI
{
    public class SetCursorMode : MonoBehaviour
    {
        public bool visible = true;
        public CursorLockMode lockMode = CursorLockMode.None;

        void Start()
        {
            Cursor.visible = visible;
            Cursor.lockState = lockMode;
        }
    }
}