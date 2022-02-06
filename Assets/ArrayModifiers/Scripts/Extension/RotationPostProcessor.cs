using UnityEngine;

namespace ArrayModifiers.Scripts.Extension
{
    public class RotationPostProcessor : PostProcessor
    {
        [SerializeField] private Vector3 offset;

        private Vector3 _currentOffset;

        public override void BeforeExecute()
        {
            base.BeforeExecute();

            _currentOffset = Vector3.zero;
        }

        public override void Execute(InstanceInfo info)
        {
            _currentOffset += offset;
            info.Instance.rotation = Quaternion.Euler(_currentOffset);
        }
    }
}