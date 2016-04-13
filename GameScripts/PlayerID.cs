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


    void Start()
    {
        names = GameObject.FindGameObjectsWithTag("NameText");
    }

    void OnNameChanged(bool set)
    {
        nameSet = set;
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
                GameObject.Find("GameManager").GetComponent<UIManager>().UpdateUI(scores.GetComponent<Text>(),
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
                GameObject.Find("GameManager").GetComponent<UIManager>().UpdateUI(scores.GetComponent<Text>(),
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
    public void CmdTellServerMyName(string name, GameObject namePanel)
    {
        if (!nameSet && !namePanel.GetComponent<NamePanel>().set)
        {
            playerID = name;
            myTransform.name = playerID;
            GameObject.Find("GameManager").GetComponent<UIManager>().UpdateUI(namePanel.GetComponent<Text>(),
               playerID, gameObject);
            nameSet = true;
            namePanel.GetComponent<NamePanel>().set = true;
            RpcTellClientsMyName(name, namePanel);
        }
    }
    [ClientRpc]
    public void RpcTellClientsMyName(string name, GameObject namePanel)
    {
        playerID = name;
        myTransform.name = playerID;
        GameObject.Find("GameManager").GetComponent<UIManager>().UpdateUI(namePanel.GetComponent<Text>(),
           playerID, gameObject);
        nameSet = true;
        namePanel.GetComponent<NamePanel>().set = true;
    }
}
