using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseGhostBehavior : MonoBehaviour
{
    [SerializeField]
    protected float duration;
    [SerializeField]
    protected float speed;
    public Ghost ghost { get; protected set; }


}
