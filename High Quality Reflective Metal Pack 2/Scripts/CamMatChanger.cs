using UnityEngine;
using System.Collections;

public class CamMatChanger : MonoBehaviour {

	public Camera[] Cameras;
	public Material[] Mats;
	public GameObject Cylinder;
	public int current_cam = 0;
	public int current_mat = 0;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyUp (KeyCode.UpArrow)) 
		{
			current_cam +=1;
		}
		if (Input.GetKeyUp (KeyCode.DownArrow)) 
		{
			current_mat +=1;
		}

		if (Cylinder != null) 
		{
			var rend = Cylinder.GetComponent<MeshRenderer>();
			rend.material = Mats[current_mat % Mats.Length];
			Cylinder.GetComponent<Alex.DemoRotator>().speed = current_cam % 2 == 0? 5 : 10;
		}

		try
		{
			Cameras [(current_cam - 1) % Cameras.Length].enabled = false;
		} catch {}
		Cameras [current_cam % Cameras.Length].enabled = true;
	}
}
