using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tracks horizontal and vertical collisions based on its travel direction. Subclasses are responsible for handling implmentation of collision handling in regards to immediate movement
// Based off of Sebastian Lague's 2D platformer controller
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Controller2D : MonoBehaviour
{
    private BoxCollider2D _collider;
    private RaycastOrigins _raycastOrigins;
    protected CollisionInfo _collisions;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float _horizontalSpacing = 0.2f;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float _verticalSpacing = 0.2f;
    [SerializeField]

    protected float _skinWidth = 0.015f;

    private void OnEnable()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private int CalculateHorizontalRays(Bounds bounds)
    {
        int numRays = (int)(bounds.size.y / _horizontalSpacing) + 1;
        return numRays;
    }

    private float CalculateHorizontalSpacing(int numRays, Bounds bounds)
    {
        float spacing = bounds.size.y / (numRays - 1);
        return spacing;
    }

    private int CalculateVerticalRays(Bounds bounds)
    {
        int numRays = (int)(bounds.size.x / _verticalSpacing) + 1;
        return numRays;
    }

    private float CalculateVerticalSpacing(int numRays, Bounds bounds)
    {
        float spacing = bounds.size.x / (numRays - 1);
        return spacing;
    }

    private void HorizontalCollisions(ref Vector2 moveAmount, Bounds bounds)
    {
        int directionX = (int)Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + _skinWidth;
        Vector2 origin = (directionX > 0) ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;
        int numRays = CalculateHorizontalRays(bounds);
        float spacing = CalculateHorizontalSpacing(numRays, bounds);
        Color color = Color.red;
        int layerMask = ~(1 << gameObject.layer);

        for(int i = 0; i < numRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * directionX, rayLength, layerMask);

            if (hit)
            {
                _collisions.Right = directionX > 0;
                _collisions.Left = directionX < 0;
                color = Color.green;
                HandleHorizontalCollision(ref moveAmount, directionX, hit);
                rayLength = hit.distance;
            }

            Debug.DrawRay(origin, rayLength * Vector2.right * directionX, color);

            origin.y += spacing;
        }


    }

    private void VerticalCollisions(ref Vector2 moveAmount, Bounds bounds)
    {
        int directionY = (int)Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + _skinWidth;
        Vector2 origin = (directionY > 0) ? _raycastOrigins.TopLeft : _raycastOrigins.BottomLeft;
        int numRays = CalculateVerticalRays(bounds);
        float spacing = CalculateVerticalSpacing(numRays, bounds);
        Color color = Color.red;
        int layerMask = ~(1 << gameObject.layer);

        for (int i = 0; i < numRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * directionY, rayLength, layerMask);

            if (hit)
            {
                _collisions.Above = directionY > 0;
                _collisions.Below = directionY < 0;
                color = Color.green;
                HandleVerticalCollisions(ref moveAmount, directionY, hit);
                rayLength = hit.distance;
            }

            Debug.DrawRay(origin, rayLength * Vector2.up * directionY, color);

            origin.x += spacing;
        }


    }

    private void UpdateRaycastOrigins(Bounds bounds)
    {
        _raycastOrigins = new RaycastOrigins(bounds);
    }

    public void Move(Vector2 velocity)
    {
        Vector2 moveAmount = velocity * Time.deltaTime;
        Bounds bounds = _collider.bounds;
        bounds.Expand(-_skinWidth * 2);

        _collisions.Reset();
        UpdateRaycastOrigins(bounds);
        if(moveAmount.x != 0)
            HorizontalCollisions(ref moveAmount, bounds);
        if(moveAmount.y != 0)
            VerticalCollisions(ref moveAmount, bounds);

        transform.Translate(moveAmount);
        Physics2D.SyncTransforms();
    }

    protected abstract void HandleHorizontalCollision(ref Vector2 moveAmount, int directionX, RaycastHit2D hit);
    protected abstract void HandleVerticalCollisions(ref Vector2 moveAmount, int directionY, RaycastHit2D hit);

    protected void SetCollider() => _collider = GetComponent<BoxCollider2D>();

    public bool CollisionAbove() => _collisions.Above;
    public bool CollisionBelow() => _collisions.Below;
    public bool CollisionRight() => _collisions.Right;
    public bool CollisionLeft() => _collisions.Left;
    public Collider2D CollisionObject() => _collisions.CollidingObject;

    public readonly struct RaycastOrigins
    {
        public readonly Vector2 TopLeft, TopRight, BottomLeft, BottomRight;
        
        public RaycastOrigins(Bounds bounds)
        {
            TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            TopRight = new Vector2(bounds.max.x, bounds.max.y);
            BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        }

    }

    //Contains all relevant collision information
    public struct CollisionInfo
    {
        public bool Above, Below, Right, Left;
        public Collider2D CollidingObject;


        public void Reset()
        {
            Above = false;
            Below = false;
            Right = false;
            Left = false;
        }

    }

}
