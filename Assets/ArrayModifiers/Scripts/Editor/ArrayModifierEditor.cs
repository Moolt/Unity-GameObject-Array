using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArrayModifiers.Scripts.ArrayModifier), true)]
public class ArrayModifierEditor : Editor
{
    private ArrayModifiers.Scripts.ArrayModifier _arrayModifier;
    private bool _colliderMissing;

    private void OnEnable()
    {
        _arrayModifier = target as ArrayModifiers.Scripts.ArrayModifier;
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
        var bakeMeshesChanged = DrawBakeMeshesProperty();
        var otherHaveChanged = DrawOtherProperties();
        var hasChanges = originalChanged || otherHaveChanged || bakeMeshesChanged;

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
        }
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

        EditorGUI.BeginChangeCheck();

        var serOriginal = serializedObject.FindProperty("bakeMeshes");
        EditorGUILayout.PropertyField(serOriginal);

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
        DrawPropertiesExcluding(serializedObject, "m_Script", "original", "bakeMeshes");
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