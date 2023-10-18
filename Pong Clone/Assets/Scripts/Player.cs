using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private KeyCode _upKey = KeyCode.W;
    [SerializeField]
    private KeyCode _downKey = KeyCode.S;
    [SerializeField]
    private float _moveSpeed = 4.0f;
    [SerializeField]
    private Vector2 _startPosition;

    private float _velocity;
    [SerializeField]
    private float _maxY = 4.25f;
    [SerializeField]
    private float _minY = -4.25f;

    // Start is called before the first frame update
    void OnEnable()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsPaused)
            return;

        if (Input.GetKey(_upKey))
        {
            _velocity += _moveSpeed;
        }

        if (Input.GetKey(_downKey))
        {
            _velocity += -_moveSpeed;
        }

        float moveAmount = _velocity * Time.deltaTime;
        float currentY = transform.position.y;

        if (currentY + moveAmount > _maxY) {
            moveAmount = _maxY - currentY;
        }

        if(currentY + moveAmount < _minY)
        {
            moveAmount = _minY - currentY;
        }

        transform.Translate(new Vector3(0.0f, moveAmount, 0.0f));

        //Reset velocity
        _velocity = 0.0f;

    }

    private void OnDrawGizmos()
    {
        float currentX = transform.position.x;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(currentX, _minY, 1), Vector3.one * 0.25f);
        Gizmos.DrawCube(new Vector3(currentX, _maxY, 1), Vector3.one * 0.25f);
    }

    public void Reset()
    {
        transform.position = _startPosition;
    }

}
