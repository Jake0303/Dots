using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerColor : NetworkBehaviour {
	[SyncVar] public Color playerColor;
	private Transform myTransform;
    
    public Color[] colors = new Color[5];

    void Start()
    {
        //This is added just so we can have indexes 1-4 not 0-3
        colors[0] = Color.black;
        colors[1] = Color.blue;
        colors[2] = Color.green;
        colors[3] = new Color(1,0,1,1);
        colors[4] = Color.red;

    }

	public override void OnStartLocalPlayer()
	{
		//GetNetIdentity ();
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
	public void CmdTellServerMyColor (Color myColor)
	{
		playerColor = myColor;
	}
}
