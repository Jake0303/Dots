using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class SquareID : PunBehaviour
{
    public string squareID;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            //Syncing player turn
            stream.SendNext(squareID);
        }
        else
        {
            // Network player, receive data
            this.squareID = (string)stream.ReceiveNext();

        }
    }
   public void CmdSetName(string name)
    {
        photonView.RPC("RpcSetName", PhotonTargets.AllBuffered, name);
    }
    [PunRPC]
    void RpcSetName(string name)
    {
        squareID = name;
        transform.name = name;
    }
}
