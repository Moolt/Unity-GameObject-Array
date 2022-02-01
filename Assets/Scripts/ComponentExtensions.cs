using System;
using System.Linq;
using UnityEngine;

public static class ComponentExtensions
{
    public static bool TryGetPrecedingInstanceOf<T>(
        this T component,
        out T neighboringInstance,
        Predicate<T> predicate = null) where T : Behaviour
    {
        neighboringInstance = component.TryGetNeighboringInstanceOf(-1, predicate);
        return neighboringInstance != null;
    }

    public static bool TryGetSubsequentInstanceOf<T>(
        this T component,
        out T neighboringInstance,
        Predicate<T> predicate = null) where T : Behaviour
    {
        neighboringInstance = component.TryGetNeighboringInstanceOf(1, predicate);
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

    public static bool IsFirstInstanceOf<T>(this T component) where T : Component
    {
        return component.FirstInstanceOf() == component;
    }

    private static T TryGetNeighboringInstanceOf<T>(
        this T component,
        int offset,
        Predicate<T> predicate = null)
        where T : Behaviour
    {
        var components = component
            .GetComponents<T>()
            .Where(c => predicate?.Invoke(c) ?? true)
            .ToList();
        var selfIndex = components.IndexOf(component);
        var index = Mathf.Clamp(selfIndex + offset, 0, components.Count - 1);

        if (components.Count == 1 || index == selfIndex)
        {
            return null;
        }

        return components.ElementAtOrDefault(selfIndex + offset);
    }
}