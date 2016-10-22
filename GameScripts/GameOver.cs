using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;


public class GameOver : PunBehaviour
{
    public bool gameOver = false, gameDone = false;
    public string winner = "", loser = "";
    private Color greyedPanel = new Color(0.5f, 0.5f, 0.5f, 0.6f);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            //stream.SendNext(gameOver);
            stream.SendNext(winner);
        }
        else
        {
            // Network player, receive data
            //this.gameOver = (bool)stream.ReceiveNext();
            this.winner = (string)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && !gameDone)
        {
            DisplayWinner();
            GetComponent<GameState>().gameState = GameState.State.GameOver;
            StartCoroutine("DelayBeforeRestart");
            PhotonNetwork.RaiseEvent(2, null, true, null);
            gameOver = false;
            gameDone = true;
        }
    }
    void OnWinnerChanged(string theWinner)
    {
        winner = theWinner;
    }
    //Display the winner of the game
    void DisplayWinner()
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
                loser = player.name;
                player.GetComponent<PlayerID>().winner = false;
            }
        }
        GameObject.Find(winner).GetComponent<UIManager>().DisplayPopupText("You have won the game!", true);
        LeaderbordController.PostScores(winner, 1, 0);
        if (GameObject.Find(loser) != null)
        {
            GameObject.Find(loser).GetComponent<UIManager>().DisplayPopupText(winner + " has won the game!", true);
            LeaderbordController.PostScores(loser, 0, 1);
        }
    }
    IEnumerator DelayBeforeRestart()
    {
        //Wait 5 seconds before resetting
        yield return new WaitForSeconds(5);
        StopAllCoroutines();
        ResetGame();
    }
    //Reset the game
    void ResetGame()
    {
        GameObject.Find(winner).GetComponent<UIManager>().DisplayPopupText("Restarting game...", true);
        if (GameObject.Find(loser) != null)
            GameObject.Find(loser).GetComponent<UIManager>().DisplayPopupText("Restarting game...", true);
        foreach (var tempObj in GameObject.FindGameObjectsWithTag("CenterSquare"))
        {
            if (tempObj.GetComponent<Light>() != null)
                tempObj.GetComponent<Light>().enabled = false;
            else if (tempObj.GetComponentInParent<Light>() != null)
                tempObj.GetComponentInParent<Light>().enabled = false;
            tempObj.GetComponentInChildren<Renderer>().enabled = false;
        }
        foreach (var tempObj in GameObject.FindGameObjectsWithTag("Line"))
        {
            if (tempObj.GetComponent<Light>() != null)
                tempObj.GetComponent<Light>().enabled = false;
            else if (tempObj.GetComponentInParent<Light>() != null)
                tempObj.GetComponentInParent<Light>().enabled = false;
            tempObj.GetComponentInChildren<Renderer>().enabled = false;
        }
        GameObject.Find("GameManager").GetComponent<GameStart>().DestroyGrid();
        var players = GameObject.FindGameObjectsWithTag("Player");
        var timerTexts = GameObject.FindGameObjectsWithTag("TimerText");
        GameObject.Find("GameManager").GetComponent<GameStart>().buildGrid = true;
        GameObject.Find("GameManager").GetComponent<GameStart>().startGame = true;
        GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Hide(false);
        //Update timer
        foreach (var player in players)
        {
            player.GetComponent<PlayerID>().isPlayersTurn = false;
            player.GetComponent<PlayerID>().winner = false;
            player.GetComponent<PlayerClick>().playingAnim = false;
            player.GetComponent<PlayerClick>().playingSquareAnim = false;
            player.GetComponent<PlayerID>().playerScore = 0;
            player.GetComponent<PlayerID>().playerTurnOrder = 0;
            player.GetComponent<PlayerID>().showWinner = true;
            GameObject.Find(player.GetComponent<PlayerID>().playersPanel).GetComponent<Image>().color = greyedPanel;
            for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
            {
                if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == player.GetComponent<PlayerID>().playerID)
                {
                    foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
                    {
                        //Reset scores
                        if (scores.name.Contains((i + 1).ToString()))
                        {
                            //Update UI with score
                            scores.GetComponent<Text>().text = player.GetComponent<PlayerID>().playerScore.ToString();
                        }
                    }
                    foreach (var timerText in timerTexts)
                    {
                        timerText.GetComponent<Text>().text = "";
                    }
                }
            }

        }
        gameObject.GetComponent<TurnTimer>().timer = GLOBALS.MAXTURNTIME;
        gameObject.GetComponent<TurnTimer>().enabled = false;
        StopAllCoroutines();
        gameOver = false;
        gameDone = false;
    }

}
