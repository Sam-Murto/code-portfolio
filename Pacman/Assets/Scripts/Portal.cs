using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Portal : MonoBehaviour
{

    [SerializeField]
    Portal connectedPortal;

    [SerializeField]
    Vector2 dropOffPosition;

    public Vector2 GetDropOffPosition() => dropOffPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 newPosition = collision.transform.position; 
        newPosition.x = connectedPortal.GetDropOffPosition().x;
        newPosition.y = connectedPortal.GetDropOffPosition().y;
        collision.transform.position = newPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(dropOffPosition, Vector3.one * 0.5f);
    }
}
