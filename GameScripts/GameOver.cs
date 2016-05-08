using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : NetworkBehaviour
{
    [SyncVar]
    public bool gameOver = false;
    [SyncVar(hook = "OnWinnerChanged")]
    public string winner = "";

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            StartCoroutine("DisplayWinner");
        }
    }
    void OnWinnerChanged(string theWinner)
    {
        winner = theWinner;
    }
    //Display the winner of the game
    IEnumerator DisplayWinner()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        List<int> scores = new List<int>();
        //Gather all the scores and see who is the highest
        foreach (var player in players)
        {
            scores.Add(player.GetComponent<PlayerID>().playerScore);
        }
        int highestScore = scores.Max();
        //Display the winner
        foreach (var player in players)
        {
            //The winner
            if (player.GetComponent<PlayerID>().playerScore == highestScore)
            {
                winner = player.name;
                player.GetComponent<PlayerID>().winner = true;

            }
            else
            {
                player.GetComponent<PlayerID>().winner = false;
            }
        }
        //Wait 5 seconds before resetting
        yield return new WaitForSeconds(5);
        StopAllCoroutines();
        ResetGame();
    }
    //Reset the game
    void ResetGame()
    {
        GameObject.Find("GameManager").GetComponent<GameStart>().DestroyGrid();
        var players = GameObject.FindGameObjectsWithTag("Player");
        var timerTexts = GameObject.FindGameObjectsWithTag("TimerText");
        //Update timer
        foreach (var player in players)
        {
            player.GetComponent<PlayerID>().isPlayersTurn = false;
            player.GetComponent<PlayerID>().winner = false;
            player.GetComponent<PlayerID>().playerScore = 0;
            player.GetComponent<PlayerID>().playerTurnOrder = 0;
            player.GetComponent<PlayerID>().showWinner = true;
            for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
            {
                if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == player.GetComponent<PlayerID>().playerID)
                {
                    foreach (var timerText in timerTexts)
                    {
                        timerText.GetComponent<Text>().text = "";
                    }
                }
            }

        }
        gameObject.GetComponent<TurnTimer>().timer = GLOBALS.MAXTURNTIME;
        gameObject.GetComponent<TurnTimer>().enabled = false;
        GameObject.Find("GameManager").GetComponent<GameStart>().buildGrid = true;
        GameObject.Find("GameManager").GetComponent<GameStart>().startGame = true;
        StopAllCoroutines();
        gameOver = false;
    }

}
