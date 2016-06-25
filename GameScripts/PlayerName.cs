using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using Photon;

public class PlayerName : PunBehaviour {
    public string enteredName;

    public void setName()
    {
        if(photonView.isMine)
            enteredName = GameObject.Find("EnterNameInputField").GetComponent<InputField>().text;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            //Syncing player turn
            if (photonView.isMine)
            {
                stream.SendNext(enteredName);
            }
        }
        else
        {
            // Network player, receive data
            this.enteredName = (string)stream.ReceiveNext();

        }
    }
}
