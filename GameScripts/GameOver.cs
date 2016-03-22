﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : NetworkBehaviour {
	[SyncVar] public bool gameOver = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameOver) {
			StartCoroutine (DisplayWinner ());
		}
	}
	//Display the winner of the game
	IEnumerator DisplayWinner()
	{
		var players = GameObject.FindGameObjectsWithTag ("Player");
		List<int> scores = new List<int>();
		//Gather all the scores and see who is the highest
		foreach (var player in players) {
			scores.Add (player.GetComponent<PlayerID> ().playerScore);
		}
		int highestScore = scores.Max ();
		//Display the winner
		foreach (var player in players) {
			//The winner
			if (player.GetComponent<PlayerID> ().playerScore == highestScore) {
				gameOver = false;
			}
		}
		//Wait 5 seconds before resetting
		yield return new WaitForSeconds(5);

		ResetGame();
	}
	//Reset the game
	void ResetGame()
	{
		GameObject.Find ("NetworkManager").GetComponent<NetworkManagerCustom> ().ChangeLevel ();
	}
}