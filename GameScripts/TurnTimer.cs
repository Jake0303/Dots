using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon;


public class TurnTimer : PunBehaviour
{
    public float timer = GLOBALS.MAXTURNTIME;
    public bool nextTurn = false;
    private bool isGameOver;
    private GameObject[] timerTexts;
    private Color greyedPanel = new Color(0.5f, 0.5f, 0.5f, 0.6f);

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
            stream.SendNext(timer);
        }
        else
        {
            // Network player, receive data
            this.timer = (float)stream.ReceiveNext();
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
                                        photonView.RPC("RpcChangePlayerTurn", PhotonTargets.AllBuffered, nextPlayer.name, player.name);
                                    //if we found the next player, exit the for loop
                                    break;
                                }
                            }
                            //Else, just increase by 1 to the next turn
                            else
                            {
                                if (nextPlayer.GetComponent<PlayerID>().playerTurnOrder - player.GetComponent<PlayerID>().playerTurnOrder == 1)
                                {
                                    if (photonView.isMine)
                                        photonView.RPC("RpcChangePlayerTurn", PhotonTargets.AllBuffered, nextPlayer.name, player.name);
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
    //Tell all clients who turn it is	
    [PunRPC]
    void RpcChangePlayerTurn(string nextPlayer, string lastPlayer)
    {
        PhotonNetwork.RaiseEvent(0, null, true, null);
        GameObject.Find(nextPlayer).GetComponent<PlayerID>().isPlayersTurn = true;
        GameObject.Find(GameObject.Find(nextPlayer).GetComponent<PlayerID>().playersPanel)
            .GetComponent<Image>().color = GameObject.Find(nextPlayer).GetComponent<PlayerColor>().playerColor;
        GameObject.Find(nextPlayer).GetComponent<UIManager>().DisplayPopupText("Its your turn, place a line!", true);

        GameObject.Find(lastPlayer).GetComponent<PlayerID>().isPlayersTurn = false;
        GameObject.Find(GameObject.Find(lastPlayer).GetComponent<PlayerID>().playersPanel).GetComponent<Image>().color = greyedPanel;
        GameObject.Find(lastPlayer).GetComponent<UIManager>().DisplayPopupText("Waiting for opponent to make a move", false);
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
