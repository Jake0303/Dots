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
        GameObject.Find("LetsPlayButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("LetsPlayButton").GetComponent<Button>().onClick.
            AddListener(() => GetComponent<UIManager>().SetPlayerName());
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
