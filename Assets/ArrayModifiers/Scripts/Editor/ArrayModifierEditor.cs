using UnityEditor;
using UnityEngine;

namespace ArrayModifiers.Scripts.Editor
{
    [CustomEditor(typeof(ArrayModifier), true)]
    public class ArrayModifierEditor : UnityEditor.Editor
    {
        private ArrayModifier _arrayModifier;
        private bool _colliderMissing;
        private bool _bakeMeshFoldoutState;

        private void OnEnable()
        {
            _arrayModifier = target as ArrayModifier;
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        public override void OnInspectorGUI()
        {
            if (_colliderMissing && _arrayModifier.IsFirstInstance())
            {
                EditorGUILayout.HelpBox(
                    "Collider missing on Prefab. Please add a collider. If you don't want collision on the prefab, you can enable the trigger option on the collider.",
                    MessageType.Warning);
                EditorGUILayout.Space();
            }

            var originalChanged = DrawOriginalProperty();
            var amountChanged = DrawAmountProperty();

            EditorGUILayout.Space();

            var bakeMeshesChanged = DrawBakeMeshesProperty();
            var otherHaveChanged = DrawOtherProperties();
            var hasChanges = originalChanged || amountChanged || otherHaveChanged || bakeMeshesChanged;

            EditorGUILayout.Space();
            DrawApplyButton();

            if (!hasChanges)
            {
                return;
            }

            SynchronizeGlobalSettings();
            _colliderMissing = IsColliderMissing();
            _arrayModifier.Execute();
        }

        private void SynchronizeGlobalSettings()
        {
            if (!_arrayModifier.IsFirstInstance())
            {
                return;
            }

            var all = _arrayModifier.AllInstances();

            foreach (var instance in all)
            {
                if (instance == _arrayModifier)
                {
                    continue;
                }

                instance.Original = _arrayModifier.Original;
                instance.BakeMeshes = _arrayModifier.BakeMeshes;
                instance.AddCollider = _arrayModifier.AddCollider;
                instance.StaticFlags = _arrayModifier.StaticFlags;
                instance.GenerateLightmapUVs = _arrayModifier.GenerateLightmapUVs;
            }
        }

        private bool DrawAmountProperty()
        {
            EditorGUI.BeginChangeCheck();
            var serAmount = serializedObject.FindProperty("amount");
            EditorGUILayout.PropertyField(serAmount);

            return EditorGUI.EndChangeCheck();
        }

        private bool DrawOriginalProperty()
        {
            if (!_arrayModifier.IsFirstInstance())
            {
                return false;
            }

            EditorGUI.BeginChangeCheck();

            var serOriginal = serializedObject.FindProperty("original");
            EditorGUILayout.PropertyField(serOriginal);

            if (!EditorGUI.EndChangeCheck())
            {
                return false;
            }

            _arrayModifier.Clear();
            return true;
        }
    
        private bool DrawBakeMeshesProperty()
        {
            if (!_arrayModifier.IsFirstInstance())
            {
                return false;
            }

            _bakeMeshFoldoutState = EditorGUILayout.Foldout(_bakeMeshFoldoutState, "Baking");

            if (!_bakeMeshFoldoutState)
            {
                return false;
            }

            EditorGUI.BeginChangeCheck();

            var serBakeMeshes = serializedObject.FindProperty("bakeMeshes");
            EditorGUILayout.PropertyField(serBakeMeshes);

            if (serBakeMeshes.boolValue)
            {
                var serStaticFlags = serializedObject.FindProperty("staticFlags");
                EditorGUILayout.PropertyField(serStaticFlags);

                var serLightmapUVs = serializedObject.FindProperty("generateLightmapUVs");
                EditorGUILayout.PropertyField(serLightmapUVs, new GUIContent("Generate Lightmap UVs"));

                var serAddCollider = serializedObject.FindProperty("addCollider");
                EditorGUILayout.PropertyField(serAddCollider);
            }

            if (!EditorGUI.EndChangeCheck())
            {
                return false;
            }

            _arrayModifier.Clear();
            return true;
        }

        private bool DrawOtherProperties()
        {
            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(target, nameof(target));
            DrawPropertiesExcluding(
                serializedObject,
                "m_Script",
                "original",
                "bakeMeshes",
                "amount",
                "staticFlags",
                "generateLightmapUVs",
                "addCollider");
            serializedObject.ApplyModifiedProperties();

            return EditorGUI.EndChangeCheck();
        }

        private void UndoRedoPerformed()
        {
            if (_arrayModifier == null)
            {
                return;
            }

            _arrayModifier.Execute();
        }

        private void DrawApplyButton()
        {
            if (_arrayModifier.IsFirstInstance() && GUILayout.Button("Apply"))
            {
                _arrayModifier.Apply(Undo.DestroyObjectImmediate);
            }
        }

        private bool IsColliderMissing()
        {
            var original = _arrayModifier.Original;

            if (original == null)
            {
                return false;
            }

            var collider = original.GetComponentInChildren<Collider>();
            var collider2D = original.GetComponentInChildren<Collider2D>();

            return collider == null && collider2D == null;
        }
    }
}