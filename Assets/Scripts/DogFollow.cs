using UnityEngine;

public class DogFollow : MonoBehaviour
{
    public Transform player;

    public float speed = 3f;
    public float followDistance = 3f;   // cand incepe sa te urmareasca
    public float stopDistance = 2f;   // unde se opreste

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

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
        // altfel -> sta (zona ideala)
    }

    void MoveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
    }

    void MoveAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}