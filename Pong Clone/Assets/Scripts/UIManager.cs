using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameManager _game;

    [SerializeField]
    private TextMeshProUGUI _player1ScoreIndicator;
    [SerializeField]
    private TextMeshProUGUI _player2ScoreIndicator;
    [SerializeField]
    private TextMeshProUGUI _winnerText;
    [SerializeField]
    private Button _playAgainButton;
    [SerializeField]
    private Button _backToMenuButton;

    // Start is called before the first frame update
    void OnEnable()
    {
        _winnerText.gameObject.SetActive(false);
        GameManager.OnGameOver += DisplayWinScreen;
        GameManager.OnGameReset += DisableWinScreen;

    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= DisplayWinScreen;
        GameManager.OnGameReset -= DisableWinScreen;
    }

    // Update is called once per frame
    void Update()
    {
        _player1ScoreIndicator.SetText("{0:00.}", _game.GetPlayer1Score());
        _player2ScoreIndicator.SetText("{0:00.}", _game.GetPlayer2Score());

    }

    private void DisplayWinScreen(int winner)
    {
        _winnerText.SetText("Player {0} wins!", winner);
        _winnerText.gameObject.SetActive(true);
        _playAgainButton.gameObject.SetActive(true);
        _backToMenuButton.gameObject.SetActive(true);
    }

    private void DisableWinScreen()
    {
        EventSystem.current.SetSelectedGameObject(null);
        _winnerText.gameObject.SetActive(false);
        _playAgainButton.gameObject.SetActive(false);
        _backToMenuButton.gameObject.SetActive(false);

    }

}
