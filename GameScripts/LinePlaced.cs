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
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[ClientRpc]
	void RpcChangeLineAppearance()
	{
		//StartCoroutine(CheckIfPlayerPlacedLine());
	}
	void OnLinePlaced(bool lineChanged)
	{
		if (lineChanged) {
			//GetComponent<Renderer> ().enabled = true;
			linePlaced = lineChanged;
		}
	}

	IEnumerator CheckIfPlayerPlacedLine()
	{
		if (linePlaced) {
			yield return new WaitForSeconds (0.2f);
			//GetComponent<Renderer> ().enabled = true;
		}
	}
}
