using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class LineID : PunBehaviour {
	 public string lineID;
     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
     {
         if (stream.isWriting && (transform.name.Contains("Clone") || transform.name == ""))
         {
             // We own this player: send the others our data
             stream.SendNext(lineID);
         }
         else 
         {
             if (transform.name.Contains("Clone") || transform.name == "")
             {
                // Network player, receive data
                this.lineID = (string)stream.ReceiveNext();
                transform.name = this.lineID;
             }
         }
     }
    public void CmdSetName(string name)
    {
        photonView.RPC("RpcSetName", PhotonTargets.AllBuffered, name);
    }
    [PunRPC]
    void RpcSetName(string name)
    {
        lineID = name;
        transform.name = name;
    }

}
