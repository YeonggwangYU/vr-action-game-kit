namespace _3DGamekitLite.Scripts.Editor.Game.Core
{
    public abstract class SubEditor<T>
    {
        public abstract void OnInspectorGUI(T instance);

        public void Init(UnityEditor.Editor editor)
        {
            this.editor = editor;
        }

        public void Update()
        {
            if (defer != null) defer();
            defer = null;
        }

        protected void Defer(System.Action fn)
        {
            defer += fn;
        }

        protected void Repaint()
        {
            editor.Repaint();
        }

        UnityEditor.Editor editor;
        System.Action defer;
    } 
}
