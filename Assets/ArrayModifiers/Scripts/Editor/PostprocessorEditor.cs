using System.Linq;
using ArrayModifiers.Scripts.Extension;
using UnityEditor;
using UnityEngine;
using static ArrayModifiers.Scripts.Extension.PostProcessor;
using static ArrayModifiers.Scripts.Extension.PostProcessor.Fields;

namespace ArrayModifiers.Scripts.Editor
{
    [CustomEditor(typeof(PostProcessor), true)]
    public class PostprocessorEditor : UnityEditor.Editor
    {
        private PostProcessor _postProcessor;
        private GameObject _gameObject;

        private void OnDestroy()
        {
            if (target == null)
            {
                UpdateArrayModifier();
            }
        }

        private void OnEnable()
        {
            _postProcessor = target as PostProcessor;

            if (_postProcessor != null)
            {
                _gameObject = _postProcessor.gameObject;
            }

            if (!Initialized)
            {
                UpdateArrayModifier();
            }

            Initialized = true;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();

            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            UpdateArrayModifier();
        }
        
        private void UpdateArrayModifier()
        {
            if (_gameObject == null)
            {
                return;
            }
            
            var arrayModifier = _gameObject.GetComponents<ArrayModifier>().FirstOrDefault();

            if (arrayModifier != null)
            {
                arrayModifier.Execute();
            }
        }
        
        private bool Initialized
        {
            get
            {
                var serInitialized = serializedObject.FindProperty(Fields.Initialized);
                return serInitialized.boolValue;
            }
            set
            {
                var serInitialized = serializedObject.FindProperty(Fields.Initialized);
                serInitialized.boolValue = value;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}