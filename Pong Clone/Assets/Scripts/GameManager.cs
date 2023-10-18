using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Player _player1;
    int _player1Score;

    [SerializeField]
    Player _player2;
    int _player2Score;

    [SerializeField]
    [Range(0, 12)]
    int _maxScore;

    [SerializeField]
    private Ball _ball;
    [SerializeField]
    private float _ballStartSpeed;

    public bool GameIsOver { get; private set; } = false;
    private int _winner;

    public static bool IsPaused { get; private set;} = false;
    private float _currentPauseTime;
    private float _maxPauseTime;

    private delegate void UnPauseProcess();
    UnPauseProcess _unPauseProcess;

    public delegate void GameOver(int winner);
    public static event GameOver OnGameOver;

    public delegate void GameReset();
    public static event GameReset OnGameReset;


    private void OnEnable()
    {
        Ball.OnScoreGoal += OnScoreGoal;
        ResetGame();
    }

    private void OnDisable()
    {
        Ball.OnScoreGoal -= OnScoreGoal;
    }

    //Increments pause timer
    private void Update()
    {
        if (IsPaused && _currentPauseTime < _maxPauseTime)
            _currentPauseTime += Time.unscaledDeltaTime;
        else if (_maxPauseTime != 0)
        {
            UnPause();
        }

    }

    //Indefinite pause
    private void Pause()
    {
        IsPaused = true;
        _currentPauseTime = 0;
        _maxPauseTime = 0;
    }

    //Pause for amount of time
    private void Pause(float pauseTime)
    {
        IsPaused = true;
        _currentPauseTime = 0.0f;
        _maxPauseTime = pauseTime;
    }

    //Pause for certain amount of time, then perform process right before unpausing
    private void Pause(float pauseTime, UnPauseProcess unPauseProcess)
    {
        _unPauseProcess = unPauseProcess;
        IsPaused = true;
        _currentPauseTime = 0.0f;
        _maxPauseTime = pauseTime;
    }

    //Unpause the game
    private void UnPause()
    {
        if(_unPauseProcess != null)
        {
            _unPauseProcess();
            _unPauseProcess = null;
        }

        //UnPauseProcess may contain another pause inside of it. If a nested pause has reset the time, then the game will not be unpaused
        if (_currentPauseTime > _maxPauseTime)
        {
            IsPaused = false;
            _currentPauseTime = 0;
            _maxPauseTime = 0;
        }



    }

    //The game is over, called when max score is reached
    private void EndGame(int winner)
    {
        Pause();
        _ball.gameObject.SetActive(false);
        _winner = winner;
        OnGameOver?.Invoke(winner);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //Reset the whole game
    public void ResetGame()
    {
        _player1Score = 0;
        _player2Score = 0;
        GameIsOver = false;
        OnGameReset?.Invoke();
        ResetState();
    }

    //Reset the states of the ball and players to start a new round
    private void ResetState()
    {

        _ball.SetStartAngle(RandomBallAngle());
        _ball.gameObject.SetActive(false);

        _player1.gameObject.SetActive(false);
        _player2.gameObject.SetActive(false);


        _player1.gameObject.SetActive(true);
        _player2.gameObject.SetActive(true);
        _ball.gameObject.SetActive(true);

        Pause(1.0f);
        
    }

    //When a goal is scored, pause for 0.5 seconds then score the goal just before unpausing
    private void OnScoreGoal()
    {
        Pause(0.5f, ScoreGoal);
    }

    //Give a point to the player that the ball is NOT behind, if the max score is reached end the game, otherwise reset the state for a new round
    private void ScoreGoal()
    {
        int playerWhoScored = (_ball.transform.position.x < _player1.transform.position.y) ? 2 : 1;

        if (playerWhoScored == 2)
        {
            _player2Score++;
        }
        else
        {
            _player1Score++;
        }

        if (_player1Score >= _maxScore || _player2Score >= _maxScore)
        {

            EndGame(playerWhoScored);
            return;
        }

        ResetState();


    }

    //Needs to be between -45.0f and 45.0f either in the left or right direction
    private float RandomBallAngle()
    {
        float ballAngle = Random.Range(-45.0f, 45.0f);
        int ballDirection = Random.Range(0, 2);;
        ballAngle += (ballDirection == 0) ? 0.0f : 180.0f;
        return ballAngle;
    }

    public int GetPlayer1Score() => _player1Score;
    public int GetPlayer2Score() => _player2Score;
    public int GetWinner() => _winner;

}
