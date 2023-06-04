using _3DGamekitLite.Scripts.Runtime.Game.Timeline.SetLocation;
using UnityEditor;
using UnityEngine;

namespace _3DGamekitLite.Scripts.Editor.Game.Timeline.SetLocation
{
    [CustomPropertyDrawer(typeof(SetLocationBehaviour))]
    public class SetLocationDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            int fieldCount = 2;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty positionProp = property.FindPropertyRelative("position");
            SerializedProperty eulerAnglesProp = property.FindPropertyRelative("eulerAngles");

            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, positionProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, eulerAnglesProp);
        }
    }
}
