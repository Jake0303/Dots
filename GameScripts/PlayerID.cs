using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerID : NetworkBehaviour {
	[SyncVar] public string playerID;
	[SyncVar] public bool isPlayersTurn = false;
	[SyncVar] public int playerTurnOrder = 0;
	[SyncVar (hook = "OnScoreChanged")] public int playerScore = 0;
	private NetworkInstanceId playerNetID;
	private Transform myTransform;



	public override void OnStartLocalPlayer()
	{
		GetNetIdentity ();
		SetIdentity ();
	}

	void OnScoreChanged(int score)
	{
		playerScore = score;
		CmdTellServerMyScore (playerScore);
	}
	[Command]
	void CmdTellServerMyScore(int score)
	{
		playerScore = score;
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
		string uniqueName = "Player" + playerNetID.ToString ();
		return uniqueName;
	}
	[Command]
	void CmdTellServerMyName (string name)
	{
		playerID = name;
	}
}
