using _3DGamekitLite.Scripts.Runtime.Game.Timeline.SceneReloader;
using UnityEditor;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Editor.Game.Timeline.SceneReloader
{
    [CustomPropertyDrawer(typeof(SceneReloaderBehaviour))]
    public class SceneReloaderDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            int fieldCount = 0;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
        
        }
    }
}
