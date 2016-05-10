using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LineID : NetworkBehaviour {
	[SyncVar] public string lineID;
    [SerializeField]
	 private Transform myTransform;
	 // Use this for initialization
	 void Start () 
	 {
		myTransform = transform;
	 }
	 
	 // Update is called once per frame
	 void Update () 
	 {
		SetIdentity();
	 }
	 
	 void SetIdentity()
	 {
		if(myTransform.name == "" || myTransform.name == "LineHor(Clone)" || myTransform.name == "LineVert(Clone)")
		{
			myTransform.name = lineID;
		}
	 }
}
