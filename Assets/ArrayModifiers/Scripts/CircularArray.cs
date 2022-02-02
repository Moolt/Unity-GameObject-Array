using UnityEngine;

public class CircularArray : ArrayModifier
{
    [SerializeField] private float radius;
    [SerializeField] private float offset;

    protected override Vector3 RelativePositionFor(int index, Bounds bounds)
    {
        var alpha = ((Mathf.Deg2Rad * 360) / Amount) * index;

        return PolarToCartesian(alpha + offset);
    }

    private Vector3 PolarToCartesian(float alpha)
    {
        var x = radius * Mathf.Cos(alpha);
        var z = radius * Mathf.Sin(alpha);

        return new Vector3(x, 0f, z);
    }
}