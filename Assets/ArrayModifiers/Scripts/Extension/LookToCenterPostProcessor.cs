using UnityEngine;

namespace ArrayModifiers.Scripts.Extension
{
    public class LookToCenterPostProcessor : PostProcessor
    {
        public override void Execute(InstanceInfo info)
        {
            var (instance, root, _, _) = info;
            var parentPosition = root.position;
            var position = new Vector3(parentPosition.x, instance.position.y, parentPosition.z);

            instance.LookAt(position);
        }
    }
}