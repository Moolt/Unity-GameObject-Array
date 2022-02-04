using System.Collections.Generic;
using UnityEngine;

namespace ArrayModifiers.Scripts
{
    public static class IEnumerable_Transform_Extensions
    {
        public static void SetParent(this IEnumerable<Transform> transforms, Transform parent)
        {
            foreach (var transform in transforms)
            {
                transform.SetParent(parent);
            }
        }
    }
}