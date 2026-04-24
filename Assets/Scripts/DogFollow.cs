using UnityEngine;

public class DogFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float followDistance = 3f;
    public Vector2 followOffset;

    public float separationRadius = 1.2f;
    public float separationStrength = 2f;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private static readonly int IsMovingHash = Animator.StringToHash("Moving");
    private static readonly int FacingBackHash = Animator.StringToHash("FacingBack");
    [SerializeField] private Animator _animator;
    private bool Moving = false;
    private bool FacingBack;

    public float arriveRadius = 0.6f;
    public float stopEpsilon = 0.05f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();

        Vector3 targetPosition3D = player.position + new Vector3(followOffset.x, 0f, followOffset.y);
        Vector3 currentPosition3D = rb.transform.position;
        Vector3 toTarget3D = targetPosition3D - currentPosition3D;

        toTarget3D.y = 0f;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        bool isPlayerMoving = horizontal != 0f || vertical != 0f;

        float distanceToTarget = toTarget3D.magnitude;

        Vector3 followVelocity = Vector3.zero;

        // Dog logic: Only follow if player is moving or out of bounds
        if (isPlayerMoving || distanceToTarget > followDistance * 1.5f)
        {
            if (distanceToTarget > followDistance)
            {
                Moving = true;
                float t = 1f;

                if (distanceToTarget < arriveRadius)
                    t = Mathf.InverseLerp(0f, arriveRadius, distanceToTarget);

                followVelocity = toTarget3D.normalized * (speed * t);

                if (distanceToTarget < stopEpsilon)
                    followVelocity = Vector3.zero;
            }
            else
            {
                Moving = false;
                followVelocity = Vector3.zero;
            }
        }
        else
        {
            Moving = false;
            followVelocity = Vector3.zero;
        }

        Vector3 separationDir = GetSeparationDirection3D();
        Vector3 separationVelocity = separationDir * separationStrength * speed;

        Vector3 finalVelocity = followVelocity + separationVelocity;
        if (finalVelocity.magnitude > speed) finalVelocity = finalVelocity.normalized * speed;

        // Visual updates identical to sheep
        if (Moving || isPlayerMoving)
        {
            if (vertical > 0f) FacingBack = true;
            else if (vertical < 0f) FacingBack = false;

            if (_spriteRenderer != null)
            {
                if (horizontal > 0f) _spriteRenderer.flipX = FacingBack;
                else if (horizontal < 0f) _spriteRenderer.flipX = !FacingBack;
            }
        }

        if (_animator != null)
        {
            _animator.SetBool(IsMovingHash, Moving);
            _animator.SetBool(FacingBackHash, FacingBack);
        }

        transform.position += finalVelocity * Time.fixedDeltaTime;
    }

    Vector3 GetSeparationDirection3D()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, separationRadius);
        Vector3 separation = Vector3.zero;

        foreach (Collider other in nearby)
        {
            if (other.gameObject == gameObject) continue;

            if (other.CompareTag("Sheep") || other.CompareTag("Dog"))
            {
                Vector3 diff = (transform.position - other.transform.position);
                diff.y = 0f;
                float distance = diff.magnitude;

                if (distance > 0.01f)
                {
                    separation += diff.normalized / distance;
                }
            }
        }

        return separation.sqrMagnitude > 0f ? separation.normalized : Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}