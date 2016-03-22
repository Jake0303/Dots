using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class GameStart : NetworkBehaviour {

	[SerializeField] public GameObject dots, lineHor,lineVert;

	//GridWidth
	public static int gridWidth  = 6;
	//GridHeight
	public static int gridHeight  = 6;
	//The distance between each dot
	public static float dotDistance = 11.0f;
	//The speed at which each dot in the grid spawns
	[SyncVar]public float spawnSpeed = 0.1f;
    [SyncVar]
    bool buildGrid = false;
	//Sync the rotation and scale,Network.Spawn only sync position
	[SyncVar (hook = "OnRotChanged")]
	public Quaternion lineRot;
	[SyncVar (hook = "OnVertScaleChanged")]
	public Vector3 lineVertScale;
	[SyncVar (hook = "OnHorScaleChanged")]
	public Vector3 lineHorScale;
	public override void OnStartServer()
	{
		buildGrid = true;
	}

	void OnVertScaleChanged(Vector3 scale)
	{
		lineVertScale = scale;
		lineVert.transform.localScale = lineVertScale;
	}
	void OnHorScaleChanged(Vector3 scale)
	{
		lineHorScale = scale;
		lineHor.transform.localScale = lineHorScale;
	}

	void OnRotChanged(Quaternion rot)
	{
		//lineRot = rot;
		//lines.transform.localRotation = lineRot;
	}
    void Update()
    {
			if(NetworkServer.connections.Count > 1 && buildGrid)
			//if(buildGrid)//This if statement is for testing
        {
            //Hide temporary lines
			lineHor.GetComponent<Renderer>().enabled = false;
			lineVert.GetComponent<Renderer>().enabled = false;
			StartCoroutine(CreateGrid());
			buildGrid = false;
        }

    }
	//Tell the server that we spawned a line or dot
	[Command]
	void CmdSpawnObj(GameObject obj)
	{
			NetworkServer.Spawn (obj);
	}
	//Build the grid
	IEnumerator CreateGrid () {
		for(int x = 0; x < gridWidth; x++) {
			yield return new WaitForSeconds(spawnSpeed);
			
			for(int z = 0; z < gridHeight; z++) {                
				yield return new WaitForSeconds(spawnSpeed);
				//Spawn dot
				dots = Instantiate(dots, Vector3.zero, dots.transform.rotation) as GameObject;
				dots.transform.localPosition = new Vector3(x*dotDistance, 0, z*dotDistance);
				dots.transform.localScale = new Vector3(3,3,3);
				dots.name = "Dot "+x.ToString()+","+z.ToString();
				dots.GetComponent<DotID>().dotID = dots.name;
				CmdSpawnObj (dots);
				//This if statement stops from building extra unnecessary lines
				if(z < gridHeight-1)
				{
					//Spawn line in between dots horizontally
					lineHor = Instantiate(lineHor, Vector3.zero, lineHor.transform.rotation) as GameObject;
					lineHor.transform.localPosition = new Vector3(x*dotDistance, 0, dots.transform.localPosition.z+(dotDistance/2.0f));
					lineHorScale = new Vector3(3,3,dotDistance-dots.transform.localScale.z+0.5f);
					//lineRot = lines.transform.rotation;
					lineHor.name = "linesHorizontal "+x.ToString()+","+z.ToString();
					lineHor.GetComponent<LineID>().lineID = lineHor.name;
					lineHor.transform.localScale = lineHorScale;
					//lines.transform.rotation = lineRot;
					CmdSpawnObj (lineHor);
				}
				if(x < gridWidth-1)
				{
					//Spawn line in between dots vertically
					lineVert = Instantiate(lineVert, Vector3.zero, lineVert.transform.rotation) as GameObject;
					lineVert.transform.localPosition = new Vector3(dots.transform.localPosition.x+(dotDistance/2.0f), 0, z*dotDistance);
					lineVertScale = new Vector3(dotDistance-dots.transform.localScale.z+0.5f,3,3);
					//lineRot = lines.transform.rotation;
					lineVert.name = "linesVertical "+x.ToString()+","+z.ToString();
					lineVert.GetComponent<LineID>().lineID = lineVert.name;
					lineVert.transform.localScale = lineVertScale;
					//lines.transform.rotation = lineRot;
					CmdSpawnObj (lineVert);
				}
			}
		}
		//Start the timer after the grid has been built
		//gameObject.GetComponent<PlayerTurn>().enabled = true;
		//Assign each player a random turn order
		var players = GameObject.FindGameObjectsWithTag("Player");
		var rnd = new System.Random();
		var randomNumbers = Enumerable.Range(1,4).OrderBy(x => rnd.Next()).Take(4).ToArray();
		int i = 0;
		foreach (var player in players) {
				player.GetComponent<PlayerID> ().playerTurnOrder = GameObject.Find ("GameManager").GetComponent<PlayerTurn> ().assortPlayerTurns[randomNumbers.ElementAt(i)];
			//Set the first players turn
			if (player.GetComponent<PlayerID> ().playerTurnOrder == 1) {
				player.GetComponent<PlayerID> ().isPlayersTurn = true;
				CmdSetFirstTurn (player.GetComponent<NetworkIdentity> ());
			} else {
				player.GetComponent<PlayerID> ().isPlayersTurn = false;
				CmdDisableTurn (player.GetComponent<NetworkIdentity>());
			}
			//Only 4 people per game
			if (i != NetworkServer.connections.Count)
				i++;
		}
		gameObject.GetComponent<TurnTimer>().enabled = true;
		CmdEnableTimer ();

	}
	//Tell the server that this player turn is disabled
	[Command]
	void CmdDisableTurn(NetworkIdentity playerID)
	{
		playerID.GetComponent<PlayerID> ().isPlayersTurn = false;
		RpcDisableTurn (playerID);
	}
	//Tell all clients who turn it is not
	[ClientRpc]
	void RpcDisableTurn(NetworkIdentity playerID)
	{
		playerID.GetComponent<PlayerID> ().isPlayersTurn = false;
	}
	//Tell the server that the first player turn is up
	[Command]
	void CmdSetFirstTurn(NetworkIdentity playerID)
	{
		playerID.GetComponent<PlayerID> ().isPlayersTurn = true;
		RpcSetFirstTurn (playerID);
	}
	//Tell all clients who turn it is
	[ClientRpc]
	void RpcSetFirstTurn(NetworkIdentity playerID)
	{
		playerID.GetComponent<PlayerID> ().isPlayersTurn = true;
	}
	//Tell the server the timer has started
	[Command]
	void CmdEnableTimer()
	{
		gameObject.GetComponent<PlayerTurn>().enabled = true;
		gameObject.GetComponent<TurnTimer>().enabled = true;
		RpcEnableTimer ();
	}
	//Replicate to all clients
	[ClientRpc]
	void RpcEnableTimer()
	{
		gameObject.GetComponent<PlayerTurn>().enabled = true;
		gameObject.GetComponent<TurnTimer>().enabled = true;
	}
}
