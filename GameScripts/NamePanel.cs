using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class NamePanel : PunBehaviour {
    
    public bool set;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(set);
        }
        else
        {
            // Network player, receive data
            this.set = (bool)stream.ReceiveNext();
        }
    }
}
