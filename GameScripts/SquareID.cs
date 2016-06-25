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
        myTransform = transform;
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
    // Update is called once per frame
    void Update()
    {
        SetIdentity();
    }

    void SetIdentity()
    {
        if (myTransform.name == "" || myTransform.name.Contains("Clone"))
        {
            myTransform.name = squareID;
        }
    }
}
