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
            stream.SendNext(gameOver);
            stream.SendNext(winner);
            stream.SendNext(loser);
        }
        else
        {
            // Network player, receive data
            this.gameOver = (bool)stream.ReceiveNext();
            this.winner = (string)stream.ReceiveNext();
            this.loser = (string)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && !gameDone && GetComponent<GameState>().gameState != GameState.State.GameOver)
        {
            photonView.RPC("GetWinner", PhotonTargets.AllBuffered);
            DisplayWinner();
            GetComponent<GameState>().gameState = GameState.State.GameOver;
            gameOver = false;
            gameDone = true;
        }
    }
    [PunRPC]
    public void GetWinner()
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
                winner = player.GetComponent<PlayerID>().playerID;
                player.GetComponent<PlayerID>().winner = true;

            }
            else
            {
                loser = player.GetComponent<PlayerID>().playerID;
                player.GetComponent<PlayerID>().winner = false;

            }
        }
        gameOver = false;
    }
    //Display the winner of the game
    public void DisplayWinner()
    {
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ExitGames.Client.Photon.ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(2, null, true, options);
    }
    IEnumerator DelayBeforeRestart()
    {
        //Wait 5 seconds before resetting
        yield return new WaitForSeconds(3);
        photonView.RPC("UpdatePlayerScores", PhotonTargets.AllBuffered);
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
    [PunRPC]
    public void UpdatePlayerScores()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        //Update player win loss UI
        for (int i = 0; i < GetComponent<GameStart>().playerNames.Count; i++)
        {
            foreach (var player in players)
            {
                if (GetComponent<GameStart>().playerNames[i] == player.GetComponent<PlayerID>().playerID)
                {
                    foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
                    {
                        if (scores.name.Contains((i + 1).ToString()))
                        {
                            //Update UI with score
                            scores.GetComponent<Text>().text = "Score: " + 0;
                        }
                    }
                    foreach (var stats in GameObject.FindGameObjectsWithTag("StatsText"))
                    {
                        if (stats.name.Contains((i + 1).ToString()))
                        {
                            //Update UI with Wins and Losses
                            stats.GetComponent<Text>().text = player.GetComponent<PlayerID>().playersWins + " W "
                             + player.GetComponent<PlayerID>().playerLosses + " L ";
                        }
                    }
                }
            }
        }
    }
    //Hide the menu if play again is pressed
    [PunRPC]
    public void HideMenu()
    {
        GameObject.Find("PlayAgainMenu").GetComponent<DoozyUI.UIElement>().moveOut.delay = 0;
        GameObject.Find("PlayAgainMenu").GetComponent<DoozyUI.UIElement>().Hide(false);
        foreach (var tempObj in GameObject.FindGameObjectsWithTag("CenterSquare"))
        {
            tempObj.GetComponentInChildren<Renderer>().enabled = false;
        }
        foreach (var tempObj in GameObject.FindGameObjectsWithTag("Line"))
        {
            tempObj.GetComponentInChildren<Renderer>().enabled = false;
        }
        GetComponent<GameStart>().DestroyGrid();
        var players = GameObject.FindGameObjectsWithTag("Player");
        GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Hide(false);
        //Update timer
        foreach (var player in players)
        {
            player.GetComponent<PlayerUIManager>().DisplayPopupText("Restarting game...", true);
            player.GetComponent<PlayerID>().isPlayersTurn = false;
            player.GetComponent<PlayerID>().winner = false;
            player.GetComponent<PlayerClick>().playingAnim = false;
            player.GetComponent<PlayerClick>().playingSquareAnim = false;
            player.GetComponent<PlayerID>().playerScore = 0;
            player.GetComponent<PlayerID>().playerTurnOrder = 0;
            player.GetComponent<PlayerID>().showWinner = true;
            GameObject.Find(player.GetComponent<PlayerID>().playersPanel).GetComponent<Image>().color = greyedPanel;
        }
        ParticleSystem.MainModule settings = GameObject.Find("EventPanelEffect").GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(Color.cyan);
        GetComponent<TurnTimer>().timer = GLOBALS.MAXTURNTIME;
        GetComponent<TurnTimer>().enabled = false;
        StopAllCoroutines();
        gameOver = false;
        gameDone = false;
        GetComponent<GameStart>().buildGrid = true;
        GetComponent<GameStart>().startGame = true;
    }
    //Play another game with the same player
    public void PlayAgain()
    {
        photonView.RPC("HideMenu", PhotonTargets.AllBuffered);
    }

}
