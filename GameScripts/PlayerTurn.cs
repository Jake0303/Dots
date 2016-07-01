using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using System;
using Photon;
using System.Collections.Generic;
using ExitGames.Client.Photon;


public class PlayerTurn : PunBehaviour {
	public List<int> assortPlayerTurns = new List<int>();
    private byte[] playerTurns;
    void Start()
    {
        PhotonPeer.RegisterType(typeof(List<int>), (byte)'L', SerializeListInt, DeserializeListInt);
    }
    private static byte[] SerializeListInt(object customobject)
    {
        List<int> vo = (List<int>)customobject;

        byte[] bytes = new byte[vo.Count * 4];
        bytes = vo.SelectMany<int, byte>(BitConverter.GetBytes).ToArray();
        Protocol.Serialize(bytes);
        return bytes;
    }

    private static object DeserializeListInt(byte[] bytes)
    {
        List<int> vo = new List<int>();
        Protocol.Deserialize(vo.SelectMany<int, byte>(BitConverter.GetBytes).ToArray());
        return vo;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            //Syncing player turn
            playerTurns = assortPlayerTurns.SelectMany<int, byte>(BitConverter.GetBytes).ToArray();
            stream.SendNext(playerTurns);
        }
        else
        {
            // Network player, receive data
            this.playerTurns = (byte[])stream.ReceiveNext();
            this.assortPlayerTurns = Enumerable.Range(0, playerTurns.Length / 4)
                             .Select(i => BitConverter.ToInt32(playerTurns, i * 4))
                             .ToList();
        }
    }
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
