using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArrayModifier))]
public class ArrayModifierEditor : Editor
{
    private ArrayModifier _arrayModifier;
    private bool _colliderMissing;
    private GameObject _gameObject;

    private void OnDisable()
    {
        if (target != null)
        {
            return;
        }

        var arrayModifiers = _gameObject
            .GetComponents<ArrayModifier>()
            .Where(a => a != null)
            .ToList();

        var first = arrayModifiers.FirstOrDefault();

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
            _gameObject = _arrayModifier.gameObject;
        }
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

        _colliderMissing = IsColliderMissing();
        _arrayModifier.Apply();
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