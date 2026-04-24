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
    private bool FacingBack;


    // nou
    public float arriveRadius = 0.6f;     // cât de aproape începe să încetinească
    public float stopEpsilon = 0.05f;     // sub distanța asta, se oprește

    public bool isFollowing = false;
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

        // Player is moving on X and Z axis, but Sheep uses a 2D Rigidbody.
        // If the game uses a 3D world (X/Z plane), a 2D Rigidbody (X/Y) will not work properly here natively without syncing.
        // It looks like the sheep operates entirely on XY physics or was meant to.
        // Let's assume you wanted XZ 3D movement but mapped to Vector2 for the logic.
        Vector3 targetPosition3D = player.position + new Vector3(followOffset.x, 0f, followOffset.y);

        Vector3 currentPosition3D = rb.transform.position;
        Vector3 toTarget3D = targetPosition3D - currentPosition3D;

        // Zero out the Y axis since floor is flat
        toTarget3D.y = 0f;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        float distanceToTarget = toTarget3D.magnitude;

        // 1) FOLLOW cu decelerare (arrive)
        Vector3 followVelocity = Vector3.zero;

        if (distanceToTarget > followDistance)
        {
            Moving = true;
            // viteză maximă
            float t = 1f;

            // dacă e aproape de target, scade viteza gradual
            if (distanceToTarget < arriveRadius)
                t = Mathf.InverseLerp(0f, arriveRadius, distanceToTarget);

            followVelocity = toTarget3D.normalized * (speed * t);

            // dacă e foarte aproape, oprește ca să nu “vâneze” punctul
            if (distanceToTarget < stopEpsilon)
                followVelocity = Vector3.zero;

            if (vertical > 0f) FacingBack = true;
            else if (vertical < 0f) FacingBack = false;
        }
        else
        {
            Moving = false;
            followVelocity = Vector3.zero;
        }

        // 2) SEPARATION
        Vector3 separationDir = GetSeparationDirection3D();
        Vector3 separationVelocity = separationDir * separationStrength * speed;

        // 3) combină și limitează
        Vector3 finalVelocity = followVelocity + separationVelocity;
        if (finalVelocity.magnitude > speed) finalVelocity = finalVelocity.normalized * speed;

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

        // If Rigidbody2D is used, it only moves XY. If the floor is XZ, we should physically move the transform or apply it properly.
        // Assuming your game is effectively a 3D space with sprites (2.5D):
        transform.position += finalVelocity * Time.fixedDeltaTime;
        // rb.velocity is generally for 2D XY physics, so transforming position manually prevents them skipping below map.
    }

    Vector3 GetSeparationDirection3D()
    {
        // For 3D 2.5D overlap
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