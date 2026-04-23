using UnityEngine;

public class DogFollow : MonoBehaviour
{
    public Transform player;

    public float speed = 3f;
    public float followDistance = 3f;   // cand incepe sa te urmareasca
    public float stopDistance = 2f;   // unde se opreste
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private static readonly int IsMovingHash = Animator.StringToHash("Moving");
    private static readonly int FacingBackHash = Animator.StringToHash("FacingBack");
    [SerializeField] private Animator _animator;
    private bool Moving = false;
    private bool FacingBack = false;

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        if (vertical > 0f) FacingBack = true;
        else if (vertical < 0f) FacingBack = false;
        if (_spriteRenderer != null)
        {
            if (horizontal > 0f) _spriteRenderer.flipX = FacingBack;
            else if (horizontal < 0f) _spriteRenderer.flipX = FacingBack == false;
        }

        // daca e prea departe -> vine dupa player
        if (distance > followDistance)
        {

            MoveTowardsPlayer();
        }
        // daca e prea aproape -> se retrage putin (optional)
        else if (distance < stopDistance)
        {

            MoveAwayFromPlayer();
        }
        if (_animator != null)
        {
            _animator.SetBool(IsMovingHash, Moving);
            _animator.SetBool(FacingBackHash, FacingBack);
        }

        // altfel -> sta (zona ideala)
    }

    void MoveTowardsPlayer()
    {

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
        Moving = true;
    }

    void MoveAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        Moving = false;
    }
}