using UnityEngine;

namespace MioritzaGame.Game
{
    [ExecuteAlways]
    public sealed class ApaTrigger : MonoBehaviour
    {
        [SerializeField] private Vector3[] _localPoints = new[]
        {
            new Vector3( 10f, 0f,  6f),
            new Vector3(-10f, 0f,  6f),
            new Vector3(-10f, 0f, -6f),
            new Vector3( 10f, 0f, -6f),
        };

        [SerializeField] private int _minPoints = 100;
        [SerializeField] private int _maxPoints = 200;
        [SerializeField] private float _cooldownSeconds = 0.6f;

        private float _nextAllowed;
        private Transform _player;

        public Vector3[] LocalPoints
        {
            get => _localPoints;
            set => _localPoints = value;
        }

        private void Update()
        {
            if (Application.isPlaying == false) return;
            if (_localPoints == null || _localPoints.Length < 3) return;
            if (Time.time < _nextAllowed) return;

            if (_player == null) TryFindPlayer();
            if (_player == null) return;

            var rel = _player.position - transform.position;

            if (IsPointInPolygon(rel.x, rel.z) == false) return;

            var ctx = _player.GetComponent<PlayerContext>();
            if (ctx == null || ctx.IsDead == true) return;

            var amount = Random.Range(_minPoints, _maxPoints + 1);
            ctx.InsanityChange(amount);
            _nextAllowed = Time.time + _cooldownSeconds;

            ScreenEffectOverlay.Show(new Color(0.1f, 0.25f, 0.6f, 0.28f), 0.6f, vignette: true, grain: false);
            MushroomToast.Show("TOXIC SUBSTANCE", "Your insanity rises.", new Color(0.55f, 0.7f, 1f), 1.6f);
        }

        private bool IsPointInPolygon(float x, float z)
        {
            var inside = false;
            for (int i = 0, j = _localPoints.Length - 1; i < _localPoints.Length; j = i++)
            {
                if (((_localPoints[i].z > z) != (_localPoints[j].z > z)) &&
                    (x < (_localPoints[j].x - _localPoints[i].x) * (z - _localPoints[i].z) / (_localPoints[j].z - _localPoints[i].z) + _localPoints[i].x))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        private void TryFindPlayer()
        {
            var ctx = Object.FindAnyObjectByType<PlayerContext>();
            if (ctx == null) return;
            _player = ctx.transform;
        }

        private void OnDrawGizmos()
        {
            if (_localPoints == null || _localPoints.Length < 2) return;

            var origin = transform.position;
            Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.9f);
            for (var i = 0; i < _localPoints.Length; i++)
            {
                var a = origin + new Vector3(_localPoints[i].x, 0f, _localPoints[i].z);
                var next = (i + 1) % _localPoints.Length;
                var b = origin + new Vector3(_localPoints[next].x, 0f, _localPoints[next].z);
                Gizmos.DrawLine(a, b);
            }
            for (var i = 0; i < _localPoints.Length; i++)
            {
                Gizmos.DrawSphere(origin + new Vector3(_localPoints[i].x, 0f, _localPoints[i].z), 0.4f);
            }
        }
    }
}
