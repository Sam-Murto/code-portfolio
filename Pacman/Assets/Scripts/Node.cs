using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Node : MonoBehaviour
{
    public List<Vector2> validDirections { get; private set; }

    [SerializeField]
    private LayerMask obstacleLayer;

    private void OnEnable()
    {
        validDirections = new List<Vector2>();
        CheckDirection(Vector2.up);
        CheckDirection(Vector2.down);
        CheckDirection(Vector2.left);
        CheckDirection(Vector2.right);
    }

    private void OnDisable()
    {
        validDirections.Clear();
    }

    private void CheckDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0f, direction, 1.5f, obstacleLayer);
        if (!hit)
        {
            validDirections.Add(direction);
        }

    }

    private void OnDrawGizmos()
    {
        Color color = Color.white;
        color.a = 0.3f;
        Gizmos.color = color;

        Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);

    }
}
