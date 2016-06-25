using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon;


public class TurnTimer : PunBehaviour
{
    //
    public float timer = GLOBALS.MAXTURNTIME;
    //
    public bool nextTurn = false;
    private bool isGameOver;
    private GameObject[] timerTexts;
    // Use this for initialization
    void Start()
    {
        timerTexts = GameObject.FindGameObjectsWithTag("TimerText");
    }
    //Algorithm to check the majority points
    int CalculateMajorityPoints()
    {
        return GLOBALS.POINTSTOWIN;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            //Syncing player turn
            stream.SendNext(timer);
            stream.SendNext(nextTurn);
        }
        else
        {
            // Network player, receive data
            this.timer = (float)stream.ReceiveNext();
            this.nextTurn = (bool)stream.ReceiveNext();
        }
    }
    // Update is called once per frame
    void Update()
    {
        StartTimer();
    }
    //Player timer functionality
    public void StartTimer()
    {
        timer -= Time.deltaTime;
        var players = GameObject.FindGameObjectsWithTag("Player");
        //Update timer
        foreach (var player in players)
        {
            if (player.GetComponent<PlayerID>().isPlayersTurn)
            {
                for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
                {
                    if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == player.GetComponent<PlayerID>().playerID)
                    {
                        foreach (var timerText in timerTexts)
                        {
                            if (timerText.name.Contains((i + 1).ToString()))
                            {
                                //Update UI with the time left
                                timerText.GetComponent<Text>().text = "Time left: " + Mathf.Round(timer);
                            }
                            else
                            {
                                timerText.GetComponent<Text>().text = "";
                            }
                        }
                    }
                }
            }
        }
        //After the timer has ended or a player has placed a line, set the next players turn
        if (timer <= 0 || nextTurn)
        {

            //Check if the player has won
            var allLines = GameObject.FindGameObjectsWithTag("Line");
            foreach (var line in allLines)
            {
                if (line.GetComponent<Renderer>().enabled == false)
                {
                    isGameOver = false;
                    break;
                    //All the lines have been placed
                }
                else
                {
                    isGameOver = true;
                }
            }
            //End game if the majority of possible squares are made
            foreach (var player in players)
            {
                if (player.GetComponent<UIManager>().routine != null)
                    //player.GetComponent<UIManager>().StopCoroutine(player.GetComponent<UIManager>().routine);
                if (player.GetComponent<PlayerID>().isPlayersTurn)
                {
                    for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
                    {
                        if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == player.GetComponent<PlayerID>().playerID)
                        {
                            foreach (var timerText in timerTexts)
                            {
                                if (timerText.name.Contains((i + 1).ToString()))
                                {
                                    //Update UI with the time left
                                    timerText.GetComponent<Text>().text = "Time left: " + Mathf.Round(timer);
                                }
                            }
                        }
                    }
                    //GameObject.Find("TimerText").GetComponent<Text>().text = "Time Left: " + Mathf.Round(timer);
                }

                if (player.GetComponent<PlayerID>().playerScore >= CalculateMajorityPoints())
                {
                    isGameOver = true;
                    break;
                }
            }
            foreach (var player in players)
            {
                if (isGameOver)
                {
                    GameObject.Find("GameManager").GetComponent<GameOver>().gameOver = isGameOver;
                    break;
                }
                //The current players turn when the timer has ended
                if (player.GetComponent<PlayerID>().isPlayersTurn)
                {
                    //Loop through all the other players
                    foreach (var nextPlayer in players)
                    {
                        //If their turn is last in the player order, reset to the first position
                        if (!nextPlayer.GetComponent<PlayerID>().isPlayersTurn)
                        {
                            if (player.GetComponent<PlayerID>().playerTurnOrder == GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count)
                            {
                                if (nextPlayer.GetComponent<PlayerID>().playerTurnOrder == 1)
                                {
                                    if (photonView.isMine)
                                        photonView.RPC("CmdChangePlayerTurn", PhotonTargets.AllBuffered,nextPlayer, player);
                                    //This if statement is for the host so this code isnt called twice
                                    else if (PhotonNetwork.isMasterClient)
                                    {
                                        nextPlayer.GetComponent<PlayerID>().isPlayersTurn = true;
                                        //Lastly end the players turn
                                        player.GetComponent<PlayerID>().isPlayersTurn = false;
                                    }
                                    //if we found the next player, exit the for loop
                                    break;
                                }
                                //Else, just increase by 1 to the next turn
                            }
                            else
                            {
                                if (nextPlayer.GetComponent<PlayerID>().playerTurnOrder - player.GetComponent<PlayerID>().playerTurnOrder == 1)
                                {
                                    if (photonView.isMine)
                                        photonView.RPC("CmdChangePlayerTurn", PhotonTargets.AllBuffered,nextPlayer, player);
                                    else if (PhotonNetwork.isMasterClient)
                                    {
                                        nextPlayer.GetComponent<PlayerID>().isPlayersTurn = true;
                                        //Lastly end the players turn
                                        player.GetComponent<PlayerID>().isPlayersTurn = false;
                                    }
                                    //if we found the next player, exit the for loop
                                    break;
                                }
                            }
                        }
                    }
                    //We found the next player, no need to loop through the others
                    break;
                }
            }
            ResetTimer();
        }
    }
    //Tell the server that it's the next player's turn
    [PunRPC]
    public void CmdChangePlayerTurn(GameObject nextPlayer, GameObject lastPlayer)
    {
        nextPlayer.GetComponent<PlayerID>().isPlayersTurn = true;
        lastPlayer.GetComponent<PlayerID>().isPlayersTurn = false;
        //RpcChangePlayerTurn(nextPlayer, lastPlayer);
    }
    //Tell all clients who turn it is	
    [PunRPC]
    void RpcChangePlayerTurn(GameObject nextPlayer, GameObject lastPlayer)
    {
        nextPlayer.GetComponent<PlayerID>().isPlayersTurn = true;
        lastPlayer.GetComponent<PlayerID>().isPlayersTurn = false;
    }
    //Reset the turn timer
    public void ResetTimer()
    {
        if (!isGameOver)
        {
            timer = GLOBALS.MAXTURNTIME;
            nextTurn = false;
            //End game if the majority of possible squares are made
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                if (player.GetComponent<PlayerID>().playerScore >= CalculateMajorityPoints())
                {
                    isGameOver = true;
                    break;
                }
            }
            foreach (var player in players)
            {
                if (isGameOver)
                {
                    GameObject.Find("GameManager").GetComponent<GameOver>().gameOver = isGameOver;
                }
            }
        }
    }
}
