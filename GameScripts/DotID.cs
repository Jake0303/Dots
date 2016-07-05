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
    public void CmdSetName(string name)
    {
        photonView.RPC("RpcSetName", PhotonTargets.AllBuffered, name);
    }
    [PunRPC]
    void RpcSetName(string name)
    {
        dotID = name;
        transform.name = name;
    }
}
