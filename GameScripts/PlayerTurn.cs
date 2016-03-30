using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using System;


public class PlayerTurn : NetworkBehaviour {
	public SyncListInt assortPlayerTurns = new SyncListInt();
	// Assort the player turn order randomly at the start of a game
	void Start () {
		var rnd = new System.Random();
		var randomNumbers = Enumerable.Range(1,4).OrderBy(x => rnd.Next()).Take(4).ToArray();
		//This is added just so we can have indexes 1-4 not 0-3
		assortPlayerTurns.Add (0);
		assortPlayerTurns.Add (randomNumbers.ElementAt (0));
		assortPlayerTurns.Add (randomNumbers.ElementAt (1));
		assortPlayerTurns.Add (randomNumbers.ElementAt (2));
		assortPlayerTurns.Add (randomNumbers.ElementAt (3));
	}
}
