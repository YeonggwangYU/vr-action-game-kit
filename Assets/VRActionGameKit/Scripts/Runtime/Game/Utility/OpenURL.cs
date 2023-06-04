using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Utility
{
    public class OpenURL : MonoBehaviour
    {
        public string websiteAddress;

        public void OpenURLOnClick()
        {
            Application.OpenURL(websiteAddress);
        }
    } 
}