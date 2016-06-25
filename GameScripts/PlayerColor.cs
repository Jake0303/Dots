using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class PlayerColor : PunBehaviour {
	public Color playerColor;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(playerColor);
        }
        else
        {
            // Network player, receive data
            this.playerColor = (Color)stream.ReceiveNext();
        }
    }

	// Use this for initialization
	void Awake () {
	}

	// Update is called once per frame
	void Update () {

	}

	void GetNetIdentity()
	{
		CmdTellServerMyColor (MakeUniqueColor());
	}



	Color MakeUniqueColor ()
	{
        
		Color uniqueColor = new Color (Random.value, Random.value, Random.value, Random.value);
		return uniqueColor;
	}
	public void CmdTellServerMyColor (Color myColor)
	{
		playerColor = myColor;
	}
}
