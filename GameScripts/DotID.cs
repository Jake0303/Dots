using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class DotID : PunBehaviour {

	public string dotID;
	private Transform myTransform;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(dotID);
        }
        else
        {
            // Network player, receive data
            this.dotID = (string)stream.ReceiveNext();
        }
    }
	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
	}

	// Update is called once per frame
	void Update () 
	{
		SetIdentity();
	}
	
	void SetIdentity()
	{
		if(myTransform.name == "" || myTransform.name.Contains("Clone"))
		{
			myTransform.name = dotID;
		}
	}
}
