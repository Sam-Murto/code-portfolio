using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GhostScatter : BaseGhostBehavior
{

    private void OnEnable()
    {
        ghost = GetComponent<Ghost>();
        ghost.movement.SetSpeed(speed);
        ghost.material.color = ghost.defaultColor;
        Invoke(nameof(Chase), duration);
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

        //Node handling, change to random direction that isn't the opposite direction
        Node node = collision.gameObject.GetComponent<Node>();

        if (node != null)
        {
            int numDirections = node.validDirections.Count;
            int index = Random.Range(0, numDirections);
            Vector2 direction = node.validDirections[index];
            if (direction == -ghost.movement.GetDirection())
            {
                if (index == numDirections - 1)
                    index = 0;
                else
                    index++;
            }

            ghost.movement.SetDirection(node.validDirections[index]);
        }
    }

    //So I can call invoke. Implementation of a timer class should make my code much cleaner
    private void Chase()
    {
        ghost.TransitionToBehavior(ghost.chase);
    }


}
