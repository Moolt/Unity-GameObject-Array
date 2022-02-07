using System;
using System.Collections.Generic;
using System.Linq;
using ArrayModifiers.Scripts.Extension;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArrayModifiers.Scripts
{
    [ExecuteInEditMode]
    public abstract partial class ArrayModifier : MonoBehaviour
    {
        [SerializeField] private Transform original;
        [SerializeField] [Min(1)] private int amount = 2;
        [SerializeField] [HideInInspector] private bool initialized;

        private bool _isCurrentlyApplying;
        private bool _executionLocked;

        private ObjectPool _objectPool;

        private ObjectPool ObjectPool => _objectPool ??= new ObjectPool(transform, () => Original);

        private void OnEnable()
        {
            _executionLocked = false;
            _isCurrentlyApplying = false;

            if (IsFirstInstance() && !bakeMeshes)
            {
                Execute();
            }
        }

        private void OnDisable()
        {
            _executionLocked = true;
        }

        private void OnDestroy()
        {
            if (_isCurrentlyApplying)
            {
                return;
            }

            _executionLocked = true;

            var arrayModifiers = GetComponents<ArrayModifier>();

            if (arrayModifiers.Length == 1)
            {
                Clear();
                return;
            }

            var first = FirstInstance();

            if (first == null)
            {
                return;
            }

            first.Execute();
        }

        public void Execute()
        {
            if (TryGetNextInstance(out var subsequent))
            {
                subsequent.Execute();
                return;
            }

            if (!TryGetPositions(out var positions))
            {
                return;
            }

            if (BakeMeshes)
            {
                Clear();
            }

            ObjectPool.Size = positions.Count;

            ApplyPositions(positions);
            HandlePostprocessing();
            HandleMeshBaking();
        }

        public void Clear() => ObjectPool.Clear();

        // ReSharper disable once UnusedMember.Global
        public void Apply() => Apply(Destroy);

        public void Apply(Action<Object> destroy)
        {
            this.DestroyComponents<PostProcessor>(destroy);
            this.DestroyComponents<ArrayModifier>(destroy, m => m._isCurrentlyApplying = true);
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

        private void ApplyPositions(IList<Vector3> positions)
        {
            if (positions.Count != transform.childCount)
            {
                return;
            }

            var i = 0;
            foreach (Transform child in transform)
            {
                var childTransform = child.transform;
                child.rotation = Quaternion.identity;
                child.localScale = Vector3.one;
                childTransform.localPosition = positions[i];
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

            if (TryGetPreviousInstance(out var previousArray))
            {
                basePositions.AddRange(previousArray.Positions);
            }

            if (!basePositions.Any())
            {
                basePositions.Add(Vector3.zero);
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
            var colliderComponent = instance.GetComponentInChildren<T>();

            if (colliderComponent == null)
            {
                bounds = new Bounds();
                return false;
            }

            bounds = getter(colliderComponent);
            return true;
        }

        private void HandlePostprocessing()
        {
            var postprocessors = GetComponents<PostProcessor>();

            foreach (var postprocessor in postprocessors)
            {
                postprocessor.BeforeExecute();
                var root = transform;
                var childCount = root.childCount;

                for (var i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var info = new InstanceInfo
                    {
                        Instance = child,
                        Root = root,
                        TotalCount = childCount,
                        Index = i
                    };
                    postprocessor.Execute(info);
                }

                postprocessor.AfterExecute();
            }
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
            get => original;
            set => SetValue(ref original, value, (o, n) => o == n);
        }

        public int Amount
        {
            get => amount;
            set => SetValue(ref amount, value);
        }
    }
}