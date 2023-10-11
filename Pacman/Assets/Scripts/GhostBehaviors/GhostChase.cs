using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChase : BaseGhostBehavior
{
    private void OnEnable()
    {
        ghost = GetComponent<Ghost>();
        ghost.movement.SetSpeed(speed);
        ghost.material.color = ghost.defaultColor;
        Invoke(nameof(Scatter), duration);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.enabled) return;


        Node node = collision.gameObject.GetComponent<Node>();

        if (node != null)
        {
            ghost.movement.SetDirection(ChooseDirection(node));
        }
    }

    //Choose direction that results in moving closer to the target
    private Vector2 ChooseDirection(Node node)
    {
        Vector2 newDirection = node.validDirections[0];
        float minDistance = DistanceToTarget(newDirection);

        foreach(Vector2 direction in node.validDirections)
        {
            float distance = DistanceToTarget(direction);
            if (distance < minDistance)
            {
                minDistance = distance;
                newDirection = direction;

            }
        }

        return newDirection;
    }

    private float DistanceToTarget(Vector2 testPosition)
    {
        Vector2 target = ghost.target.transform.position;

        return Vector2.Distance((Vector2)transform.position + testPosition, target);

    }

    private void Scatter()
    {
        ghost.TransitionToBehavior(ghost.scatter);
    }

     

}
