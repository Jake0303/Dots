using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;
using ExitGames.Client.Photon;


public class PlayerColor : PunBehaviour {
	public Color playerColor;
	private Transform myTransform;
    private Vector3 tempColor;

    public Color[] colors = new Color[5];

    void Start()
    {
        //This is added just so we can have indexes 1-4 not 0-3
        colors[0] = Color.black;
        if (!GLOBALS.ColorBlindAssist)
        {
            colors[1] = Color.green;
            colors[2] = Color.red;
        }
        else
        {
            colors[1] = Color.blue;
            colors[2] = Color.yellow;
        }
    }
	public void CmdTellServerMyColor (Color myColor)
	{
        photonView.RPC("RpcTellServerMyColor", PhotonTargets.AllBuffered, myColor.r,myColor.g,myColor.b,myColor.a);
	}
    [PunRPC]
    public void RpcTellServerMyColor(float r,float g,float b,float a)
    {
        playerColor = new Color(r, g, b, a);
    }
}
