﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerClick:NetworkBehaviour{
	[SerializeField]public Material hoverMat;
	//Material for the line placed
	[SerializeField] public Material lineMat;
	//Radius of the square hitbox
	[SyncVar] private float squareRadius = GameStart.dotDistance / 2;
    [SyncVar]
    private Color objectColor;
    [SyncVar]
    private GameObject objectID;
    private NetworkIdentity objNetId;
    private RaycastHit hit;
	[SerializeField]
	private Collider[] hitCollidersRight;
	[SerializeField]
	private Collider[] hitCollidersLeft;
	[SerializeField]
	private Collider[] hitCollidersTop;
	[SerializeField]
	private Collider[] hitCollidersBottom;

	void Start () {}
    //Check if a player placed a line and update on clients if so


	[ClientRpc]
    void RpcPaint(GameObject obj, Color col)
    {
        obj.GetComponent<Renderer>().enabled = true;
        obj.GetComponent<Renderer>().material = lineMat;
        obj.GetComponent<Renderer>().material.color = col;      // this is the line that actually makes the change in color happen
		obj.GetComponent<LinePlaced>().linePlaced = true;
		GameObject.Find ("GameManager").GetComponent<TurnTimer> ().nextTurn = true;
	}

	//On click place a line at the mouse location
    [Command]
    void CmdPlaceLine(GameObject obj, Color col)
    {
        objNetId = obj.GetComponent<NetworkIdentity>();
        obj.GetComponent<Renderer>().enabled = true;// get the object's network ID
        obj.GetComponent<Renderer>().material = lineMat;
        obj.GetComponent<Renderer>().material.color = col;
		obj.GetComponent<LinePlaced>().linePlaced = true;
		if(objNetId.clientAuthorityOwner == null)
        	objNetId.AssignClientAuthority(connectionToClient);
        RpcPaint(obj, col);// use a Client RPC function to "paint" the object on all clients
	}
	//Tell the server the current players score
	[Command]
	void CmdTellServerYourScore(int score)
	{
		GetComponent<PlayerID> ().playerScore = score;
		RpcUpdateScore (gameObject);
	}
	//Tell all the clients their current score
	[ClientRpc]
	void RpcUpdateScore(GameObject player)
	{
		var players = GameObject.FindGameObjectsWithTag ("Player");
		foreach(var aPlayer in players)
		{
			if (aPlayer == player) 
				player.GetComponent<PlayerID> ().playerScore = aPlayer.GetComponent<PlayerID> ().playerScore;
			
		}
	}

	// Update is called once per frame
    void Update()
    {
		//Must be the players turn to place a line
		if (GetComponent<PlayerID> ().isPlayersTurn) {
			//Check if a player is clicking, if hovering over a line place one
			if (Input.GetMouseButtonDown (0)) {
				if (isLocalPlayer) {
					//empty RaycastHit object which raycast puts the hit details into
					RaycastHit hit = new RaycastHit ();
					//ray shooting out of the camera from where the mouse is
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

					//Raycast from the mouse to the level, if hit place a line
					if (Physics.Raycast (ray, out hit)) {
						if (hit.collider.name.Contains ("line") && hit.collider.GetComponent<LinePlaced> ().linePlaced == false) {
							objectID = GameObject.Find (hit.collider.name);// this gets the object that is hit
							objectColor = GetComponent<PlayerColor> ().playerColor;
							objectID.GetComponent<LinePlaced> ().linePlaced = true;
							CmdPlaceLine (objectID, objectColor);


							//Check if square is made
							//Check horizontal line hitboxes
							if (hit.collider.name.Contains ("linesHorizontal") && hit.collider.GetComponent<Renderer> ().enabled) {
								Vector3 centerOfSquareRight = new Vector3 (hit.collider.transform.localPosition.x + (GameStart.dotDistance / 2), hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z);
								hitCollidersRight = Physics.OverlapSphere (centerOfSquareRight, squareRadius);
								int i = 0;
								int howManyLines = 0;
								//In the hitbox, check how many lines

								while (i < hitCollidersRight.Length) {
									if (hitCollidersRight [i].name.Contains ("lines") && hitCollidersRight [i].GetComponent<Renderer> ().enabled
									   || hitCollidersRight [i].name.Contains ("square")) {
										hitCollidersRight [i].GetComponent<LineID> ().lineID = "square " + hit.collider.transform.localPosition;
										howManyLines++;
									}
									i++;
								}

								//If there is a 4 lines in the hitbox a square is made and the player gets a point.

								if (howManyLines == 4) {
									GetComponent<PlayerID> ().playerScore+=13;
									CmdTellServerYourScore (GetComponent<PlayerID> ().playerScore);
									GameObject.Find ("GameManager").GetComponent<ScoreManager> ().UpdateScore (gameObject);
									hit.collider.GetComponent<LineID> ().lineID = "square " + hit.collider.transform.localPosition;
								}

								//Left hitbox
								Vector3 centerOfSquareLeft = new Vector3 (hit.collider.transform.localPosition.x - (GameStart.dotDistance / 2), hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z);
								hitCollidersLeft = Physics.OverlapSphere (centerOfSquareLeft, squareRadius);
								int j = 0;
								howManyLines = 0;
								while (j < hitCollidersLeft.Length) {
									if (hitCollidersLeft [j].name.Contains ("lines") && hitCollidersLeft [j].GetComponent<Renderer> ().enabled
									   || hitCollidersLeft [j].name.Contains ("square")) {
										hitCollidersLeft [j].GetComponent<LineID> ().lineID = "square " + hit.collider.transform.localPosition;
										howManyLines++;
									}
									j++;
								}
								if (howManyLines == 4) {
									GetComponent<PlayerID> ().playerScore+=13;
									CmdTellServerYourScore (GetComponent<PlayerID> ().playerScore);
									GameObject.Find ("GameManager").GetComponent<ScoreManager> ().UpdateScore (gameObject);
									hit.collider.GetComponent<LineID> ().lineID = "square " + hit.collider.transform.localPosition;
								}
							}
							//Same as above just for vertical lines
							if (hit.collider.name.Contains ("linesVertical") && hit.collider.GetComponent<Renderer> ().enabled) {
								Vector3 centerOfSquareBottom = new Vector3 (hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z + (GameStart.dotDistance / 2));
								hitCollidersBottom = Physics.OverlapSphere (centerOfSquareBottom, squareRadius);
								int i = 0;
								int howManyLines = 0;
								while (i < hitCollidersBottom.Length) {
									if (hitCollidersBottom [i].name.Contains ("lines") && hitCollidersBottom [i].GetComponent<Renderer> ().enabled
									   || hitCollidersBottom [i].name.Contains ("square")) {
										hitCollidersBottom [i].GetComponent<LineID> ().lineID = "square " + hit.collider.transform.localPosition;
										howManyLines++;
									}
									i++;
								}
								if (howManyLines == 4) {
									GetComponent<PlayerID> ().playerScore+=13;
									CmdTellServerYourScore (GetComponent<PlayerID> ().playerScore);
									GameObject.Find ("GameManager").GetComponent<ScoreManager> ().UpdateScore (gameObject);
									hit.collider.GetComponent<LineID> ().lineID = "square " + hit.collider.transform.localPosition;
								}
								Vector3 centerOfSquareTop = new Vector3 (hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z - (GameStart.dotDistance / 2));
								hitCollidersTop = Physics.OverlapSphere (centerOfSquareTop, squareRadius);
								int j = 0;
								howManyLines = 0;
								while (j < hitCollidersTop.Length) {
									if (hitCollidersTop [j].name.Contains ("lines") && hitCollidersTop [j].GetComponent<Renderer> ().enabled
									   || hitCollidersTop [j].name.Contains ("square")) {
										hitCollidersTop [j].GetComponent<LineID> ().lineID = "square " + hit.collider.transform.localPosition;
										howManyLines++;
									}
									j++;
								}
								if (howManyLines == 4) {
									GetComponent<PlayerID> ().playerScore+=13;
									CmdTellServerYourScore (GetComponent<PlayerID> ().playerScore);
									GameObject.Find ("GameManager").GetComponent<ScoreManager> ().UpdateScore (gameObject);
									hit.collider.GetComponent<LineID> ().lineID = "square " + hit.collider.transform.localPosition;
								}
							}
						}
					}
				}
			}
		}
	}
}
