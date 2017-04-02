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
    public bool fbInfoFound;
    void Start()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
        fbInfoFound = false;
        if (photonView.isMine)
        {
            if (LeaderbordController.data.list != null)
            {
                foreach (var aData in LeaderbordController.data.list)
                {
                    if (aData["FBUserID"].str == GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken)
                    {
                        fbInfoFound = true;
                        this.GetComponent<UIManager>().fbAuthenticated(aData["Username"].str);
                        StartCoroutine(LeaderbordController.PostScores(this.playerID, playersWins, playerLosses, GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken));
                        break;
                    }
                }
            }
            //Facebook Info not found so player must be logging in as guest and using local account
            if (!fbInfoFound)
            {
                playerID = PlayerPrefs.GetString("Username");
                guestToken = PlayerPrefs.GetString("GuestID");
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                playersWins = PlayerPrefs.GetInt("Wins");
                playerLosses = PlayerPrefs.GetInt("Losses");
                guestToken = PlayerPrefs.GetString("GuestID");
                GetComponent<UIManager>().photonView.RPC("CmdAddPlayer", PhotonTargets.AllBuffered, PlayerPrefs.GetString("Username"));
                PhotonNetwork.player.name = PlayerPrefs.GetString("Username");
                GameObject.Find("PopupText").transform.localScale = new Vector3(1, 1, 1);
                GameObject.Find("LoadingGif").transform.localScale = new Vector3(1, 1, 1);
                GameObject.Find("ColorBlindAssistCheckbox").GetComponent<Toggle>().isOn = GLOBALS.ColorBlindAssist;
                StartCoroutine(LeaderbordController.PostScores(guestToken, playerID, playersWins, playerLosses));
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
            this.GetComponent<UIManager>().DisplayPopupText("Generating grid", false);
        }
        //Game over
        else if (eventcode == 2 && this != null)
        {
            //Play victory sound
            if (photonView.isMine && winner && !GetComponents<AudioSource>()[0].isPlaying)
            {
                GetComponents<AudioSource>()[0].volume = GLOBALS.Volume;
                GetComponents<AudioSource>()[0].Play();
            }
            else if (photonView.isMine && !winner && !GetComponents<AudioSource>()[0].isPlaying)
            {
                //Play defeat sound
                GetComponents<AudioSource>()[1].volume = GLOBALS.Volume;
                GetComponents<AudioSource>()[1].Play();
            }
        }
        PlayerPrefs.SetInt("Wins", playersWins);
        PlayerPrefs.SetInt("Losses", playerLosses);
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
