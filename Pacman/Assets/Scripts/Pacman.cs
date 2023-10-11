using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{

    public delegate void PelletEaten(NormalPellet pellet);
    public static event PelletEaten OnPelletEaten;

    public delegate void PowerPelletEaten(PowerPellet pellet);
    public static event PowerPelletEaten OnPowerPelletEaten;

    public delegate void GhostEaten(Ghost ghost);
    public static event GhostEaten OnGhostEaten;

    public delegate void PacmanDeath();
    public static event PacmanDeath OnPacmanDeath;

    Movement movement;

    public Vector2 startPosition { get; private set; } = new Vector2(0, -7.5f);

    void OnEnable()
    {
        gameObject.name = "Pacman";
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        if (GameManager.isPaused) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement.SetDirection(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement.SetDirection(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement.SetDirection(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement.SetDirection(Vector2.right);
        }
    }

    //Handles collisions with pellets, ghost related collisions will be handled by ghost
    private void OnTriggerEnter2D(Collider2D collider)
    {

        //Eating normal pellet
        NormalPellet pellet = collider.gameObject.GetComponent <NormalPellet>();

        if (pellet != null)
        {
            OnPelletEaten?.Invoke(pellet);
        }

        PowerPellet powerPellet = collider.gameObject.GetComponent<PowerPellet>();

        if(powerPellet != null)
        {
            OnPowerPelletEaten?.Invoke(powerPellet);
        }

    }

    public void ResetState()
    {
        movement.ResetState();
    }

    public void Die()
    {
        OnPacmanDeath?.Invoke();
    }

    public void EatGhost(Ghost ghost)
    {
        OnGhostEaten?.Invoke(ghost);
    }

}
