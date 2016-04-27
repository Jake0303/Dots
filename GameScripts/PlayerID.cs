using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerID : NetworkBehaviour
{
    [SyncVar]
    public string playerID;
    [SyncVar]
    public bool isPlayersTurn = false;
    [SyncVar]
    public int playerTurnOrder = 0;
    [SyncVar(hook = "OnScoreChanged")]
    public int playerScore = 0;
    private NetworkInstanceId playerNetID;
    private Transform myTransform;
    private GameObject[] names;
    [SyncVar(hook = "OnNameChanged")]
    public bool nameSet = false;
    public GameObject prefabButton,userinputField,panel,infoText;
    [SyncVar (hook = "OnPanelNameChanged")]
    public string playersPanel = "";
    void Start()
    {
        //Setup the enter username panel locally
        if (isLocalPlayer)
        {
            GameObject goPanel = (GameObject)Instantiate(panel);
            goPanel.transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);

            GameObject goText = (GameObject)Instantiate(infoText);
            goText.transform.localScale = new Vector3(5, 3, 3);

            Text tempText = goText.GetComponent<Text>();
            tempText.transform.SetParent(goPanel.transform, false);

            GameObject goInputField = (GameObject)Instantiate(userinputField);
            goInputField.transform.localScale = new Vector3(5, 3, 3);

            GameObject goButton = (GameObject)Instantiate(prefabButton);
            goButton.transform.localScale = new Vector3(5, 3, 3);

            goPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);

            InputField tempField = goInputField.GetComponent<InputField>();
            tempField.transform.SetParent(goPanel.transform, false);

            Button tempButton = goButton.GetComponent<Button>();
            tempButton.transform.SetParent(goPanel.transform, false);

            tempButton.onClick.AddListener(() => this.GetComponent<UIManager>().SetPlayerName(tempField, goPanel));
        }

        myTransform = transform;
        names = GameObject.FindGameObjectsWithTag("NameText");
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
        if(isLocalPlayer)
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
        if(isPlayersTurn && playersPanel != "")
        {
            GameObject.Find(playersPanel).GetComponent<Image>().color = Color.green;
        }
        else if (!isPlayersTurn && playersPanel != "")
        {
            GameObject.Find(playersPanel).GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f,0.2f);
        }
    }

    [Client]
    void GetNetIdentity()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        //CmdTellServerMyName(MakeUniqueIdentity());
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
