using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerColor : NetworkBehaviour {
	[SyncVar] public Color playerColor;
	private Transform myTransform;

	public override void OnStartLocalPlayer()
	{
		GetNetIdentity ();
	}

	// Use this for initialization
	void Awake () {
	}

	// Update is called once per frame
	void Update () {

	}

	[Client]
	void GetNetIdentity()
	{
		CmdTellServerMyColor (MakeUniqueColor());
	}



	Color MakeUniqueColor ()
	{
		Color uniqueColor = new Color (Random.value, Random.value, Random.value, Random.value);
		return uniqueColor;
	}
	[Command]
	void CmdTellServerMyColor (Color myColor)
	{
		playerColor = myColor;
	}
}
