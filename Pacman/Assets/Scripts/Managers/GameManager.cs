using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Ghost[] ghosts;

    [SerializeField]
    private Pacman player;

    [SerializeField]
    private Transform pellets;

    private float pauseTime = 0.0f;
    private float currentPauseTime = 0.0f;

    public static bool isPaused;

    public int lives { get; private set; } = 5;
    public int multiplier { get; private set; } = 1;

    public delegate void Unpaused();
    public static event Unpaused OnUnpaused;

    private void OnEnable()
    {
        Pacman.OnPelletEaten += HandlePelletEaten;
        Pacman.OnPowerPelletEaten += HandlePowerPelletEaten;
        Pacman.OnGhostEaten += HandleGhostEaten;
        Pacman.OnPacmanDeath += HandlePacmanDeath;
    }

    private void OnDisable()
    {
        Pacman.OnPelletEaten -= HandlePelletEaten;
        Pacman.OnPowerPelletEaten -= HandlePowerPelletEaten;
        Pacman.OnGhostEaten -= HandleGhostEaten;
        Pacman.OnPacmanDeath -= HandlePacmanDeath;
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NewGame();
        }

        if (isPaused && currentPauseTime < pauseTime)
            currentPauseTime += Time.unscaledDeltaTime;
        else if(currentPauseTime > pauseTime)
            UnPause();
       

    }

    //Pause indefinitely
    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    private void Pause(float time)
    {
        isPaused = true;
        pauseTime = time;
        Time.timeScale = 0;
    }

    private void UnPause()
    {
        pauseTime = 0;
        currentPauseTime = 0;
        isPaused = false;
        Time.timeScale = 1;

        
        OnUnpaused?.Invoke();
        OnUnpaused = null;
    }

    //Reset entire game
    private void NewGame()
    {
        SetScore(0);
        SetLives(3);

        ResetRound();
    }

    //Reset Pellets, Pacman, and Ghosts
    void ResetRound()
    {
        //Reset Pellets
        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();

    }

    //Reset Pacman and the Ghosts' state
    void ResetState()
    {
        Pause(3.0f);
        FindObjectOfType<UIManager>().ReadySequence(1.5f, 1.5f);

        //Reset ghosts
        foreach (Ghost ghost in ghosts)
        {
            ghost.gameObject.SetActive(false);
            ghost.gameObject.SetActive(true);
            ghost.ResetState();
        }

        //Reset Pacman
        player.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
        player.ResetState();
        SetMultiplier(1);

    }

    void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void SetScore(int newScore)
    {
        GameStateData.score = newScore;
    }

    void SetLives(int newLives)
    {
        lives = newLives;
    }

    void SetMultiplier(int newMultiplier)
    {
        multiplier = newMultiplier;
    }

    void HandlePelletEaten(NormalPellet pellet)
    { 
        SetScore(GameStateData.score + pellet.value);

        pellet.gameObject.SetActive(false);

        if (!HasRemainingPellets())
            ResetRound();
    }

    void HandlePowerPelletEaten(PowerPellet pellet)
    {
        SetScore(GameStateData.score + pellet.value);

        pellet.gameObject.SetActive(false);

        MakeGhostsVulnerable(pellet.duration);

        if(!HasRemainingPellets())
            ResetRound();


    }

    void HandleGhostEaten(Ghost ghost)
    {
        Pause(0.5f);

        SetScore(GameStateData.score + ghost.value * multiplier);
        multiplier++;
        //Make eaten Ghost invisible
        ghost.TransitionToBehavior(ghost.returnHome);

    }

    void HandlePacmanDeath()
    {
        SetLives(lives - 1);

        //Pause for a couple seconds
        Pause(1.5f);

        if (lives > 0)
        {
            //Reset the state
            OnUnpaused += ResetState;   
        } 
        else
        {
            //Game over
            OnUnpaused += GameOver;
        }
    }

    bool HasRemainingPellets()
    {
        foreach(Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    void MakeGhostsVulnerable(float duration)
    {
        //Make all active Ghosts vulnerable
        foreach (Ghost ghost in ghosts)
        {
            if(!ghost.isInvisible)
            {
                ghost.BecomeVulnerable();
            }
        }

        CancelInvoke(nameof(MakeGhostsNormal));
        Invoke(nameof(MakeGhostsNormal), duration);
    }

    void MakeGhostsNormal()
    {
        //Make all vulnerable ghosts active
        foreach(Ghost ghost in ghosts)
        {
            if(ghost.isVulnerable)
            {
                ghost.BecomeNormal();
            }
        }

        SetMultiplier(1);
    }
}
