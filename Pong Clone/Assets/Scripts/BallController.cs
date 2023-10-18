using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Solely responsible to handling the immediate movement of the ball and tracking collision information
public class BallController : Controller2D
{

    protected override void HandleHorizontalCollision(ref Vector2 moveAmount, int directionX, RaycastHit2D hit)
    {
        float distanceTohit = hit.distance - _skinWidth;

        //Bounce off of surface
        moveAmount.x = -moveAmount.x;

        _collisions.Right = moveAmount.x > 0;
        _collisions.Left = moveAmount.x < 0;
        _collisions.CollidingObject = hit.collider;

        //Also needs to travel the distance to actually hit the surface, so subtract that from the moveAmount based on direction
        moveAmount.x -= distanceTohit * directionX;


    }

    protected override void HandleVerticalCollisions(ref Vector2 moveAmount, int directionY, RaycastHit2D hit)
    {
        float distanceTohit = hit.distance - _skinWidth;

        //Bounce off of surface
        moveAmount.y = -moveAmount.y;

        _collisions.Above = moveAmount.y > 0;
        _collisions.Below = moveAmount.y < 0;
        _collisions.CollidingObject = hit.collider;

        //Also needs to travel the distance to actually hit the surface, so subtract that from the moveAmount based on direction
        moveAmount.y -= distanceTohit * ((directionY > 0) ? 1 : -1);


    }

}
