using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerID : NetworkBehaviour
{
    [SyncVar]
    public string playerID;
    [SyncVar(hook = "OnPlayerTurn")]
    public bool isPlayersTurn = false;
    public bool winner = false;
    [SyncVar]
    public int playerTurnOrder = 0;
    [SyncVar(hook = "OnScoreChanged")]
    public int playerScore = 0;
    private NetworkInstanceId playerNetID;
    private NetworkPlayer networkPlayer;
    private Transform myTransform;
    private GameObject[] names;
    [SyncVar(hook = "OnNameChanged")]
    public bool nameSet = false;
    public GameObject prefabButton, userinputField, panel, infoText, errorText;
    [SyncVar(hook = "OnPanelNameChanged")]
    public string playersPanel = "";

    private Button tempButton;
    private InputField tempField;
    private GameObject goPanel;
    private Text errorMsg;

    private bool showPopup;
    public bool showWinner = true;
    void Start()
    {
        //Setup the enter username panel UI locally
        if (isLocalPlayer)
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
            errorMsg.transform.position = new Vector3(errorMsg.transform.position.x,40,errorMsg.transform.position.z);

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
            tempButton.onClick.AddListener(() => this.GetComponent<UIManager>().SetPlayerName(tempField, goPanel,errorMsg));
        }

        myTransform = transform;
        names = GameObject.FindGameObjectsWithTag("NameText");
    }
    void OnPlayerTurn(bool turn)
    {
        isPlayersTurn = turn;
        if (isPlayersTurn && !GameObject.Find("GameManager").GetComponent<GameOver>().gameOver)
            showPopup = true;
        else if (GameObject.Find("GameManager").GetComponent<GameOver>().gameOver)
            showPopup = false;
        else if (!isPlayersTurn)
            this.GetComponent<UIManager>().DisplayPopupText("");

    }
    public void OnNameChanged(bool set)
    {
        nameSet = set;
        if (nameSet)
        {
            transform.name = playerID;
        }
    }
    void OnPanelNameChanged(string name)
    {
        playersPanel = name;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        names = GameObject.FindGameObjectsWithTag("NameText");
        GetNetIdentity();
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
        if (isLocalPlayer)
            CmdTellServerMyScore(playerScore);
    }
    [Command]
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
            GameObject.Find(playersPanel).GetComponent<Image>().color = Color.green;
            if (isLocalPlayer)
            {
                if (showPopup)
                {
                    this.GetComponent<UIManager>().DisplayPopupText("Its your turn, place a line!");
                    showPopup = false;
                }
            }

        }
        else if (!isPlayersTurn && playersPanel != "")
        {
            GameObject.Find(playersPanel).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        }
        //If the use presses enter set the player name and join the game
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (tempButton != null && goPanel.activeSelf)
            {
                this.GetComponent<UIManager>().SetPlayerName(tempField, goPanel,errorMsg);
            }
        }
        //If gameover display the winner
        if(GameObject.Find("GameManager").GetComponent<GameOver>().gameOver)
        {
            if (isLocalPlayer && showWinner)
            {
                if (winner)
                {
                    GetComponent<UIManager>().DisplayPopupText("You have won the game!");
                }
                else
                {
                    GetComponent<UIManager>().DisplayPopupText(GameObject.Find("GameManager").GetComponent<GameOver>().winner + " has won the game!");
                }
                showWinner = false;
            }
        }
        if (tempField != null && isLocalPlayer)
        {
            if (tempField.isFocused)
            {
                errorMsg.text = "";
            }
        }
    }

    [Client]
    void GetNetIdentity()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
    }

    [Client]
    void SetIdentity()
    {
        if (!isLocalPlayer)
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
        string uniqueName = "Player " + playerNetID;
        return uniqueName;
    }
    [Command]
    public void CmdTellServerMyName(string name)
    {
        playerID = name;
        myTransform.name = playerID;
        RpcTellClientsMyName(name);
    }
    [ClientRpc]
    public void RpcTellClientsMyName(string name)
    {
        playerID = name;
        myTransform.name = playerID;
    }

}
