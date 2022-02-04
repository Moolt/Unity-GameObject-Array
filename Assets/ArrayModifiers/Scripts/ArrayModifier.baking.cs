using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArrayModifiers.Scripts
{
    public partial class ArrayModifier
    {
        [SerializeField] private bool bakeMeshes;

        public bool BakeMeshes
        {
            get => bakeMeshes;
            set => SetValue(ref bakeMeshes, value);
        }

#if UNITY_EDITOR
        [SerializeField] private StaticEditorFlags staticFlags;

        public StaticEditorFlags StaticFlags
        {
            get => staticFlags;
            set => SetValue(ref staticFlags, value, (o, n) => o == n);
        }

        [SerializeField] private bool generateLightmapUVs;

        public bool GenerateLightmapUVs
        {
            get => generateLightmapUVs;
            set => SetValue(ref generateLightmapUVs, value);
        }
#endif

        [SerializeField] private bool addCollider;

        public bool AddCollider
        {
            get => addCollider;
            set => SetValue(ref addCollider, value);
        }

        private void HandleMeshBaking()
        {
            if (!bakeMeshes)
            {
                return;
            }

            var bakedMeshes = MeshBakery.Bake(transform).ToList();

#if UNITY_EDITOR
            bakedMeshes.ForEach(m => GameObjectUtility.SetStaticEditorFlags(m.gameObject, StaticFlags));

            if (GenerateLightmapUVs)
            {
                bakedMeshes.ForEach(m =>
                {
                    var mesh = m.gameObject.GetComponent<MeshFilter>().sharedMesh;
                    Unwrapping.GenerateSecondaryUVSet(mesh);
                });
            }
#endif

            if (AddCollider)
            {
                bakedMeshes.ForEach(m => m.gameObject.AddComponent<BoxCollider>());
            }

            bakedMeshes.SetParent(null);
            Clear();
            bakedMeshes.SetParent(transform);
        }
    }
}