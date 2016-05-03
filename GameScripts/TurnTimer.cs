using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TurnTimer : NetworkBehaviour {
	[SyncVar] public float timer = MAX_TURN_TIME;
	[SyncVar]public bool nextTurn = false;
	private const float MAX_TURN_TIME = 30.0f;
	private bool isGameOver;
	// Use this for initialization
	void Start () {
	
	}
	//Algorithm to check the majority points
	int CalculateMajorityPoints()
	{
		return ((GameStart.gridWidth - 1) * (GameStart.gridHeight - 1) / 2)+1;
	}

	// Update is called once per frame
	void Update () {
		StartTimer();
	}
	//Player timer functionality
	public void StartTimer()
	{
		timer -= Time.deltaTime;
		GameObject.Find ("TimerText").GetComponent<Text> ().text = "Time Left: " + Mathf.Round (timer);
		//After the timer has ended or a player has placed a line, set the next players turn
		if (timer <= 0 || nextTurn) {

			//Check if the player has won
			var allLines = GameObject.FindGameObjectsWithTag ("Line");
			foreach (var line in allLines) {
				if (line.GetComponent<Renderer> ().enabled == false) {
					isGameOver = false;
					break;
					//All the lines have been placed
				} else {
					isGameOver = true;
				}
			}
			//End game if the majority of possible squares are made
			var players = GameObject.FindGameObjectsWithTag ("Player");
			foreach (var player in players) {
				if (player.GetComponent<PlayerID> ().playerScore >= CalculateMajorityPoints ()) {
					isGameOver = true;
					break;
				}
			}
			foreach (var player in players) {
				if (isGameOver) {
					GameObject.Find ("GameManager").GetComponent<GameOver> ().gameOver = isGameOver;
					break;
				}
					//The current players turn when the timer has ended
					if (player.GetComponent<PlayerID> ().isPlayersTurn) {
						//Loop through all the other players
						foreach (var nextPlayer in players) {
							//If their turn is last in the player order, reset to the first position
							if (!nextPlayer.GetComponent<PlayerID> ().isPlayersTurn) {
                                if (player.GetComponent<PlayerID>().playerTurnOrder == GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count)
                                {
									if (nextPlayer.GetComponent<PlayerID> ().playerTurnOrder == 1) {
									if (isLocalPlayer)
										CmdChangePlayerTurn (nextPlayer, player);
									//This if statement is for the host so this code isnt called twice
									else if (isServer) {
										nextPlayer.GetComponent<PlayerID> ().isPlayersTurn = true;
										//Lastly end the players turn
										player.GetComponent<PlayerID> ().isPlayersTurn = false;
									}
										//if we found the next player, exit the for loop
										break;
									}
									//Else, just increase by 1 to the next turn
								} else {
									if (nextPlayer.GetComponent<PlayerID> ().playerTurnOrder - player.GetComponent<PlayerID> ().playerTurnOrder == 1) {
									if (isLocalPlayer)
										CmdChangePlayerTurn (nextPlayer, player);
									else if (isServer) {	
										nextPlayer.GetComponent<PlayerID> ().isPlayersTurn = true;
										//Lastly end the players turn
										player.GetComponent<PlayerID> ().isPlayersTurn = false;
									}
										//if we found the next player, exit the for loop
										break;
									}
								}
							}
						}
						//We found the next player, no need to loop through the others
						break;
					}
				}
				ResetTimer ();
			}
	}
	//Tell the server that it's the next player's turn
	[Command]
	public void CmdChangePlayerTurn(GameObject nextPlayer,GameObject lastPlayer)
	{
		nextPlayer.GetComponent<PlayerID> ().isPlayersTurn = true;
		lastPlayer.GetComponent<PlayerID> ().isPlayersTurn = false;
		RpcChangePlayerTurn (nextPlayer,lastPlayer);
	}
	//Tell all clients who turn it is	
	[ClientRpc]
	void RpcChangePlayerTurn(GameObject nextPlayer,GameObject lastPlayer)
	{
		nextPlayer.GetComponent<PlayerID> ().isPlayersTurn = true;
		lastPlayer.GetComponent<PlayerID> ().isPlayersTurn = false;
	}
	//Reset the turn timer
	public void ResetTimer()
	{
		timer = MAX_TURN_TIME;
		nextTurn = false;
	}
}
