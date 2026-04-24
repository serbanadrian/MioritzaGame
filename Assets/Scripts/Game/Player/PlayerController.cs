using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _moveSpeed = 5f;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int FacingBackHash = Animator.StringToHash("FacingBack");

        private Camera _camera;
        private bool _isFacingBack;

        private void Awake()
        {
            _camera = Camera.main;
            if (_camera == null)
                Debug.LogError($"{nameof(PlayerController)} could not find main {nameof(Camera)}.");
        }

        private void Update()
        {
            if (_camera == null) return;

            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");

            var camRight = _camera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            var camForward = _camera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            var direction = camRight * horizontal + camForward * vertical;
            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            transform.position += direction * (_moveSpeed * Time.deltaTime);

            var isMoving = direction.sqrMagnitude > 0f;

            if (vertical > 0f) _isFacingBack = true;
            else if (vertical < 0f) _isFacingBack = false;

            if (_animator != null)
            {
                _animator.SetBool(IsMovingHash, isMoving);
                _animator.SetBool(FacingBackHash, _isFacingBack);
            }

            if (_spriteRenderer != null)
            {
                if (horizontal > 0f) _spriteRenderer.flipX = _isFacingBack;
                else if (horizontal < 0f) _spriteRenderer.flipX = _isFacingBack == false;
            }
        }
    }
}
