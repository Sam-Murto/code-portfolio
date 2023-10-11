using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(GhostScatter))]
[RequireComponent(typeof(GhostChase))]
[RequireComponent(typeof(GhostFlee))]
[RequireComponent(typeof(GhostReturn))]
[RequireComponent(typeof(GhostHome))]
public class Ghost : MonoBehaviour
{
    public int value { get; private set; } = 200;

    public Vector3 startPosition = Vector3.zero;
    public Vector3 restPosition = Vector3.zero;

    public Movement movement { get; private set; }

    public Transform target;


    public Color defaultColor;
    public Color vulnerableColor;
    public Color invisibleColor;

    [SerializeField]
    public Material material;


    [SerializeField]
    private BaseGhostBehavior startBehavior;
    [SerializeField]
    private BaseGhostBehavior currentBehavior;

    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFlee flee { get; private set; }
    public GhostReturn returnHome { get; private set; }
    public GhostHome home { get; private set; }

    public bool isVulnerable = false;
    public bool isInvisible = false;
    public bool isHome = false;

    void OnEnable()
    {
        movement = GetComponent<Movement>();
        scatter = GetComponent<GhostScatter>();
        scatter.enabled = false;
        chase = GetComponent<GhostChase>();
        chase.enabled = false;
        flee = GetComponent<GhostFlee>();
        flee.enabled = false;
        returnHome = GetComponent<GhostReturn>();
        returnHome.enabled = false;
        home = GetComponent<GhostHome>();
        home.enabled = false;
        SetTarget(FindObjectOfType<Pacman>().transform);

        currentBehavior = startBehavior;
        currentBehavior.enabled = true;

    }

    void Update()
    {

    }

    public void TransitionToBehavior(BaseGhostBehavior newBehavior)
    {
        currentBehavior.enabled = false;
        currentBehavior = newBehavior;
        currentBehavior.enabled = true;
    }

    public void ResetState()
    {
        movement.ResetState();
        TransitionToBehavior(startBehavior);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    //Ghosts that are invisible will not collide with Pacman
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Pacman pacman = collision.gameObject.GetComponent<Pacman>();
        if (pacman != null)
        {

            if (isVulnerable)
            {
                //Get eaten by Pacman
                pacman.EatGhost(this);
            }
            else if(!isInvisible)
            {
                //Eat Pacman
                pacman.Die();
            }
        }
    }

    public void BecomeVulnerable()
    {
        if (currentBehavior == returnHome) return;

        isVulnerable = true;
        material.color = vulnerableColor;


        if(currentBehavior != home)
        {
            TransitionToBehavior(flee);
        }

    }

    public void BecomeNormal()
    {
        if (currentBehavior == returnHome) return;

        isVulnerable = false;
        material.color = defaultColor;

        if (currentBehavior != home)
        {
            TransitionToBehavior(scatter);
        }
    }

}
