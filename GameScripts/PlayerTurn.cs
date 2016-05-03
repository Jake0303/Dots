using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using System;


public class PlayerTurn : NetworkBehaviour {
	public SyncListInt assortPlayerTurns = new SyncListInt();
	// Assort the player turn order randomly at the start of a game
	public void AssignTurns () {
		var rnd = new System.Random();
        var randomNumbers = Enumerable.Range(1, GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count).OrderBy(x => rnd.Next()).Take(GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count).ToArray();
		//This is added just so we can have indexes 1-4 not 0-3
		assortPlayerTurns.Add (0);
		assortPlayerTurns.Add (randomNumbers.ElementAt (0));
        if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count > 1)
		    assortPlayerTurns.Add (randomNumbers.ElementAt (1));
        if(GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count > 2)
		    assortPlayerTurns.Add (randomNumbers.ElementAt (2));
        if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count > 3)
		    assortPlayerTurns.Add (randomNumbers.ElementAt (3));
	}
}
