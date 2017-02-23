using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon;
using Facebook.Unity;


public class GameOver : PunBehaviour
{
    public bool gameOver = false, gameDone = false;
    public string winner = "", loser = "";
    private Color greyedPanel = new Color(0.5f, 0.5f, 0.5f, 0.6f);
    void Start()
    {
        GameObject.Find("NetworkManager").GetComponent<NetworkManager>().findAnotherMatch = false;
    }
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
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ExitGames.Client.Photon.ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(2, null, true, options);
            gameOver = false;
            gameDone = true;
        }
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
        if (GameObject.Find(loser) != null) GameObject.Find(loser).GetComponent<UIManager>().DisplayPopupText(winner + " has won the game!", true);
        //Update player win loss UI
        for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
        {
            if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == GameObject.Find(winner).GetComponent<PlayerID>().playerID)
            {
                foreach (var stats in GameObject.FindGameObjectsWithTag("StatsText"))
                {
                    if (stats.name.Contains((i + 1).ToString()))
                    {
                        //Update UI with Wins and Losses
                        GameObject.Find(winner).GetComponent<PlayerID>().playersWins += 1;
                        stats.GetComponent<Text>().text = GameObject.Find(winner).GetComponent<PlayerID>().playersWins + " W "
                            + GameObject.Find(winner).GetComponent<PlayerID>().playerLosses + " L ";
                        Debug.Log(GameObject.Find(winner).GetComponent<PlayerID>().guestToken);
                        if (PhotonNetwork.isMasterClient)
                        {
                            if (GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken != "")
                            {
                                StartCoroutine(LeaderbordController.PostScores(winner, GameObject.Find(winner).GetComponent<PlayerID>().playersWins, GameObject.Find(winner).GetComponent<PlayerID>().playerLosses, GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken));
                            }
                            else
                            {
                                StartCoroutine(LeaderbordController.PostScores(GameObject.Find(winner).GetComponent<PlayerID>().guestToken, winner, GameObject.Find(winner).GetComponent<PlayerID>().playersWins, GameObject.Find(winner).GetComponent<PlayerID>().playerLosses));
                            }
                        }
                        break;
                    }
                }
            }
            else if (GameObject.Find(loser) != null && GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == GameObject.Find(loser).GetComponent<PlayerID>().playerID)
            {
                foreach (var stats in GameObject.FindGameObjectsWithTag("StatsText"))
                {
                    if (stats.name.Contains((i + 1).ToString()))
                    {
                        //Update UI with Wins and Losses
                        GameObject.Find(loser).GetComponent<PlayerID>().playerLosses += 1;
                        stats.GetComponent<Text>().text = GameObject.Find(loser).GetComponent<PlayerID>().playersWins + " W "
                            + GameObject.Find(loser).GetComponent<PlayerID>().playerLosses + " L ";
                        if (PhotonNetwork.isMasterClient)
                        {
                            if (GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken != "")
                                StartCoroutine(LeaderbordController.PostScores(loser, GameObject.Find(loser).GetComponent<PlayerID>().playersWins, GameObject.Find(loser).GetComponent<PlayerID>().playerLosses, GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken));
                            else
                                StartCoroutine(LeaderbordController.PostScores(GameObject.Find(loser).GetComponent<PlayerID>().guestToken, loser, GameObject.Find(loser).GetComponent<PlayerID>().playersWins, GameObject.Find(loser).GetComponent<PlayerID>().playerLosses));
                        }
                        break;
                    }
                }
            }
        }
    }
    IEnumerator DelayBeforeRestart()
    {
        //Wait 5 seconds before resetting
        yield return new WaitForSeconds(3);
        GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Hide(false);
        yield return new WaitForSeconds(2);
        StopAllCoroutines();
        ResetGame();
    }
    //Find another match button click
    public void FindAnotherMatch()
    {
        PhotonNetwork.Disconnect();
        StartCoroutine(DoSwitchLevel());
    }
    //need this or else level changes before we disconnect

    IEnumerator DoSwitchLevel()
    {
        while (PhotonNetwork.connected)
            yield return null;
        GameObject.Find("NetworkManager").GetComponent<NetworkManager>().findAnotherMatch = true;
        SceneManager.LoadScene(0);
    }
    //Reset the game
    void ResetGame()
    {
        GameObject.Find("PlayAgainMenu").GetComponent<DoozyUI.UIElement>().Show(false);
    }
    //Hide the menu if play again is pressed
    [PunRPC]
    public void HideMenu()
    {
        GameObject.Find("PlayAgainMenu").GetComponent<DoozyUI.UIElement>().moveOut.delay = 0;
        GameObject.Find("PlayAgainMenu").GetComponent<DoozyUI.UIElement>().Hide(false);
        GameObject.Find(winner).GetComponent<UIManager>().DisplayPopupText("Restarting game...", true);
        if (GameObject.Find(loser) != null)
        {
            GameObject.Find(loser).GetComponent<UIManager>().DisplayPopupText("Restarting game...", true);
        }
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
    //Play another game with the same player
    public void PlayAgain()
    {
        photonView.RPC("HideMenu", PhotonTargets.AllBuffered);
    }

}
