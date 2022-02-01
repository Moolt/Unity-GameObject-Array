using UnityEngine;

public class LinearArray : ArrayModifier
{
    [Space] [Header("Relative Offset")] [SerializeField]
    private bool useRelativeOffset = true;

    [SerializeField] private Vector3 relativeOffset = Vector3.right;

    [Space] [Header("Constant Offset")] [SerializeField]
    private bool useConstantOffset;

    [SerializeField] private Vector3 constantOffset = Vector3.zero;

    protected override Vector3 RelativePositionFor(int index, Bounds bounds)
    {
        var size = bounds.size;
        var relative = useRelativeOffset ? Vector3.Scale(size, relativeOffset) : Vector3.zero;
        var constant = useConstantOffset ? constantOffset : Vector3.zero;
        var offset = (relative + constant) * index;

        return offset;
    }

    public bool UseRelativeOffset
    {
        get => useRelativeOffset;
        set => SetValue(ref useRelativeOffset, value);
    }

    public Vector3 RelativeOffset
    {
        get => relativeOffset;
        set => SetValue(ref relativeOffset, value);
    }

    public bool UseConstantOffset
    {
        get => useConstantOffset;
        set => SetValue(ref useConstantOffset, value);
    }

    public Vector3 ConstantOffset
    {
        get => constantOffset;
        set => SetValue(ref constantOffset, value);
    }
}