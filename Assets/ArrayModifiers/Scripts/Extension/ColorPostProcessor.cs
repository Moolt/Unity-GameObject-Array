using UnityEngine;

namespace ArrayModifiers.Scripts.Extension
{
    public class ColorPostProcessor : PostProcessor
    {
        [SerializeField] private Color startColor;
        [SerializeField] private Color endColor;

        public override void Execute(InstanceInfo info)
        {
            var (instance, _, totalCount, index) = info;
            var lerpValue = index / (float) totalCount;
            var color = Color.Lerp(startColor, endColor, lerpValue);
            var instanceRenderer = instance.GetComponent<Renderer>();

            var tempMaterial = new Material(instanceRenderer.sharedMaterial)
            {
                color = color
            };

            instanceRenderer.sharedMaterial = tempMaterial;
        }
    }
}