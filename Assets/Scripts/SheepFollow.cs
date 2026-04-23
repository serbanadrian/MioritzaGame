using UnityEngine;

public class SheepFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float followDistance = 3f;
    public Vector2 followOffset;

    public float separationRadius = 1.2f;
    public float separationStrength = 2f;

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
        float distanceToTarget = toTarget.magnitude;

        // 1) FOLLOW cu decelerare (arrive)
        Vector2 followVelocity = Vector2.zero;

        if (distanceToTarget > followDistance)
        {
            // viteză maximă
            float t = 1f;

            // dacă e aproape de target, scade viteza gradual
            if (distanceToTarget < arriveRadius)
                t = Mathf.InverseLerp(0f, arriveRadius, distanceToTarget);

            followVelocity = toTarget.normalized * (speed * t);

            // dacă e foarte aproape, oprește ca să nu “vâneze” punctul
            if (distanceToTarget < stopEpsilon)
                followVelocity = Vector2.zero;
        }

        // 2) SEPARATION (ca viteză, nu direcție normalizată)
        Vector2 separationDir = GetSeparationDirection();
        Vector2 separationVelocity = separationDir * separationStrength * speed;

        // 3) combină și limitează
        Vector2 finalVelocity = followVelocity + separationVelocity;
        finalVelocity = Vector2.ClampMagnitude(finalVelocity, speed);

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