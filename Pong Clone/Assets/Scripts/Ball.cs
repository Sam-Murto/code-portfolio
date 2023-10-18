using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallController))]
public class Ball : MonoBehaviour
{
    public delegate void ScoreGoal();
    public static event ScoreGoal OnScoreGoal;

    private BallController _controller;

    [SerializeField]
    private float _speed;
    private Vector2 _velocity;
    [SerializeField]
    private float _startSpeed = 4.0f;
    [SerializeField]
    private Vector2 _startPosition = Vector2.zero;
    [SerializeField]
    private float _speedPercentIncreaseOnPlayerHit = 0.01f;
    private float _startAngle = 0.0f;

    [SerializeField]
    private float _maxDeflectAngle = 60.0f;

    void OnEnable()
    {
        _controller = GetComponent<BallController>();
        Reset();
    }

    
    void Update()
    {
        if (GameManager.IsPaused)
            return;


        //We know we are colliding with either a player or a goal, since the only other two objects are the top and bottom of the map which will always be a vertical collision
        if (_controller.CollisionLeft() || _controller.CollisionRight())
        {

            Collider2D collider = _controller.CollisionObject();

            //When hitting the side of the player, the ball should deflect and increase its speed
            if (collider.tag == "Player")
            {
                _speed += _startSpeed * _speedPercentIncreaseOnPlayerHit;
                Deflect(collider);
            }
            //Otherwise we are hitting a goal, but checking just in case
            else if(collider.tag == "Goal")
            {
                OnScoreGoal?.Invoke();
            }

        }

        if(_controller.CollisionAbove() || _controller.CollisionBelow())
        {
            _velocity.y = -_velocity.y;
        }


        _controller.Move(_velocity);

    }
    
    // When deflecting, the ball will bounce at an angle based on the Y distance from the center of what it's bouncing off of.
    private void Deflect(Collider2D collider)
    {
        float collisionPositionY = collider.transform.position.y;
        float positionY = transform.position.y;
        float directionX = Mathf.Sign(_velocity.x);


        float distanceYFromCenter = Mathf.Abs(collisionPositionY - positionY);
        float deflectAngle = Mathf.Lerp(0.0f, _maxDeflectAngle, distanceYFromCenter / collider.bounds.size.y);

        //Deflect X velocity to the opposite direction
        _velocity.x = _speed * Mathf.Cos(deflectAngle * Mathf.Deg2Rad) * -directionX;
        //Deflect Y velocity down if ball is below center of player, up otherwise
        _velocity.y = _speed * Mathf.Sin(deflectAngle * Mathf.Deg2Rad) * ((positionY < collisionPositionY) ? -1 : 1);
    }

    public void SetStartAngle(float newStartAngle)
    {
        _startAngle = newStartAngle;
    }

    private void Reset()
    {
        transform.position = _startPosition;
        float startVelocityX = _startSpeed * Mathf.Cos(_startAngle * Mathf.Deg2Rad);
        float startVelocityY = _startSpeed * Mathf.Sin(_startAngle * Mathf.Deg2Rad);
        _velocity = new Vector2(startVelocityX, startVelocityY);
        _speed = _startSpeed;
    }

}
