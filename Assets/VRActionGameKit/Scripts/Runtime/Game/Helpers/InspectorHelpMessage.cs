using UnityEditor;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Game.Helpers
{
    /// <summary>
    /// Little helper class that allow to display a message in the inspector for documentation on some gameobject.
    /// </summary>
    public class InspectorHelpMessage : MonoBehaviour
    {
        public string message;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InspectorHelpMessage))]
    public class InspectorHelpMessageEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox((target as InspectorHelpMessage).message, MessageType.Info);
        }
    }
#endif 
}