using ArrayModifiers.Scripts;
using UnityEngine;

namespace Samples.Scripts
{
    public class ColorPostProcessor : PostProcessor
    {
        [SerializeField] private Color color;

        public override void Execute(Transform instance)
        {
            var material = instance.GetComponent<MeshRenderer>().sharedMaterial;
            material.color = color;
        }
    }
}
