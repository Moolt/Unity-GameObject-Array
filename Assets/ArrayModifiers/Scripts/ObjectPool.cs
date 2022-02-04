using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArrayModifiers.Scripts
{
    public class ObjectPool
    {
        private readonly Func<Transform> _prefabGetter;
        private readonly IList<Transform> _instances = new List<Transform>();

        private Transform Prefab => _prefabGetter();
        private Transform Parent { get; }

        public ObjectPool(Transform parent, Func<Transform> prefabGetter)
        {
            Parent = parent;
            _prefabGetter = prefabGetter;
        }

        public ObjectPool(Transform parent, Transform prefab) : this(parent, () => prefab)
        {
        }

        private int _size;

        public int Size
        {
            get => _size;
            set
            {
                if (_size < 0)
                {
                    throw new ArgumentException("Object pool size can not be negative.");
                }

                _size = value;
                ApplySize();
            }
        }

        public void Clear() => Size = 0;

        private void ApplySize()
        {
            var childCount = Parent.childCount;
            var difference = childCount - Size;

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

        private void AddInstance()
        {
            Debug.Log("add instance");
            Transform instance;
#if UNITY_EDITOR
            instance = PrefabUtility.InstantiatePrefab(Prefab) as Transform;

            if (instance == null)
            {
                return;
            }
#else
            instance = Instantiate(Original);
#endif

            instance.SetParent(Parent);
            _instances.Add(instance);
        }

        private void RemoveInstance()
        {
            var index = Parent.childCount - 1;
            var child = Parent.GetChild(index);

            _instances.Remove(child);
            Object.DestroyImmediate(child.gameObject);
        }
    }
}