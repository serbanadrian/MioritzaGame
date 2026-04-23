using UnityEngine;

public class SheepFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 4f;
    public float followDistance = 4f;
    public float stopDistance = 2.5f;

    private bool isFollowing = false;

    void OnMouseDown()
    {
        isFollowing = true;
    }

    void Update()
    {
        if (!isFollowing || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
    }
}