using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArrayModifier))]
public class ArrayModifierEditor : Editor
{
    private ArrayModifier _arrayModifier;
    private bool _colliderMissing;
    private Transform _transform;

    private void OnDestroy()
    {
        var arrayModifiers = _transform.GetComponents<ArrayModifier>();

        if (arrayModifiers.Length == 0)
        {
            _transform.ClearChildren();
            return;
        }

        var first = _transform.GetComponents<ArrayModifier>().FirstOrDefault();

        if (first == null)
        {
            return;
        }

        first.Apply();
    }

    private void OnEnable()
    {
        _arrayModifier = target as ArrayModifier;

        if (_arrayModifier != null)
        {
            _transform = _arrayModifier.transform;
        }
    }

    public override void OnInspectorGUI()
    {
        if (_colliderMissing && _arrayModifier.IsFirstInstanceOf())
        {
            EditorGUILayout.HelpBox(
                "Collider missing on Prefab. Please add a collider. If you don't want collision on the prefab, you can enable the trigger option on the collider.",
                MessageType.Warning);
            EditorGUILayout.Space();
        }

        var originalChanged = DrawOriginalProperty();
        var otherHaveChanged = DrawOtherProperties();
        var hasChanges = originalChanged || otherHaveChanged;

        if (!hasChanges)
        {
            return;
        }

        _colliderMissing = IsColliderMissing();
        _arrayModifier.Apply();
    }

    private bool DrawOriginalProperty()
    {
        if (!_arrayModifier.IsFirstInstanceOf())
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

        DrawPropertiesExcluding(serializedObject, "m_Script", "original");
        serializedObject.ApplyModifiedProperties();

        return EditorGUI.EndChangeCheck();
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