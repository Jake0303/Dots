using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using Photon;


public class PlayerID : PunBehaviour
{
    public string playerID;
    public bool isPlayersTurn = false;
    public bool winner = false;
    
    public int playerTurnOrder = 0;
    public int playerScore = 0;
    private Transform myTransform;
    private GameObject[] names;
    public bool nameSet = false;
    public GameObject prefabButton, userinputField, panel, infoText, errorText;
    public string playersPanel = "";

    private Button tempButton;
    private InputField tempField;
    private GameObject goPanel;
    private Text errorMsg;

    private bool showPopup;
    public bool showWinner = true;
    void Start()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
        //Setup the enter username panel UI locally
        if (photonView.isMine)
        {
            goPanel = (GameObject)Instantiate(panel);
            goPanel.transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);

            GameObject goText = (GameObject)Instantiate(infoText);
            goText.transform.localScale = new Vector3(5, 3, 3);

            GameObject errorTxt = (GameObject)Instantiate(errorText);
            errorTxt.transform.localScale = new Vector3(5, 3, 3);

            Text tempText = goText.GetComponent<Text>();
            tempText.transform.SetParent(goPanel.transform, false);

            errorMsg = errorTxt.GetComponent<Text>();
            errorMsg.transform.SetParent(goPanel.transform, false);
            errorMsg.transform.position = new Vector3(errorMsg.transform.position.x, 40, errorMsg.transform.position.z);

            GameObject goInputField = (GameObject)Instantiate(userinputField);
            goInputField.transform.localScale = new Vector3(5, 3, 3);

            GameObject goButton = (GameObject)Instantiate(prefabButton);
            goButton.transform.localScale = new Vector3(5, 3, 3);

            goPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);

            tempField = goInputField.GetComponent<InputField>();
            tempField.transform.SetParent(goPanel.transform, false);
            tempField.characterLimit = 12;
            tempField.characterValidation = InputField.CharacterValidation.Alphanumeric;
            tempField.ActivateInputField();

            tempButton = goButton.GetComponent<Button>();
            tempButton.transform.SetParent(goPanel.transform, false);
            tempButton.onClick.AddListener(() => this.GetComponent<UIManager>().SetPlayerName(tempField, goPanel, errorMsg));
        }

        myTransform = transform;
        names = GameObject.FindGameObjectsWithTag("NameText");
    }
    private void OnEvent(byte eventcode, object content, int senderid)
    {
        //If a turn has changed
        if (eventcode == 0)
        {
            if (isPlayersTurn && !GameObject.Find("GameManager").GetComponent<GameOver>().gameOver)
                showPopup = true;
            else if (GameObject.Find("GameManager").GetComponent<GameOver>().gameOver)
                showPopup = false;
            else if (!isPlayersTurn)
                this.GetComponent<UIManager>().DisplayPopupText("Waiting for opponent to make a move", false);
        }
        //Building grid
        else if(eventcode == 1)
        {
            this.GetComponent<UIManager>().DisplayPopupText("Generating grid", false);
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
            stream.SendNext(playersPanel);
            for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
            {
                if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == GetComponent<PlayerID>().playerID)
                {
                    foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
                    {
                        if (scores.name.Contains((i + 1).ToString()))
                        {
                            //Update UI with score
                            scores.GetComponent<Text>().text = playerScore.ToString();
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            // Network player, receive data
            this.isPlayersTurn = (bool)stream.ReceiveNext();
            this.nameSet = (bool)stream.ReceiveNext();
            this.playersPanel = (string)stream.ReceiveNext();
        }
    }

    public override void OnJoinedRoom()
    {
 	    base.OnJoinedRoom();
        names = GameObject.FindGameObjectsWithTag("NameText");
        SetIdentity();
    }
    void OnScoreChanged(int score)
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
                        scores.GetComponent<Text>().text = score.ToString();
                        return;
                    }
                }
            }
        }
        if (photonView.isMine)
            photonView.RPC("CmdTellServerMyScore", PhotonTargets.AllBuffered, playerScore);
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
                        scores.GetComponent<Text>().text = score.ToString();
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
        //Update panel to green if its the players turn
        if (isPlayersTurn && playersPanel != "")
        {
            GameObject.Find(playersPanel).GetComponent<Image>().color = GetComponent<PlayerColor>().playerColor;
            if (photonView.isMine)
            {
                if (showPopup)
                {
                    this.GetComponent<UIManager>().DisplayPopupText("Its your turn, place a line!", true);
                    showPopup = false;
                }
            }

        }
        else if (!isPlayersTurn && playersPanel != "")
        {
            GameObject.Find(playersPanel).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        }
        //If the use presses enter set the player name and join the game
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (tempButton != null && goPanel.activeSelf)
            {
                this.GetComponent<UIManager>().SetPlayerName(tempField, goPanel, errorMsg);
            }
        }
        //If gameover display the winner
        if (GameObject.Find("GameManager").GetComponent<GameOver>() != null && 
            GameObject.Find("GameManager").GetComponent<GameOver>().gameOver)
        {
            if (photonView.isMine && showWinner)
            {
                if (winner)
                {
                    GetComponent<UIManager>().DisplayPopupText("You have won the game!", true);
                }
                else
                {
                    GetComponent<UIManager>().DisplayPopupText(GameObject.Find("GameManager").GetComponent<GameOver>().winner + " has won the game!", true);
                }
                showWinner = false;
            }
        }
        if (tempField != null && photonView.isMine)
        {
            if (tempField.isFocused)
            {
                errorMsg.text = "";
            }
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
        //RpcTellClientsMyName(name);
    }
    [PunRPC]
    public void RpcTellClientsMyName(string name)
    {
        playerID = name;
        myTransform.name = playerID;
    }

}
