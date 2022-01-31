using UnityEngine;

public class ArrayController : MonoBehaviour
{
    [SerializeField] private ArrayModifier arrayModifier;

    private void Update()
    {
        var sine = Mathf.Sin(Time.realtimeSinceStartup * 2f);
        arrayModifier.RelativeOffset = Vector3.up * (sine + 2.1f);
    }
}