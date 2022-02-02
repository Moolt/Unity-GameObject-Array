using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ArrayModifierExtensions
{
    public static IList<ArrayModifier> AllInstances(this ArrayModifier component)
    {
        return component
            .GetComponents<ArrayModifier>()
            .Where(a => !a.IsBeingDestroyed)
            .ToList();
    }

    public static bool TryGetPreviousInstance(this ArrayModifier component, out ArrayModifier neighboringInstance)
    {
        neighboringInstance = component.TryGetNeighboringInstance(-1);
        return neighboringInstance != null;
    }

    public static bool TryGetNextInstance(this ArrayModifier component, out ArrayModifier neighboringInstance)
    {
        neighboringInstance = component.TryGetNeighboringInstance(1);
        return neighboringInstance != null;
    }

    public static ArrayModifier FirstInstance(this ArrayModifier component)
    {
        var components = component.AllInstances();
        return !components.Any() ? null : components[0];
    }

    public static bool IsFirstInstance(this ArrayModifier component)
    {
        return component.FirstInstance() == component;
    }

    private static ArrayModifier TryGetNeighboringInstance(this ArrayModifier component, int offset)
    {
        var components = component.AllInstances();
        var selfIndex = components.IndexOf(component);
        var index = Mathf.Clamp(selfIndex + offset, 0, components.Count - 1);

        if (components.Count == 1 || index == selfIndex)
        {
            return null;
        }

        return components[selfIndex + offset];
    }
}