using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ArrayModifier : MonoBehaviour
{
    [SerializeField] private Transform original;
    [SerializeField] [Min(1)] private int amount = 2;

    [Space] [Header("Relative Offset")] [SerializeField]
    private bool useRelativeOffset = true;

    [SerializeField] private Vector3 relativeOffset = Vector3.right;

    [Space] [Header("Constant Offset")] [SerializeField]
    private bool useConstantOffset;

    [SerializeField] private Vector3 constantOffset = Vector3.zero;

    public void Apply()
    {
        if (this.TryGetSubsequentInstanceOf(out var subsequent))
        {
            subsequent.Apply();
            return;
        }

        if (!TryGetPositions(out var positions))
        {
            return;
        }

        ResizeObjectPool(positions.Count);
        ApplyPositions(positions);
    }

    public void Clear() => transform.ClearChildren();

    private void ApplyPositions(IList<Vector3> positions)
    {
        if (positions.Count != transform.childCount)
        {
            return;
        }

        var i = 0;
        foreach (Transform child in transform)
        {
            child.transform.position = positions[i];
            i++;
        }
    }

    private void ResizeObjectPool(int size)
    {
        var childCount = transform.childCount;
        var difference = childCount - size;

        if (difference == 0)
        {
            return;
        }

        for (var i = 0; i < Mathf.Abs(difference); i++)
        {
            if (difference > 0)
            {
                RemoveInstance();
            }
            else
            {
                AddInstance();
            }
        }
    }

    private IEnumerable<Vector3> GetPositions()
    {
        TryGetPositions(out var positions);
        return positions;
    }

    private bool TryGetPositions(out IList<Vector3> positions)
    {
        positions = new List<Vector3>();
        var generatedPositions = new List<Vector3>();
        var basePositions = new List<Vector3>();

        if (!TryGetBounds(out var bounds))
        {
            return false;
        }

        if (this.TryGetPrecedingInstanceOf(out var previousArray))
        {
            basePositions.AddRange(previousArray.GetPositions());
        }

        if (!basePositions.Any())
        {
            basePositions.Add(transform.position);
        }

        foreach (var position in basePositions)
        {
            for (var i = 0; i < Math.Max(amount, 0); i++)
            {
                var center = AbsolutePositionFor(i, bounds, position);
                generatedPositions.Add(center);
            }
        }

        positions = generatedPositions.Distinct().ToList();
        return true;
    }

    private bool TryGetBounds(out Bounds bounds)
    {
        if (Original == null)
        {
            bounds = new Bounds();
            return false;
        }

        using var instance = new VolatileInstance(Original);

        if (TryGetBounds<Collider>(instance.Value, c => c.bounds, out bounds))
        {
            return true;
        }
        
        if (TryGetBounds<Collider2D>(instance.Value, c => c.bounds, out bounds))
        {
            return true;
        }

        return false;
    }

    private bool TryGetBounds<T>(Transform instance, Func<T, Bounds> getter, out Bounds bounds) where T: Component
    {
        var colliderComponent = instance.GetComponent<T>();

        if (colliderComponent == null)
        {
            bounds = new Bounds();
            return false;
        }

        bounds = getter(colliderComponent);
        return true;
    }

    private Vector3 AbsolutePositionFor(int index, Bounds bounds, Vector3 pivot)
    {
        return pivot + RelativePositionFor(index, bounds);
    }

    private Vector3 RelativePositionFor(int index, Bounds bounds)
    {
        var size = bounds.size;
        var relative = useRelativeOffset ? Vector3.Scale(size, relativeOffset) : Vector3.zero;
        var constant = useConstantOffset ? constantOffset : Vector3.zero;
        var offset = (relative + constant) * index;

        return offset;
    }

    private void AddInstance()
    {
        Transform instance;
#if UNITY_EDITOR
        instance = PrefabUtility.InstantiatePrefab(Original, transform) as Transform;

        if (instance == null)
        {
            return;
        }
#else
        instance = Instantiate(Original);
#endif

        instance.SetParent(transform);
    }

    private void RemoveInstance()
    {
        var index = transform.childCount - 1;
        var child = transform.GetChild(index);
        DestroyImmediate(child.gameObject);
    }

    public Transform Original
    {
        get
        {
            var arrayModifier = this.FirstInstanceOf();

            if (arrayModifier == this || arrayModifier.Original == null)
            {
                return original;
            }

            original = arrayModifier.Original;
            return original;
        }
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