using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public abstract class ArrayModifier : MonoBehaviour
{
    [SerializeField] private Transform original;
    [SerializeField] [Min(1)] private int amount = 2;

    private bool _isCurrentlyApplying;
    private bool _isCurrentlyBeingDestroyed;

    private void OnEnable()
    {
        _isCurrentlyBeingDestroyed = false;

        if (transform.IsFirstInstanceOf())
        {
            Execute();
        }
    }

    private void OnDestroy()
    {
        if (_isCurrentlyApplying)
        {
            return;
        }

        _isCurrentlyBeingDestroyed = true;

        var arrayModifiers = GetComponents<ArrayModifier>();

        if (arrayModifiers.Length == 1)
        {
            transform.ClearChildren();
            return;
        }

        var first = transform
            .GetComponents<ArrayModifier>()
            .FirstOrDefault(c => c != this);

        if (first == null)
        {
            return;
        }

        first.Execute();
    }

    public void Execute()
    {
        if (this.TryGetSubsequentInstanceOf(out var subsequent))
        {
            subsequent.Execute();
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

    // ReSharper disable once UnusedMember.Global
    public void Apply() => Apply(Destroy);

    public void Apply(Action<Object> destroy)
    {
        var modifiers = GetComponents<ArrayModifier>();

        foreach (var modifier in modifiers)
        {
            modifier._isCurrentlyApplying = true;
            destroy(modifier);
        }
    }

    protected abstract Vector3 RelativePositionFor(int index, Bounds bounds);

    // ReSharper disable once MemberCanBePrivate.Global
    protected void SetValue<T>(ref T oldValue, T newValue, Func<T, T, bool> predicate)
    {
        if (predicate(oldValue, newValue))
        {
            return;
        }

        oldValue = newValue;
        Execute();
    }

    protected void SetValue<T>(ref T oldValue, T newValue) where T : IEquatable<T>
    {
        SetValue(ref oldValue, newValue, (o, n) => o.Equals(n));
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

    private bool TryGetPositions(out IList<Vector3> positions)
    {
        positions = new List<Vector3>();
        var generatedPositions = new List<Vector3>();
        var basePositions = new List<Vector3>();

        if (!TryGetBounds(out var bounds))
        {
            return false;
        }

        if (this.TryGetPrecedingInstanceOf(out var previousArray, modifier => !modifier._isCurrentlyBeingDestroyed))
        {
            basePositions.AddRange(previousArray.Positions);
        }

        if (!basePositions.Any())
        {
            basePositions.Add(transform.position);
        }

        foreach (var position in basePositions)
        {
            for (var i = 0; i < Math.Max(Amount, 0); i++)
            {
                var center = AbsolutePositionFor(i, bounds, position);
                generatedPositions.Add(center);
            }
        }

        positions = generatedPositions.Distinct().ToList();
        return true;
    }

    private Vector3 AbsolutePositionFor(int index, Bounds bounds, Vector3 pivot)
    {
        return pivot + RelativePositionFor(index, bounds);
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

    private bool TryGetBounds<T>(Component instance, Func<T, Bounds> getter, out Bounds bounds) where T : Component
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

    private IEnumerable<Vector3> Positions
    {
        get
        {
            TryGetPositions(out var positions);
            return positions;
        }
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
}