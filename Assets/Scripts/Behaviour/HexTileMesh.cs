using Base;
using UnityEngine;

namespace Behaviour
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class HexTileMesh : MonoBehaviour
    {
        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _collider;

        private static readonly int[] TrianglesPoints =
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1
        };
        
        private static readonly Vector3[] Vertices = {
            Vector3.zero, 
            new (0, 0, TileProperties.OuterRadius), 
            new (TileProperties.InnerRadius, 0, 0.5f * TileProperties.OuterRadius), 
            new (TileProperties.InnerRadius, 0, -0.5f * TileProperties.OuterRadius), 
            new (0, 0, -TileProperties.OuterRadius), 
            new (-TileProperties.InnerRadius, 0, -0.5f * TileProperties.OuterRadius), 
            new (-TileProperties.InnerRadius, 0, 0.5f * TileProperties.OuterRadius)
        };

        private void Awake()
        {
            _mesh = new Mesh();
            _mesh.name = "Hex Mesh";
            
            _meshFilter = GetComponent<MeshFilter>();
            _collider = GetComponent<MeshCollider>();
            
            RecalculateMesh();
            
            _meshFilter.mesh = _mesh;
            _collider.sharedMesh = _mesh;
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void RecalculateMesh()
        {
            _mesh.Clear();
            _mesh.vertices = Vertices;
            _mesh.triangles = TrianglesPoints;
            _mesh.RecalculateNormals();
        }

        public void SetColor(Color color)
        {
            Color[] colors = new Color[7];
            for (int i = 0; i < 7; i++)
                colors[i] = color;
            _mesh.colors = colors;

            _meshRenderer.material.color = color;
        }

        public Color GetColor()
        {
            return _meshRenderer.material.color;
        }
    }
}