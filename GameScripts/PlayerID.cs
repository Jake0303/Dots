using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using Photon;
using System;

public class PlayerID : PunBehaviour
{
    public string playerID, fbToken, guestToken;
    public bool isPlayersTurn = false;
    public bool winner = false;

    public int playerTurnOrder = 0;
    public int playerScore = 0;
    public int playersWins;
    public int playerLosses;
    private Transform myTransform;
    public bool nameSet = false;
    public GameObject prefabButton, userinputField, panel, infoText, errorText;
    public string playersPanel = "";

    private Button tempButton;
    private InputField tempField;
    private GameObject goPanel;
    private Text errorMsg;

    private bool showPopup;
    public bool showWinner = true;
    public bool infoFound;
    void Start()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
        infoFound = false;
        if (photonView.isMine)
        {
            if (LeaderbordController.data.list != null)
            {
                foreach (var aData in LeaderbordController.data.list)
                {
                    if (aData["FBUserID"].str != ""
                        && aData["FBUserID"].str == GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken)
                    {
                        infoFound = true;
                        playerID = aData["Username"].str;
                        playersWins = Int32.Parse(aData["Wins"].str);
                        playerLosses = Int32.Parse(aData["Losses"].str);
                        fbToken = aData["FBUserID"].str;
                        this.GetComponent<PlayerUIManager>().fbAuthenticated(aData["Username"].str);
                        PhotonNetwork.player.name = playerID;
                        StartCoroutine(LeaderbordController.PostScores(this.playerID, playersWins, playerLosses, GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken, false));
                        break;
                    }
                    else if (aData["GuestID"].str != ""
                        && aData["GuestID"].str == PlayerPrefs.GetString("GuestID")
                        && GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken == "")
                    {
                        infoFound = true;
                        playerID = aData["Username"].str;
                        guestToken = PlayerPrefs.GetString("GuestID");
                        Screen.orientation = ScreenOrientation.LandscapeLeft;
                        playersWins = Int32.Parse(aData["Wins"].str);
                        playerLosses = Int32.Parse(aData["Losses"].str);
                        guestToken = aData["GuestID"].str;
                        GetComponent<PlayerUIManager>().photonView.RPC("CmdAddPlayer", PhotonTargets.AllBuffered, PlayerPrefs.GetString("Username"));
                        PhotonNetwork.player.name = PlayerPrefs.GetString("Username");
                        GameObject.Find("PopupText").transform.localScale = new Vector3(1, 1, 1);
                        GameObject.Find("LoadingGif").transform.localScale = new Vector3(1, 1, 1);
                        GameObject.Find("ColorBlindAssistCheckbox").GetComponent<Toggle>().isOn = GLOBALS.ColorBlindAssist;
                        StartCoroutine(LeaderbordController.PostScores(guestToken, playerID, playersWins, playerLosses, false));
                        break;
                    }
                }
            }
            //Facebook Info not found so player must be logging in as guest and using local account

            if (!infoFound)
            {
                playerID = PlayerPrefs.GetString("Username");
                guestToken = PlayerPrefs.GetString("GuestID");
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                playersWins = PlayerPrefs.GetInt("Wins");
                playerLosses = PlayerPrefs.GetInt("Losses");
                guestToken = PlayerPrefs.GetString("GuestID");
                GetComponent<PlayerUIManager>().photonView.RPC("CmdAddPlayer", PhotonTargets.AllBuffered, PlayerPrefs.GetString("Username"));
                PhotonNetwork.player.name = PlayerPrefs.GetString("Username");
                GameObject.Find("PopupText").transform.localScale = new Vector3(1, 1, 1);
                GameObject.Find("LoadingGif").transform.localScale = new Vector3(1, 1, 1);
                GameObject.Find("ColorBlindAssistCheckbox").GetComponent<Toggle>().isOn = GLOBALS.ColorBlindAssist;
                StartCoroutine(LeaderbordController.PostScores(guestToken, playerID, playersWins, playerLosses, false));
            }
        }
        myTransform = transform;
    }
    private void OnEvent(byte eventcode, object content, int senderid)
    {
        //Building grid
        if (eventcode == 1 && this != null)
        {
            GameObject.Find("LoadingGif").transform.localScale = new Vector3(0, 0, 0);
            this.GetComponent<PlayerUIManager>().DisplayPopupText("Generating grid", false);
        }
        //Game over
        else if (eventcode == 2 && this != null)
        {
            //Play victory sound
            if (photonView.isMine && winner)
            {
                if (GameObject.Find("PopupText").GetComponent<Text>().text != "You have won the game!")
                    playersWins++;
                PlayerPrefs.SetInt("Wins", playersWins);
                GetComponent<PlayerUIManager>().DisplayPopupText("You have won the game!", true);
                GameObject.Find(playersPanel).GetComponentsInChildren<Text>()[3].text = playersWins + " W " + playerLosses + " L ";
                GetComponents<AudioSource>()[0].volume = GLOBALS.Volume;
                GetComponents<AudioSource>()[0].Play();
                if (GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken != "")
                    StartCoroutine(LeaderbordController.PostScores(playerID, GetComponent<PlayerID>().playersWins, GetComponent<PlayerID>().playerLosses, GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken, false));
                else
                    StartCoroutine(LeaderbordController.PostScores(guestToken, playerID, playersWins, playerLosses, false));
                GameObject.Find("GameManager").GetComponent<GameOver>().StartCoroutine("DelayBeforeRestart");

            }
            //Play defeat sound
            else if (photonView.isMine && !winner)
            {
                if (GameObject.Find("PopupText").GetComponent<Text>().text != GameObject.Find("GameManager").GetComponent<GameOver>().winner + " has won the game!")
                    playerLosses++;
                PlayerPrefs.SetInt("Losses", playerLosses);
                GetComponent<PlayerUIManager>().DisplayPopupText(GameObject.Find("GameManager").GetComponent<GameOver>().winner + " has won the game!", true);
                GameObject.Find(playersPanel).GetComponentsInChildren<Text>()[3].text = playersWins + " W " + playerLosses + " L ";
                GetComponents<AudioSource>()[1].volume = GLOBALS.Volume;
                GetComponents<AudioSource>()[1].Play();
                if (GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken != "")
                    StartCoroutine(LeaderbordController.PostScores(playerID, playersWins, playerLosses, GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken, false));
                else
                    StartCoroutine(LeaderbordController.PostScores(guestToken, playerID, playersWins, playerLosses, false));
                GameObject.Find("GameManager").GetComponent<GameOver>().StartCoroutine("DelayBeforeRestart");
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            //Syncing player turn
            stream.SendNext(isPlayersTurn);
            //Syncing if a player name is set
            stream.SendNext(nameSet);
            if (nameSet)
            {
                transform.name = playerID;
            }
            //Syncing players UI panel
            stream.SendNext(playerID);
            stream.SendNext(playerScore);
            stream.SendNext(winner);
            stream.SendNext(playersWins);
            stream.SendNext(playerLosses);
        }
        else
        {
            // Network player, receive data
            this.isPlayersTurn = (bool)stream.ReceiveNext();
            this.nameSet = (bool)stream.ReceiveNext();
            this.playerID = (string)stream.ReceiveNext();
            this.playerScore = (int)stream.ReceiveNext();
            this.winner = (bool)stream.ReceiveNext();
            this.playersWins = (int)stream.ReceiveNext();
            this.playerLosses = (int)stream.ReceiveNext();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SetIdentity();
    }
    [PunRPC]
    void CmdTellServerMyScore(int score)
    {
        playerScore = score;
        for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
        {
            if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == GetComponent<PlayerID>().playerID)
            {
                foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
                {
                    if (scores.name.Contains((i + 1).ToString()))
                    {
                        //Update UI with score
                        scores.GetComponent<Text>().text = "Score: " + score.ToString();
                        return;
                    }
                }
            }
        }
    }

    // Use this for initialization
    void Awake()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!nameSet)
        {
            SetIdentity();
        }
    }



    void SetIdentity()
    {
        if (!photonView.isMine)
        {
            myTransform.name = playerID;
        }
        else
        {
            myTransform.name = MakeUniqueIdentity();
        }
    }

    string MakeUniqueIdentity()
    {
        string uniqueName = "Player " + PhotonNetwork.player.ID;
        return uniqueName;
    }
    [PunRPC]
    public void CmdTellServerMyName(string name)
    {
        playerID = name;
        myTransform.name = playerID;
    }
    [PunRPC]
    public void RpcTellClientsMyName(string name)
    {
        playerID = name;
        myTransform.name = playerID;
    }

}
