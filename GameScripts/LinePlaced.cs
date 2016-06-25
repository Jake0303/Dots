using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class LinePlaced : PunBehaviour {
	public bool linePlaced;
	[SerializeField]public Material hoverMat;
	//Material for the line placed
	[SerializeField] public Material lineMat;
	//
	private Color objectColor;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(linePlaced);
        }
        else
        {
            // Network player, receive data
            this.linePlaced = (bool)stream.ReceiveNext();
        }
    }

	void OnLinePlaced(bool lineChanged)
	{
		if (lineChanged && !GameObject.Find("GameManager").GetComponent<GameStart>().buildGrid) {
			linePlaced = lineChanged;
		}
	}


}
