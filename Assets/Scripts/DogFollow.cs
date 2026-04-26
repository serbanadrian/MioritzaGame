using UnityEngine;
using System.Collections;

public class DogFollow : MonoBehaviour
{
    [Header("Follow")]
    public Transform player;
    public float speed = 3f;
    public float followDistance = 3f;
    public Vector3 followOffset = new Vector3(-1f, 0f, -1f);

    [Header("Separation")]
    public float separationRadius = 1.2f;
    public float separationStrength = 1.2f;

    [Header("Smooth Stop")]
    public float arriveRadius = 0.6f;
    public float stopEpsilon = 0.05f;

    [Header("Dog Ability")]
    public float mushroomStopDistance = 1f;
    public float abilityCooldown = 15f;
    public int maxUsesPerLevel = 3;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource barkAudioSource;

    private static readonly int MovingHash = Animator.StringToHash("Moving");
    private static readonly int FacingBackHash = Animator.StringToHash("FacingBack");
    private static readonly int GrowlHash = Animator.StringToHash("Growl");
    private static readonly int HappyHash = Animator.StringToHash("Happy");

    private Rigidbody2D rb;

    private int usesLeft;
    private bool abilityOnCooldown;
    private bool isUsingAbility;
    private bool facingBack;

    private Transform targetMushroom;
    private Coroutine showMushroomCoroutine;

    private enum DogState
    {
        FollowingPlayer,
        GoingToMushroom,
        ShowingMushroom
    }

    private DogState state = DogState.FollowingPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        usesLeft = maxUsesPerLevel;
    }

    void OnMouseDown()
    {
        TryUseAbility();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        switch (state)
        {
            case DogState.FollowingPlayer:
                FollowPlayer();
                break;

            case DogState.GoingToMushroom:
                GoToMushroom();
                break;

            case DogState.ShowingMushroom:
                break;
        }
    }

    private void TryUseAbility()
    {
        if (abilityOnCooldown) return;
        if (usesLeft <= 0) return;
        if (isUsingAbility) return;

        targetMushroom = FindClosestGoodMushroom();

        if (targetMushroom == null)
        {
            Debug.Log("No good mushroom found.");
            return;
        }

        usesLeft--;
        abilityOnCooldown = true;
        isUsingAbility = true;
        state = DogState.GoingToMushroom;

        StartCoroutine(CooldownRoutine());
    }

    private Transform FindClosestGoodMushroom()
    {
        GameObject[] mushrooms = GameObject.FindGameObjectsWithTag("GoodMushroom");

        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject mushroom in mushrooms)
        {
            Vector3 diff = mushroom.transform.position - transform.position;
            float distance = diff.magnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = mushroom.transform;
            }
        }

        return closest;
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = player.position + followOffset;
        targetPosition.y = transform.position.y; // Ignore vertical height differences
        Vector3 toTarget = targetPosition - transform.position;

        Vector3 followVelocity = Vector3.zero;
        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget > followDistance)
        {
            float speedMultiplier = 1f;

            if (distanceToTarget < arriveRadius)
                speedMultiplier = Mathf.InverseLerp(0f, arriveRadius, distanceToTarget);

            followVelocity = toTarget.normalized * speed * speedMultiplier;

            if (distanceToTarget < stopEpsilon)
                followVelocity = Vector3.zero;
        }

        Vector3 separationVelocity = GetSeparationDirection3D() * separationStrength * speed;

        Vector3 finalVelocity = followVelocity + separationVelocity;
        finalVelocity.y = 0f; // Ensure movement stays strictly horizontal

        if (finalVelocity.magnitude > speed)
            finalVelocity = finalVelocity.normalized * speed;

        transform.position += finalVelocity * Time.fixedDeltaTime;

        bool moving = finalVelocity.sqrMagnitude > 0.001f;
        UpdateAnimation(finalVelocity, moving);
    }

    private void GoToMushroom()
    {
        if (targetMushroom == null)
        {
            ReturnToFollow();
            return;
        }

        Vector3 targetPos = targetMushroom.position;
        targetPos.y = transform.position.y; // Ignore vertical height differences
        Vector3 toMushroom = targetPos - transform.position;
        float distance = toMushroom.magnitude;

        if (distance > mushroomStopDistance)
        {
            Vector3 moveVelocity = toMushroom.normalized * speed;
            Vector3 separationVelocity = GetSeparationDirection3D() * separationStrength * speed;

            Vector3 finalVelocity = moveVelocity + separationVelocity;
            finalVelocity.y = 0f; // Ensure movement stays strictly horizontal

            if (finalVelocity.magnitude > speed)
                finalVelocity = finalVelocity.normalized * speed;

            transform.position += finalVelocity * Time.fixedDeltaTime;

            UpdateAnimation(finalVelocity, true);
        }
        else
        {
            if (showMushroomCoroutine == null)
                showMushroomCoroutine = StartCoroutine(ShowMushroomRoutine());
        }
    }

    private IEnumerator ShowMushroomRoutine()
    {
        state = DogState.ShowingMushroom;

        // Activate and play the barking audio when mushroom is found
        if (barkAudioSource != null)
        {
            barkAudioSource.enabled = true;
            barkAudioSource.Play();
        }

        if (animator != null)
        {
            animator.SetBool(MovingHash, false);
            animator.SetTrigger(GrowlHash);
        }

        yield return new WaitForSeconds(1f);

        if (animator != null)
            animator.SetTrigger(HappyHash);

        yield return new WaitForSeconds(1f);

        showMushroomCoroutine = null;
        ReturnToFollow();
    }

    private void ReturnToFollow()
    {
        targetMushroom = null;
        isUsingAbility = false;
        state = DogState.FollowingPlayer;

        // Stop and deactivate the barking sound once the interaction is done
        if (barkAudioSource != null)
        {
            barkAudioSource.Stop();
            barkAudioSource.enabled = false;
        }

        if (animator != null)
        {
            animator.ResetTrigger(GrowlHash);
            animator.ResetTrigger(HappyHash);
            animator.SetBool(MovingHash, false);
        }
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(abilityCooldown);
        abilityOnCooldown = false;
    }

    private void UpdateAnimation(Vector3 velocity, bool moving)
    {
        if (animator != null)
            animator.SetBool(MovingHash, moving);

        if (!moving)
            return;

        if (velocity.z > 0.001f)
            facingBack = true;
        else if (velocity.z < -0.001f)
            facingBack = false;

        if (spriteRenderer != null)
        {
            if (velocity.x > 0.001f)
                spriteRenderer.flipX = facingBack;
            else if (velocity.x < -0.001f)
                spriteRenderer.flipX = !facingBack;
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
    }
}