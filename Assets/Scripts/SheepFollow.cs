using UnityEngine;
using MioritzaGame.Constants;

public class SheepFollow : MonoBehaviour
{
    [Header("Follow")]
    public Transform player;
    public float speedMultiplier = 1.2f;
    public float fallbackSpeed = 5f;
    public float followDistance = 3f;
    public Vector3 followOffset = new Vector3(1f, 0f, -1f);

    [Header("Separation")]
    public float separationRadius = 1.2f;
    public float separationStrength = 1.2f;

    [Header("Smooth Stop")]
    public float arriveRadius = 0.6f;
    public float stopEpsilon = 0.05f;

    [Header("Pickup")]
    public float pickupRadius = 4f;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private static readonly int MovingHash = Animator.StringToHash("Moving");
    private static readonly int FacingBackHash = Animator.StringToHash("FacingBack");

    public bool isFollowing = false;

    private MioritzaGame.Game.PlayerController playerController;
    private bool facingBack;
    private bool playerInRange;

    private float CurrentSpeed
    {
        get
        {
            var basis = playerController != null ? playerController.MoveSpeed : fallbackSpeed;
            return basis * speedMultiplier;
        }
    }

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        TryFindPlayer();
    }

    void Update()
    {
        if (player == null) TryFindPlayer();
        if (player == null) return;
        if (isFollowing == true) return;

        var distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= pickupRadius;

        if (playerInRange == true && Input.GetKeyDown(KeyCode.E) == true)
        {
            isFollowing = true;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnGUI()
    {
        if (isFollowing == true || playerInRange == false) return;
        var cam = Camera.main;
        if (cam == null) return;

        var screenPos = cam.WorldToScreenPoint(transform.position);
        var rect = new Rect(screenPos.x - 120f, Screen.height - screenPos.y - 80f, 240f, 28f);
        var style = new GUIStyle(GUI.skin.box);
        style.fontSize = 16;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        GUI.Label(rect, Texts.Sheep.PRESS_E_FOLLOW, style);
    }

    void FixedUpdate()
    {
        if (isFollowing == false || player == null) return;

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = player.position + followOffset;
        Vector3 toTarget = targetPosition - transform.position;

        Vector3 followVelocity = Vector3.zero;
        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget > followDistance)
        {
            float arriveSlowdown = 1f;

            if (distanceToTarget < arriveRadius)
                arriveSlowdown = Mathf.InverseLerp(0f, arriveRadius, distanceToTarget);

            followVelocity = toTarget.normalized * CurrentSpeed * arriveSlowdown;

            if (distanceToTarget < stopEpsilon)
                followVelocity = Vector3.zero;
        }

        Vector3 separationVelocity = GetSeparationDirection3D() * separationStrength * CurrentSpeed;

        Vector3 finalVelocity = followVelocity + separationVelocity;

        if (finalVelocity.magnitude > CurrentSpeed)
            finalVelocity = finalVelocity.normalized * CurrentSpeed;

        transform.position += finalVelocity * Time.fixedDeltaTime;

        bool moving = finalVelocity.sqrMagnitude > 0.001f;
        UpdateAnimation(finalVelocity, moving);
    }

    private void TryFindPlayer()
    {
        var pc = Object.FindAnyObjectByType<MioritzaGame.Game.PlayerController>();
        if (pc == null) return;
        playerController = pc;
        player = pc.transform;
    }

    private void UpdateAnimation(Vector3 velocity, bool moving)
    {
        if (animator != null)
            animator.SetBool(MovingHash, moving);

        if (moving == false || velocity.sqrMagnitude < 0.001f)
            return;

        if (velocity.z > 0.1f)
            facingBack = true;
        else if (velocity.z < -0.1f)
            facingBack = false;

        if (spriteRenderer != null)
        {
            if (velocity.x > 0.1f)
                spriteRenderer.flipX = facingBack;
            else if (velocity.x < -0.1f)
                spriteRenderer.flipX = facingBack == false;
        }

        if (animator != null)
            animator.SetBool(FacingBackHash, facingBack);
    }

    private Vector3 GetSeparationDirection3D()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, separationRadius);
        Vector3 separation = Vector3.zero;

        foreach (Collider other in nearby)
        {
            if (other.gameObject == gameObject) continue;

            if (other.CompareTag("Sheep") || other.CompareTag("Dog"))
            {
                Vector3 diff = transform.position - other.transform.position;
                float distance = diff.magnitude;

                if (distance > 0.01f)
                    separation += diff.normalized / distance;
            }
        }

        return separation.sqrMagnitude > 0f ? separation.normalized : Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
