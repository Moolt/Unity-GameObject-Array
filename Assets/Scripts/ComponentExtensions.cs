using System.Linq;
using UnityEngine;

public static class ComponentExtensions
{
    public static bool TryGetPrecedingInstanceOf<T>(this T component, out T neighboringInstance) where T : Component
    {
        neighboringInstance = component.TryGetNeighboringInstanceOf(-1);
        return neighboringInstance != null;
    }

    public static bool TryGetSubsequentInstanceOf<T>(this T component, out T neighboringInstance) where T : Component
    {
        neighboringInstance = component.TryGetNeighboringInstanceOf(1);
        return neighboringInstance != null;
    }

    public static T FirstInstanceOf<T>(this T component) where T : Component
    {
        var components = component
            .GetComponents<T>()
            .Where(c => c != null)
            .ToList();
        return !components.Any() ? null : components[0];
    }

    private static T TryGetNeighboringInstanceOf<T>(this T component, int offset) where T : Component
    {
        var components = component.GetComponents<T>().ToList();
        var selfIndex = components.IndexOf(component);
        var index = Mathf.Clamp(selfIndex + offset, 0, components.Count - 1);

        if (components.Count == 1 || index == selfIndex)
        {
            return null;
        }

        return components[selfIndex + offset];
    }
}