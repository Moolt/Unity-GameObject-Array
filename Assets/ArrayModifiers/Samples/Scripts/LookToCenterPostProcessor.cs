using ArrayModifiers.Scripts;
using UnityEngine;

namespace Samples.Scripts
{
    public class LookToCenterPostProcessor : PostProcessor
    {
        public override void Execute(Transform instance)
        {
            var parent = instance.parent.transform;
            var parentPosition = parent.position;
            var position = new Vector3(parentPosition.x, instance.position.y, parentPosition.z);

            instance.LookAt(position);
        }
    }
}