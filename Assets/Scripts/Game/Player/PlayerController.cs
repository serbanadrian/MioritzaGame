using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _moveSpeed = 5f;
        [SerializeField, Min(1)] private int _maxHealth = 100;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int FacingBackHash = Animator.StringToHash("FacingBack");

        private Camera _camera;
        private Vector2 _input;
        private bool _isFacingBack;
        private int _currentHealth;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;

        private void Awake()
        {
            _camera = Camera.main;
            _currentHealth = _maxHealth;

            if (_animator == null) _animator = GetComponent<Animator>();
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_rigidbody2D == null) _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            if (_rigidbody2D == null) MoveCameraRelative();

            UpdateFacingAndAnimation();
        }

        private void FixedUpdate()
        {
            if (_rigidbody2D == null) return;

            var velocity = _input.sqrMagnitude > 1f ? _input.normalized : _input;
            _rigidbody2D.linearVelocity = velocity * _moveSpeed;
        }

        private void MoveCameraRelative()
        {
            if (_camera == null)
            {
                Debug.LogError($"{nameof(PlayerController)} missing main {nameof(Camera)} for camera-relative movement.");
                return;
            }

            var camRight = _camera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            var camForward = _camera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            var direction = camRight * _input.x + camForward * _input.y;
            if (direction.sqrMagnitude > 1f) direction.Normalize();

            transform.position += direction * (_moveSpeed * Time.deltaTime);
        }

        private void UpdateFacingAndAnimation()
        {
            var horizontal = _input.x;
            var vertical = _input.y;
            var isMoving = _input.sqrMagnitude > 0f;

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

        public void TakeDamage(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogError($"{nameof(PlayerController)}.{nameof(TakeDamage)} expected positive {nameof(amount)} but got {amount}.");
                return;
            }

            _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
            if (_currentHealth == 0) Die();
        }

        public void Heal(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogError($"{nameof(PlayerController)}.{nameof(Heal)} expected positive {nameof(amount)} but got {amount}.");
                return;
            }

            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        }

        private void Die()
        {
            Debug.Log($"{nameof(PlayerController)} died.");
        }
    }
}
