using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class LineID : PunBehaviour {
	 public string lineID;
     [SerializeField]
	 private Transform myTransform;
	 // Use this for initialization
	 void Start () 
	 {
         PhotonNetwork.OnEventCall += this.OnEvent;
     }
     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
     {
         if (stream.isWriting)
         {
             // We own this player: send the others our data
             stream.SendNext(lineID);
         }
         else
         {
             // Network player, receive data
             this.lineID = (string)stream.ReceiveNext();
         }
     }
     private void OnEvent(byte eventcode, object content, int senderid)
     {
         //Update dot name
         if ((eventcode == 3 || eventcode == 4) && this != null 
             && transform.name.Contains("Clone"))
         {
             transform.name = (string)content;
         }
     }
}
