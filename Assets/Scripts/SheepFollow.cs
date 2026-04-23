using UnityEditor.MPE;
using UnityEngine;

public class SheepFollow : MonoBehaviour
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
    [SerializeField] private Animator _sheep;
    private bool Moving = false;
    private bool FacingBack = false;


    // nou
    public float arriveRadius = 0.6f;     // cât de aproape începe să încetinească
    public float stopEpsilon = 0.05f;     // sub distanța asta, se oprește

    private bool isFollowing = false;
    private Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // recomandat pt MovePosition ca să fie mai smooth
        if (rb != null) rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void OnMouseDown()
    {
        isFollowing = true;
    }

    void FixedUpdate()
    {

        if (!isFollowing || player == null || rb == null) return;

        Vector2 targetPosition = (Vector2)player.position + followOffset;
        Vector2 toTarget = targetPosition - rb.position;
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        float distanceToTarget = toTarget.magnitude;

        // 1) FOLLOW cu decelerare (arrive)
        Vector2 followVelocity = Vector2.zero;

        if (distanceToTarget > followDistance)
        {
            Moving = true;
            // viteză maximă
            float t = 1f;

            // dacă e aproape de target, scade viteza gradual
            if (distanceToTarget < arriveRadius)
                t = Mathf.InverseLerp(0f, arriveRadius, distanceToTarget);

            followVelocity = toTarget.normalized * (speed * t);

            // dacă e foarte aproape, oprește ca să nu “vâneze” punctul
            if (distanceToTarget < stopEpsilon)
                followVelocity = Vector2.zero;
            if (vertical > 0f) FacingBack = true;
            else if (vertical < 0f) FacingBack = false;
        }
        else
        {
            Moving = false;
        }

        // 2) SEPARATION (ca viteză, nu direcție normalizată)
        Vector2 separationDir = GetSeparationDirection();
        Vector2 separationVelocity = separationDir * separationStrength * speed;

        // 3) combină și limitează
        Vector2 finalVelocity = followVelocity + separationVelocity;
        finalVelocity = Vector2.ClampMagnitude(finalVelocity, speed);
        if (_sheep != null)
        {
            _sheep.SetBool(IsMovingHash, Moving);
            _sheep.SetBool(FacingBackHash, FacingBack);
        }
        if (_spriteRenderer != null)
        {
            if (horizontal > 0f) _spriteRenderer.flipX = FacingBack;
            else if (horizontal < 0f) _spriteRenderer.flipX = FacingBack == false;
        }
        rb.MovePosition(rb.position + finalVelocity * Time.fixedDeltaTime);
    }

    Vector2 GetSeparationDirection()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        Vector2 separation = Vector2.zero;

        foreach (Collider2D other in nearby)
        {
            if (other.gameObject == gameObject) continue;

            if (other.CompareTag("Sheep") || other.CompareTag("Dog"))
            {
                Vector2 diff = (Vector2)(transform.position - other.transform.position);
                float distance = diff.magnitude;

                if (distance > 0.01f)
                {
                    separation += diff.normalized / distance;
                }
            }
        }

        return separation.sqrMagnitude > 0f ? separation.normalized : Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}