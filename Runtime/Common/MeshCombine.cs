using UnityEngine;
using UnityEngine.Rendering;

namespace UnityUtils
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshCombine : MonoBehaviour
    {
        private void Start()
        {
            CombineInstance[] combineInstances = new CombineInstance[transform.childCount];

            int index = 0;
            int totalSourceVertices = 0;

            foreach (Transform child in transform)
            {
                MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                Mesh sourceMesh = meshFilter != null ? meshFilter.sharedMesh : null;
                if (sourceMesh != null)
                {
                    totalSourceVertices += sourceMesh.vertexCount;
                }

                combineInstances[index].mesh = sourceMesh;
                combineInstances[index].transform = child.localToWorldMatrix;
                child.gameObject.SetActive(false);
                index++;
            }

            Mesh combinedMesh = new();
            // Unity defaults to 16-bit indices (UInt16), which caps the vertex count at 65535.
            // Switch to UInt32 when we expect to exceed that limit.
            if (totalSourceVertices > 65535)
            {
                combinedMesh.indexFormat = IndexFormat.UInt32;
            }

            combinedMesh.CombineMeshes(combineInstances);
            transform.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
        }
    }
}