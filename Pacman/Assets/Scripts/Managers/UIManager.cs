using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameManager game;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI livesText;
    [SerializeField]
    private TextMeshProUGUI readyText;


    // Update is called once per frame
    void Update()
    {
        //Update Score
        scoreText.SetText("SCORE: {0}", GameStateData.score);
        //Update Lives
        livesText.SetText("LIVES: {0}", game.lives);

    }

    public async void ReadySequence(float readyTime, float goTime)
    {
        float end = Time.unscaledTime + readyTime;

        readyText.text = "Ready...";
        readyText.gameObject.SetActive(true);

        //Wait for readyTime
        while(Time.unscaledTime < end)
        {
            await Task.Yield();
        }

        readyText.text = "Go!";

        end = Time.unscaledTime + goTime;

        //Wait for readyTime
        while (Time.unscaledTime < end)
        {
            await Task.Yield();
        }

        readyText.gameObject.SetActive(false);

    }



}
