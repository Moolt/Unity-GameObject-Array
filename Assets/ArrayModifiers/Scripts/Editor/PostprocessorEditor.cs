using System.Linq;
using UnityEditor;

namespace ArrayModifiers.Scripts.Editor
{
    [CustomEditor(typeof(PostProcessor), true)]
    public class PostprocessorEditor : UnityEditor.Editor
    {
        private PostProcessor _postProcessor;

        private void OnEnable()
        {
            _postProcessor = target as PostProcessor;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();

            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            var arrayModifier = _postProcessor.GetComponents<ArrayModifier>().FirstOrDefault();

            if (arrayModifier == null)
            {
                return;
            }

            arrayModifier.Execute();
        }
    }
}