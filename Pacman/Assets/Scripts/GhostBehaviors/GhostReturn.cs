using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class GhostReturn : BaseGhostBehavior
{

    void OnEnable()
    {
        ghost = GetComponent<Ghost>();
        ghost.isInvisible = true;
        gameObject.layer = 10;
        ghost.material.color = ghost.invisibleColor;
        ghost.movement.SetSpeed(speed);
        ghost.SetTarget(FindObjectOfType<HomeEntrance>().transform);

    }

    private void OnDisable()
    {
        CancelInvoke();
        StopAllCoroutines();
        ghost.isInvisible = false;
        ghost.movement.SetCheckCollisions(true);
        gameObject.layer = 8;
        ghost.SetTarget(FindObjectOfType<Pacman>().transform);
    }


    public IEnumerator EnterHomeCoroutine()
    {

        float duration = 0.2f;
        float normalize_t = 1.0f / duration;

        Vector3 position = transform.position;

        //Lerp to entry point
        for(float t = 0.0f;  t < duration; t += Time.deltaTime)
        {
            Vector3 newPosition = Vector3.Lerp(position, ghost.target.position, t * normalize_t);
            newPosition.z = position.z;
            ghost.transform.position = newPosition;
            yield return null;
        }

        duration = 0.5f;
        normalize_t = 1.0f / duration;

        position = transform.position;
        Vector3 homeCenter = FindObjectOfType<HomeCenter>().transform.position;

        //Lerp to home center
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            Vector3 newPosition = Vector3.Lerp(position, homeCenter, t * normalize_t);
            newPosition.z = position.z;
            ghost.transform.position = newPosition;
            yield return null;
        }

        position = transform.position;

        //Lerp to rest point
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            Vector3 newPosition = Vector3.Lerp(position, ghost.restPosition, t * normalize_t);
            newPosition.z = position.z;
            ghost.transform.position = newPosition;
            yield return null;
        }


        ghost.TransitionToBehavior(ghost.home);

    }


    //Select direction that takes ghost home
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.enabled) return;

        Node node = collision.gameObject.GetComponent<Node>();

        if (node != null)
        {
            ghost.movement.SetDirection(ChooseDirection(node));
        }

        if (collision.tag == "HomeEntrance") 
            StartCoroutine(EnterHomeCoroutine());

    }

    //Choose direction that results in moving closer to the target
    private Vector2 ChooseDirection(Node node)
    {
        Vector2 newDirection = node.validDirections[0];
        float minDistance = DistanceToTarget(newDirection);

        foreach (Vector2 direction in node.validDirections)
        {
            float distance = DistanceToTarget(direction);
            if (distance < minDistance && direction != -ghost.movement.GetDirection())
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
