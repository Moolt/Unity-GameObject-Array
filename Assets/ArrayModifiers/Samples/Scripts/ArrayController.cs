using ArrayModifiers.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class ArrayController : MonoBehaviour
{
    [FormerlySerializedAs("arrayModifier")] [SerializeField] private LinearArray linearArray;

    private void Update()
    {
        var sine = Mathf.Sin(Time.realtimeSinceStartup * 2f);
        linearArray.RelativeOffset = Vector3.up * (sine + 2.1f);
    }
}