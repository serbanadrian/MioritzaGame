using UnityEngine;

namespace MioritzaGame.Game
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshCollider))]
    public sealed class PolygonDeadZone : MonoBehaviour
    {
        [SerializeField] private Vector3[] _localPoints = new[]
        {
            new Vector3(0.5f, 0f, 0.5f),
            new Vector3(-0.5f, 0f, 0.5f),
            new Vector3(-0.5f, 0f, -0.5f),
            new Vector3(0.5f, 0f, -0.5f),
        };

        [SerializeField, Min(1f)] private float _height = 500f;

        private MeshCollider _collider;
        private Mesh _mesh;

        public Vector3[] LocalPoints
        {
            get => _localPoints;
            set
            {
                _localPoints = value;
                Rebuild();
            }
        }

        private void OnEnable() => Rebuild();
        private void OnValidate() => Rebuild();

        private void Rebuild()
        {
            if (_localPoints == null || _localPoints.Length < 2) return;
            if (_collider == null) _collider = GetComponent<MeshCollider>();
            if (_collider == null) return;

            if (_mesh == null) _mesh = new Mesh { name = nameof(PolygonDeadZone) };
            _mesh.Clear();

            var count = _localPoints.Length;
            var vertices = new Vector3[count * 2];
            for (var i = 0; i < count; i++)
            {
                var p = _localPoints[i];
                vertices[i] = new Vector3(p.x, 0f, p.z);
                vertices[i + count] = new Vector3(p.x, _height, p.z);
            }

            var triangles = new int[count * 12];
            var t = 0;
            for (var i = 0; i < count; i++)
            {
                var next = (i + 1) % count;
                triangles[t++] = i;
                triangles[t++] = next + count;
                triangles[t++] = next;
                triangles[t++] = i;
                triangles[t++] = i + count;
                triangles[t++] = next + count;
                triangles[t++] = next;
                triangles[t++] = next + count;
                triangles[t++] = i;
                triangles[t++] = next + count;
                triangles[t++] = i + count;
                triangles[t++] = i;
            }

            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();

            _collider.sharedMesh = null;
            _collider.convex = false;
            _collider.sharedMesh = _mesh;
        }

        private void OnDrawGizmos()
        {
            if (_localPoints == null || _localPoints.Length < 2) return;
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.9f);
            Gizmos.matrix = transform.localToWorldMatrix;
            for (var i = 0; i < _localPoints.Length; i++)
            {
                var a = _localPoints[i];
                var b = _localPoints[(i + 1) % _localPoints.Length];
                Gizmos.DrawLine(a, b);
            }
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
