using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LinePlaced : NetworkBehaviour {
	[SyncVar (hook = "OnLinePlaced")] public bool linePlaced;
	[SerializeField]public Material hoverMat;
	//Material for the line placed
	[SerializeField] public Material lineMat;
	[SyncVar]
	private Color objectColor;


	void OnLinePlaced(bool lineChanged)
	{
		if (lineChanged && !GameObject.Find("GameManager").GetComponent<GameStart>().buildGrid) {
			//GetComponent<Renderer> ().enabled = true;
			linePlaced = lineChanged;
		}
	}


}
