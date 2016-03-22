using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateValueForSlider : MonoBehaviour {

	public Text sizeText;
	private float SliderValue;

	public void UpdateText()
	{
		SliderValue = GameObject.Find ("GridSizeSlider").GetComponent<Slider> ().value;
		GameObject.Find ("GridSizeLabel").GetComponent<Text> ().text = SliderValue.ToString () + " x " + SliderValue.ToString ();
	}

}
