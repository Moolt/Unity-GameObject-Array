using ArrayModifiers.Scripts;
using UnityEngine;

namespace Samples.Scripts
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

        public override void Execute(Transform instance)
        {
            _currentOffset += offset;
            instance.rotation = Quaternion.Euler(_currentOffset);
        }
    }
}