using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArrayModifiers.Scripts
{
    public static class ComponentExtensions
    {
        public static void DestroyComponents<T>(
            this Component self,
            Action<Object> destroy,
            Action<T> prepare = null) where T: Component
        {
            var components = self.GetComponents<T>();

            foreach (var component in components)
            {
                prepare?.Invoke(component);
                destroy(component);
            }
        }
    }
}