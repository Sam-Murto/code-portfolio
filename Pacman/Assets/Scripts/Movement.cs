using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField]
    float speed = 8;
    [SerializeField]
    float speedMultiplier = 1;

    [SerializeField]
    private Vector2 startDirection = Vector2.zero;
    [SerializeField]
    private Vector3 startPosition;

    Vector2 direction;
    Vector2 nextDirection = Vector2.zero;
    
    public LayerMask obstacleLayer;

    Rigidbody2D rigidBody;
    bool checkCollisions = true;

    private void OnEnable()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        direction = startDirection;
        
    }

    void FixedUpdate()
    {
        if (GameManager.isPaused) return;

        rigidBody.MovePosition(rigidBody.position + direction * speed * Time.fixedDeltaTime);
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection, !checkCollisions);
        }

        transform.rotation = Quaternion.identity;
    }

    public Vector2 GetDirection()
    {
        return direction;
    }

    public void SetDirection(Vector2 newDirection, bool forced = false)
    {
        if (!Occupied(newDirection) || forced)
        {
            direction = newDirection;
            nextDirection = Vector2.zero;
        }
        else
        {
            nextDirection = newDirection;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(rigidBody.position, Vector2.one * 0.75f, 0f, direction, 1.5f, obstacleLayer);
        return hit.collider != null;
    }

    public void ResetState()
    {
        direction = startDirection;
        transform.position = startPosition;
    }

    public void SetSpeed(float speed) => this.speed = speed;

    public void SetCheckCollisions(bool checkCollisions)
    {
        rigidBody.isKinematic = !checkCollisions;
        this.checkCollisions = checkCollisions;
    }
}
