using UnityEngine;

namespace ArrayModifiers.Scripts.Extension
{
    public struct InstanceInfo
    {
        public Transform Instance { get; set; }
        public Transform Root { get; set; }
        public int TotalCount { get; set; }
        public int Index { get; set; }

        public void Deconstruct(out Transform instance, out Transform root, out int totalCount, out int index)
        {
            instance = Instance;
            root = Root;
            totalCount = TotalCount;
            index = Index;
        }
    }
}