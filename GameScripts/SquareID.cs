using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class SquareID : PunBehaviour
{
    //
    public string squareID;
    [SerializeField]
    private Transform myTransform;
    // Use this for initialization
    void Start()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
    }
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
    private void OnEvent(byte eventcode, object content, int senderid)
    {
        //Update dot name
        if (eventcode == 5 && transform.name.Contains("Clone"))
        {
            transform.name = (string)content;
        }
    }
}
