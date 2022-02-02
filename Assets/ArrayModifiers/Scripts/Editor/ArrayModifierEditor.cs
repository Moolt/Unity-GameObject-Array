using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArrayModifier), true)]
public class ArrayModifierEditor : Editor
{
    private ArrayModifier _arrayModifier;
    private bool _colliderMissing;

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
        var otherHaveChanged = DrawOtherProperties();
        var hasChanges = originalChanged || otherHaveChanged;

        EditorGUILayout.Space();
        DrawApplyButton();

        if (!hasChanges)
        {
            return;
        }

        _colliderMissing = IsColliderMissing();
        _arrayModifier.Execute();
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

    private bool DrawOtherProperties()
    {
        EditorGUI.BeginChangeCheck();

        Undo.RecordObject(target, nameof(target));
        DrawPropertiesExcluding(serializedObject, "m_Script", "original");
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

        var collider = original.GetComponent<Collider>();
        var collider2D = original.GetComponent<Collider2D>();

        return collider == null && collider2D == null;
    }
}