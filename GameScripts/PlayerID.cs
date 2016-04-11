using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerID : NetworkBehaviour {
	[SyncVar] public string playerID;
	[SyncVar] public bool isPlayersTurn = false;
	[SyncVar] public int playerTurnOrder = 0;
	[SyncVar (hook = "OnScoreChanged")] public int playerScore = 0;
	private NetworkInstanceId playerNetID;
	private Transform myTransform;
    void Start()
    {

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        GetNetIdentity();
        SetIdentity();
        var names = GameObject.FindGameObjectsWithTag("NameText");
        foreach (var name in names)
        {
            if (name.GetComponent<Text>().text.Contains("Waiting"))
            {
                GameObject.Find("GameManager").GetComponent<UIManager>().UpdateUI(name.GetComponent<Text>(), 
                    playerID, gameObject);
                break;
            }
        }
    }
	public override void OnStartLocalPlayer()
	{
		
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
		CmdTellServerMyScore (playerScore);
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
	void Awake () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (myTransform.name == "" || myTransform.name == "Player(Clone)") {
			SetIdentity();
		}
	}
	
	[Client]
	void GetNetIdentity()
	{
		playerNetID = GetComponent<NetworkIdentity> ().netId;
		CmdTellServerMyName (MakeUniqueIdentity());
	}
	
	[Client]
	void SetIdentity()
	{
		if (!isLocalPlayer) {
			myTransform.name = playerID;
		} else {
			myTransform.name = MakeUniqueIdentity();
		}
	}
	
	string MakeUniqueIdentity ()
	{
        string uniqueName = PlayerPrefs.GetString(Network.player.ipAddress);
		return uniqueName;
	}
	[Command]
	void CmdTellServerMyName (string name)
	{
		playerID = name;
	}
}
