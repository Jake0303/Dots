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
        foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
        {
            if (scores.name.Contains(playerTurnOrder.ToString()))
            {
                GetComponent<UIManager>().UpdateUI(scores.GetComponent<Text>(),
                   playerScore.ToString(), gameObject);
                break;
            }
        }
        CmdTellServerMyScore(playerScore);
    }
    [Command]
    void CmdTellServerMyScore(int score)
    {
        playerScore = score;
        foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
        {
            if (scores.name.Contains(playerTurnOrder.ToString()))
            {
                GetComponent<UIManager>().UpdateUI(scores.GetComponent<Text>(),
                   playerScore.ToString(), gameObject);
                break;
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
