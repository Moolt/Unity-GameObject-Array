using UnityEngine;

public static class TransformExtensions
{
    public static void ClearChildren(this Transform transform)
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            Object.DestroyImmediate(child.gameObject);
        }
    }
}