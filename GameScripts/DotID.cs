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
        PhotonNetwork.OnEventCall += this.OnEvent;
	}
    private void OnEvent(byte eventcode, object content, int senderid)
    {
        //Update dot name
        if (eventcode == 2 && transform.name.Contains("Clone"))
        {
            transform.name = (string)content;
        }
    }
}
