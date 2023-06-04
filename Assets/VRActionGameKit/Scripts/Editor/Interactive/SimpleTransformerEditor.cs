using _3DGamekitLite.Scripts.Runtime.Interactive.Actions;
using UnityEditor;

namespace _3DGamekitLite.Scripts.Editor.Interactive
{
    [CustomEditor(typeof(SimpleTransformer), true)]
    public class SimpleTransformerEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (cc.changed)
                {
                    var pt = target as SimpleTransformer;
                    pt.PerformTransform(pt.previewPosition);
                }
            }
        }

    }

}
