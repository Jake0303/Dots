using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {
	// Update the UI for the player score to 0
	void Start () {
		var players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (var player in players) {
			UpdateScore (player);
		}
	}
	
	// Update is called once per frame
	public void UpdateScore (GameObject player) {
		//Update the player's UI with their score
		GameObject.Find ("ScoreText").GetComponent<Text> ().text = "Score: " + player.GetComponent<PlayerID>().playerScore;
	}
}
