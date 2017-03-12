using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


public class PlayerPanel : PunBehaviour {

    public string owner = "";

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(owner);
        }
        else
        {
            this.owner = (string)stream.ReceiveNext();
        }
    }
}
