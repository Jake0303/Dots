using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class DotID : PunBehaviour {

	public string dotID;
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
		if(myTransform.name == "" || myTransform.name.Contains("Clone"))
		{
			myTransform.name = dotID;
		}
	}
}
