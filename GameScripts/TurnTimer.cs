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
    private GameObject tapGif;


    // Use this for initialization
    void Start()
    {
        tapGif = GameObject.Find("TapGif");
        timerTexts = GameObject.FindGameObjectsWithTag("TimerText");
        timerTexts[0].GetComponent<AudioSource>().volume = GLOBALS.Volume / 50;
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
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (timer <= 5 && !timerTexts[0].GetComponent<AudioSource>().isPlaying)
            timerTexts[0].GetComponent<AudioSource>().Play();
        //Update timer
        foreach (var player in players)
        {
            if (player.GetComponent<PlayerID>().isPlayersTurn)
            {
                if (!player.GetComponent<PlayerClick>().playingAnim
                    && !player.GetComponent<PlayerClick>().playingSquareAnim
                    && !player.GetComponent<PlayerID>().winner)
                {
                    timer -= Time.deltaTime;
                    if (timer < 18)
                        GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Hide(false);
                }
                else
                {
                    timer = GLOBALS.MAXTURNTIME;
                }
                for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
                {
                    if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == player.GetComponent<PlayerID>().playerID)
                    {
                        foreach (var timerText in timerTexts)
                        {
                            if (timerText.name.Contains((i + 1).ToString())
                                && player.GetComponent<PlayerID>().isPlayersTurn)
                            {
                                //Update UI with the time left
                                timerText.GetComponent<Text>().text = "Time left: " + Mathf.Round(timer);
                                if (timer <= 5)
                                {
                                    timerText.GetComponent<Text>().color = Color.red;
                                }
                                else
                                    timerText.GetComponent<Text>().color = Color.white;

                                //If the player has run out of time and not placed a line place a random one
                                if (timer < 1
                                    && !player.GetComponent<PlayerClick>().playingAnim
                                    && !player.GetComponent<PlayerClick>().playingSquareAnim
                                    && !player.GetComponent<PlayerID>().winner
                                    && player.GetComponent<PlayerClick>().photonView.isMine)
                                {
                                    foreach (var randomLine in GameObject.FindGameObjectsWithTag("Line"))
                                    {
                                        if (randomLine.GetComponent<LinePlaced>() != null
                                            && !randomLine.GetComponent<LinePlaced>().linePlaced
                                            && randomLine.name.Contains("line"))
                                        {
                                            player.GetComponent<PlayerClick>().objectID = randomLine.name;
                                            player.GetComponent<PlayerClick>().hit = new RaycastHit();
                                            //ray shooting out of the camera from where the mouse is
                                            Ray rayToCameraPos = new Ray(Camera.main.transform.position, Camera.main.transform.position - randomLine.transform.position);
                                            player.GetComponent<PlayerClick>().ray = rayToCameraPos;
                                            Physics.Raycast(Camera.main.transform.position, randomLine.transform.position - Camera.main.transform.position, out player.GetComponent<PlayerClick>().hit, 5000);
                                            randomLine.GetComponentInChildren<Renderer>().enabled = false;
                                            player.GetComponent<PlayerClick>().objectColor = player.GetComponent<PlayerColor>().playerColor;
                                            randomLine.GetComponents<AudioSource>()[1].volume = GLOBALS.Volume / 100;
                                            randomLine.GetComponents<AudioSource>()[1].Play();
                                            player.GetComponent<PlayerClick>().photonView.RPC("CmdSelectObject", PhotonTargets.AllBuffered, randomLine.name);
                                            player.GetComponent<PlayerClick>().photonView.RPC("CmdPlayAnim", PhotonTargets.AllBuffered, randomLine.name);
                                            break;
                                        }
                                    }
                                }
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
                if (line.GetComponentInChildren<Renderer>().enabled == false)
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
        if (GameObject.Find(nextPlayer).GetComponent<PlayerID>().firstTurn)
        {
            if (Application.isMobilePlatform)
            {
                GameObject.Find(nextPlayer).GetComponent<PlayerUIManager>().DisplayPopupBox("It's your turn first, tap to place a line!");
                GameObject.Find(nextPlayer).GetComponent<PlayerUIManager>().DisplayPopupText("It's your turn , tap to place a line!", false);
            }
            else
            {
                GameObject.Find(nextPlayer).GetComponent<PlayerUIManager>().DisplayPopupBox("It's your turn first, click to place a line!");
                GameObject.Find(nextPlayer).GetComponent<PlayerUIManager>().DisplayPopupText("It's your turn , click to place a line!", false);
            }
            GameObject.Find("TapGif").GetComponent<LoadingGif>().StopAllCoroutines();
            StartCoroutine(GameObject.Find("TapGif").GetComponent<LoadingGif>().playGif());
            tapGif.GetComponent<Image>().enabled = true;
            tapGif.GetComponent<LoadingGif>().enabled = true;
        }

        GameObject.Find(GameObject.Find(nextPlayer).GetComponent<PlayerID>().playersPanel)
            .GetComponent<Image>().color = GameObject.Find(nextPlayer).GetComponent<PlayerColor>().playerColor;
        if (Application.isMobilePlatform)
            GameObject.Find(nextPlayer).GetComponent<PlayerUIManager>().DisplayPopupText("It's your turn, tap to place a line!", true);
        else
            GameObject.Find(nextPlayer).GetComponent<PlayerUIManager>().DisplayPopupText("It's your turn, click to place a line!", true);
        GameObject.Find(nextPlayer).GetComponent<PlayerClick>().playingAnim = false;
        GameObject.Find(GameObject.Find(lastPlayer).GetComponent<PlayerID>().playersPanel).GetComponent<Image>().color = greyedPanel;
        GameObject.Find(lastPlayer).GetComponent<PlayerUIManager>().DisplayPopupText("Waiting for opponent to make a move", false);
        GameObject.Find(lastPlayer).GetComponent<PlayerID>().isPlayersTurn = false;
        GameObject.Find(lastPlayer).GetComponent<PlayerID>().firstTurn = false;
    }
    //Reset the turn timer
    public void ResetTimer()
    {
        if (!isGameOver)
        {
            timer = GLOBALS.MAXTURNTIME;
            nextTurn = false;
        }
        if (isGameOver)
        {
            GameObject.Find("GameManager").GetComponent<GameOver>().gameOver = isGameOver;
        }
    }
}
