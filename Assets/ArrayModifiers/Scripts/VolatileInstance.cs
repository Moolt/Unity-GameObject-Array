using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArrayModifiers.Scripts
{
    public class VolatileInstance : IDisposable
    {
        public VolatileInstance(Transform prefab)
        {
            Value = Object.Instantiate(prefab);
        }

        public Transform Value { get; }

        public void Dispose()
        {
            if (Value == null)
            {
                return;
            }

            Object.DestroyImmediate(Value.gameObject);
        }
    }
}