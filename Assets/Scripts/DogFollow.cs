using UnityEngine;
using System.Collections;

public class DogFollow : MonoBehaviour
{
    public Transform player;

    public float speed = 3f;
    public float followDistance = 3f;
    public float stopDistance = 2f;
    public float mushroomStopDistance = 1f;

    public float abilityCooldown = 15f;
    public int maxUsesPerLevel = 3;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private static readonly int MovingHash = Animator.StringToHash("Moving");
    private static readonly int FacingBackHash = Animator.StringToHash("FacingBack");
    private static readonly int GrowlHash = Animator.StringToHash("Growl");
    private static readonly int HappyHash = Animator.StringToHash("Happy");

    private int usesLeft;
    private bool abilityOnCooldown = false;
    private bool isUsingAbility = false;

    private enum DogState
    {
        FollowingPlayer,
        GoingToMushroom,
        ShowingMushroom
    }

    private DogState state = DogState.FollowingPlayer;
    private Transform targetMushroom;

    void Start()
    {
        usesLeft = maxUsesPerLevel;
    }

    void OnMouseDown()
    {
        TryUseAbility();
    }

    void Update()
    {
        if (player == null) return;

        if (state == DogState.FollowingPlayer)
        {
            FollowPlayer();
        }
        else if (state == DogState.GoingToMushroom)
        {
            GoToMushroom();
        }
    }

    void TryUseAbility()
    {
        if (abilityOnCooldown) return;
        if (usesLeft <= 0) return;
        if (isUsingAbility) return;

        targetMushroom = FindClosestGoodMushroom();

        if (targetMushroom == null) return;

        usesLeft--;
        isUsingAbility = true;
        abilityOnCooldown = true;
        state = DogState.GoingToMushroom;

        StartCoroutine(CooldownRoutine());
    }

    Transform FindClosestGoodMushroom()
    {
        GameObject[] mushrooms = GameObject.FindGameObjectsWithTag("GoodMushroom");

        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject mushroom in mushrooms)
        {
            float distance = Vector2.Distance(transform.position, mushroom.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = mushroom.transform;
            }
        }

        return closest;
    }

    void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 moveDirection = Vector2.zero;
        bool moving = false;

        if (distance > followDistance)
        {
            moveDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;

            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );

            moving = true;
        }
        else if (distance < stopDistance)
        {
            moveDirection = ((Vector2)transform.position - (Vector2)player.position).normalized;

            transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
            moving = true;
        }

        UpdateAnimation(moveDirection, moving);
    }

    void GoToMushroom()
    {
        if (targetMushroom == null)
        {
            ReturnToFollow();
            return;
        }

        Vector2 moveDirection = ((Vector2)targetMushroom.position - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetMushroom.position);

        if (distance > mushroomStopDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetMushroom.position,
                speed * Time.deltaTime
            );

            UpdateAnimation(moveDirection, true);
        }
        else
        {
            StartCoroutine(ShowMushroomRoutine());
        }
    }

    IEnumerator ShowMushroomRoutine()
    {
        state = DogState.ShowingMushroom;

        if (_animator != null)
        {
            _animator.SetBool(MovingHash, false);
            _animator.SetTrigger(GrowlHash);
        }

        yield return new WaitForSeconds(1f);

        if (_animator != null)
        {
            _animator.SetTrigger(HappyHash);
        }

        yield return new WaitForSeconds(1f);

        ReturnToFollow();
    }

    void ReturnToFollow()
    {
        targetMushroom = null;
        isUsingAbility = false;
        state = DogState.FollowingPlayer;
    }

    IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(abilityCooldown);
        abilityOnCooldown = false;
    }

    void UpdateAnimation(Vector2 moveDirection, bool moving)
    {
        if (moving && moveDirection != Vector2.zero)
        {
            bool facingBack = moveDirection.y > 0.1f;

            if (_spriteRenderer != null)
            {
                if (moveDirection.x > 0.1f)
                    _spriteRenderer.flipX = false;
                else if (moveDirection.x < -0.1f)
                    _spriteRenderer.flipX = true;
            }

            if (_animator != null)
                _animator.SetBool(FacingBackHash, facingBack);
        }

        if (_animator != null)
            _animator.SetBool(MovingHash, moving);
    }
}