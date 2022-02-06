using UnityEngine;

namespace ArrayModifiers.Scripts.Extension
{
    public class LookToCenterPostProcessor : PostProcessor
    {
        [SerializeField] private Vector3 position;

        public override void Execute(InstanceInfo info)
        {
            var (root, instance, _, _) = info;
            var parentPosition = root.position;
            position = new Vector3(0f, instance.position.y, 0f);

            instance.LookAt(position);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(position, Vector3.one);
        }
    }
}