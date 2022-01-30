using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArrayModifier))]
public class ArrayModifierEditor : Editor
{
    private ArrayModifier _arrayModifier;
    private bool _colliderMissing;

    private void OnEnable()
    {
        _arrayModifier = target as ArrayModifier;
    }

    public override void OnInspectorGUI()
    {
        if (_colliderMissing)
        {
            EditorGUILayout.HelpBox(
                "Collider missing on Prefab. Please add a collider. If you don't want collision on the prefab, you can enable the trigger option on the collider.",
                MessageType.Warning);
            EditorGUILayout.Space();
        }

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (!EditorGUI.EndChangeCheck())
        {
            return;
        }

        _arrayModifier.Apply();
        _colliderMissing = IsColliderMissing();
    }

    private bool IsColliderMissing()
    {
        var original = _arrayModifier.Original;

        if (original == null)
        {
            return false;
        }

        var collider = original.GetComponent<Collider>();
        return collider == null;
    }
}