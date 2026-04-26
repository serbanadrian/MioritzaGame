using UnityEngine;

namespace MioritzaGame.Game
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerConfiguration _configuration;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private bool _invertControls;

        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int FacingBackHash = Animator.StringToHash("FacingBack");

        private Camera _camera;
        private Vector2 _input;
        private bool _isFacingBack;
        private int _currentHealth;

        public int MaxHealth => _configuration != null ? _configuration.MaxHealth : 0;
        public int CurrentHealth => _currentHealth;
        public float MoveSpeed => _configuration != null ? _configuration.MoveSpeed : 0f;

        private void Awake()
        {
            if (_configuration == null)
            {
                Debug.LogError($"{nameof(PlayerController)} missing {nameof(_configuration)}.");
                enabled = false;
                return;
            }

            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            _camera = Camera.main;
            _currentHealth = _configuration.MaxHealth;
        }

        private void Update()
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            if (_invertControls == true) _input = -_input;

            UpdateFacingAndAnimation();
        }

        private void FixedUpdate()
        {
            if (_camera == null) return;

            var camTransform = _camera.transform;

            var forward = camTransform.up;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                forward = camTransform.forward;
            forward.y = 0f;
            forward.Normalize();

            var right = camTransform.right;
            right.y = 0f;
            right.Normalize();

            var direction = right * _input.x + forward * _input.y;
            if (direction.sqrMagnitude > 1f) direction.Normalize();

            var horizontalVelocity = direction * _configuration.MoveSpeed;
            var current = _rigidbody.linearVelocity;
            _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, current.y, horizontalVelocity.z);
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

        public void Spawn(Vector3 worldPosition, EntryFacing facing)
        {
            if (_rigidbody != null)
            {
                _rigidbody.position = worldPosition;
                _rigidbody.linearVelocity = Vector3.zero;
            }
            transform.position = worldPosition;

            _isFacingBack = facing == EntryFacing.Back;
            if (_animator != null) _animator.SetBool(FacingBackHash, _isFacingBack);

            if (_spriteRenderer != null)
            {
                if (facing == EntryFacing.Left) _spriteRenderer.flipX = true;
                else if (facing == EntryFacing.Right) _spriteRenderer.flipX = false;
            }

            TeleportFollowers(worldPosition);
        }

        private static void TeleportFollowers(Vector3 worldPosition)
        {
            var sheep = FindObjectsByType<SheepFollow>(FindObjectsSortMode.None);
            foreach (var s in sheep)
            {
                if (s.isFollowing == false) continue;
                s.transform.position = worldPosition;
            }
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogError($"{nameof(PlayerController)}.{nameof(TakeDamage)} expected positive {nameof(amount)} but got {amount}.");
                return;
            }

            _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _configuration.MaxHealth);
            if (_currentHealth == 0) Die();
        }

        public void Heal(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogError($"{nameof(PlayerController)}.{nameof(Heal)} expected positive {nameof(amount)} but got {amount}.");
                return;
            }

            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _configuration.MaxHealth);
        }

        private void Die()
        {
            Debug.Log($"{nameof(PlayerController)} died.");
        }
    }
}
