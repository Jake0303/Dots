// this script is only used for rotating the runtime demo scene object

using UnityEngine;
using System.Collections;

namespace Alex
{
	public class DemoRotator : MonoBehaviour 
	{
		public float speed = 25F;
		void Update () 
		{
			transform.Rotate(0,speed*Time.deltaTime,0);
		}
	}
}