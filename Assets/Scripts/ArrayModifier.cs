using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ArrayModifier : MonoBehaviour
{
    [SerializeField] private Transform original;
    [SerializeField] [Min(0)] private int amount;

    [Space] [Header("Relative Offset")] [SerializeField]
    private bool useRelativeOffset = true;

    [SerializeField] private Vector3 relativeOffset = Vector3.right;

    [Space] [Header("Constant Offset")] [SerializeField]
    private bool useConstantOffset;

    [SerializeField] private Vector3 constantOffset = Vector3.zero;

    public void Apply()
    {
        ClearChildren();

        if (!TryInstantiateFirstInstance(out var instance))
        {
            return;
        }

        if (!TryGetPositions(instance, out var positions))
        {
            return;
        }

        foreach (var position in positions)
        {
            InstantiateAt(position);
        }
    }

    private bool TryInstantiateFirstInstance(out Transform instance)
    {
        if (original == null)
        {
            instance = null;
            return false;
        }

        instance = InstantiateFirst();
        return true;
    }

    private void ClearChildren()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
    }

    private bool TryGetPositions(Transform instance, out IList<Vector3> positions)
    {
        positions = new List<Vector3>();

        if (!TryGetBounds(instance, out var bounds))
        {
            return false;
        }

        for (var i = 1; i < Math.Max(amount + 1, 0); i++)
        {
            var center = AbsolutePositionFor(i, bounds);
            positions.Add(center);
        }

        return true;
    }

    private bool TryGetBounds(Transform instance, out Bounds bounds)
    {
        if (instance == null)
        {
            bounds = new Bounds();
            return false;
        }

        var objectCollider = instance.GetComponent<Collider>();

        if (objectCollider == null)
        {
            bounds = new Bounds();
            return false;
        }

        bounds = objectCollider.bounds;
        return true;
    }

    private Vector3 AbsolutePositionFor(int index, Bounds bounds)
    {
        return transform.position + RelativePositionFor(index, bounds);
    }

    private Vector3 RelativePositionFor(int index, Bounds bounds)
    {
        var size = bounds.size;
        var relative = useRelativeOffset ? Vector3.Scale(size, relativeOffset) : Vector3.zero;
        var constant = useConstantOffset ? constantOffset : Vector3.zero;
        var offset = (relative + constant) * index;

        return offset;
    }

    private Transform InstantiateFirst()
    {
        return InstantiateAt(transform.position);
    }

    private Transform InstantiateAt(Vector3 position)
    {
        Transform instance;
#if UNITY_EDITOR
        instance = PrefabUtility.InstantiatePrefab(original, transform) as Transform;

        if (instance == null)
        {
            return null;
        }

        instance.transform.position = position;
#else
        instance = Instantiate(original, position, Quaternion.identity);
#endif

        instance.SetParent(transform);
        return instance;
    }
    
    public Transform Original
    {
        get => original;
        set => SetValue(ref original, value, (o, n) => o == n);
    }

    public int Amount
    {
        get => amount;
        set => SetValue(ref amount, value);
    }

    public bool UseRelativeOffset
    {
        get => useRelativeOffset;
        set => SetValue(ref useRelativeOffset, value);
    }

    public Vector3 RelativeOffset
    {
        get => relativeOffset;
        set => SetValue(ref relativeOffset, value);
    }

    public bool UseConstantOffset
    {
        get => useConstantOffset;
        set => SetValue(ref useConstantOffset, value);
    }

    public Vector3 ConstantOffset
    {
        get => constantOffset;
        set => SetValue(ref constantOffset, value);
    }

    private void SetValue<T>(ref T oldValue, T newValue, Func<T, T, bool> predicate)
    {
        if (predicate(oldValue, newValue))
        {
            return;
        }

        oldValue = newValue;
        Apply();
    }

    private void SetValue<T>(ref T oldValue, T newValue) where T : IEquatable<T>
    {
        SetValue(ref oldValue, newValue, (o, n) => o.Equals(n));
    }
}