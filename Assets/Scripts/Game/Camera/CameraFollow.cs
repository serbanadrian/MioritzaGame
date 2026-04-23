using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField, Min(0f)] private float _smoothTime = 0.2f;

        private Vector3 _offset;
        private Vector3 _velocity;

        private void Start()
        {
            if (_target == null)
            {
                Debug.LogError($"{nameof(CameraFollow)} missing {nameof(_target)}.");
                enabled = false;
                return;
            }

            _offset = transform.position - _target.position;
        }

        private void LateUpdate()
        {
            var desired = _target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, _smoothTime);
        }
    }
}
