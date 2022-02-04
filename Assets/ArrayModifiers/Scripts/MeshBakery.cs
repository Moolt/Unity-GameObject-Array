using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArrayModifiers.Scripts
{
    public static class MeshBakery
    {
        public static IEnumerable<Transform> Bake(Transform parent)
        {
            var meshFilters = parent.GetComponentsInChildren<MeshFilter>();
            var mapping = new Dictionary<Material, IList<CombineInstance>>();

            foreach (var meshFilter in meshFilters)
            {
                var material = meshFilter.GetComponent<MeshRenderer>().sharedMaterial;

                if (!mapping.ContainsKey(material))
                {
                    mapping[material] = new List<CombineInstance>();
                }

                var combine = new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                };

                mapping[material].Add(combine);
            }

            var index = 0;
            foreach (var pair in mapping)
            {
                var obj = new GameObject($"submesh_{index}");
                var meshFilter = obj.AddComponent<MeshFilter>();
                var meshRenderer = obj.AddComponent<MeshRenderer>();
                var mesh = new Mesh();

                meshFilter.sharedMesh = mesh;
                meshFilter.sharedMesh.CombineMeshes(pair.Value.ToArray(), true, true, false);
                meshRenderer.sharedMaterial = pair.Key;

                index++;

                yield return obj.transform;
            }
        }
    }
}