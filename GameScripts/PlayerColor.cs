using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;
using ExitGames.Client.Photon;


public class PlayerColor : PunBehaviour {
	public Color playerColor;
	private Transform myTransform;
    
    public Color[] colors = new Color[5];

    void Start()
    {
        PhotonPeer.RegisterType(typeof(Color), (byte)'C', SerializeColor, DeserializeColor);
        //This is added just so we can have indexes 1-4 not 0-3
        colors[0] = Color.black;
        colors[1] = Color.blue;
        colors[2] = Color.green;
        colors[3] = new Color(1,0,1,1);
        colors[4] = Color.red;

    }

    private static byte[] SerializeColor(object customobject)
    {
        Color vo = (Color)customobject;

        byte[] bytes = new byte[4 * 4];
        int index = 0;
        Protocol.Serialize(vo.r, bytes, ref index);
        Protocol.Serialize(vo.g, bytes, ref index);
        Protocol.Serialize(vo.b, bytes, ref index);
        Protocol.Serialize(vo.a, bytes, ref index);
        return bytes;
    }

    private static object DeserializeColor(byte[] bytes)
    {
        Color vo = new Color();
        int index = 0;
        Protocol.Deserialize(out vo.r, bytes, ref index);
        Protocol.Deserialize(out vo.g, bytes, ref index);
        Protocol.Deserialize(out vo.b, bytes, ref index);
        Protocol.Deserialize(out vo.a, bytes, ref index);
        return vo;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(SerializeColor(playerColor));
        }
        else
        {
            // Network player, receive data
            this.playerColor = (Color)DeserializeColor((byte[])stream.ReceiveNext());
        }
    }
    [PunRPC]
	public void CmdTellServerMyColor (Color myColor)
	{
		playerColor = myColor;
	}
}
