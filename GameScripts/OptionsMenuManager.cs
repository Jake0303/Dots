using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class OptionsMenuManager : MonoBehaviour {

	// Initialize volume slider
	void Start () {
        GameObject.Find("GameName").GetComponent<Text>().text = GLOBALS.GameName;
        if (GameObject.Find("VolumeSlider") != null)
            GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GLOBALS.Volume;
	}
    //Update the volume when the slider has changed
	public void OnVolumeSliderChanged(float value)
    {
        GLOBALS.Volume = value;
        GameObject.Find("VolumeLevel").GetComponent<Text>().text = GLOBALS.Volume.ToString();
        GameObject.Find("AudioManager").GetComponent<Sound>().fxSound.volume = (GLOBALS.Volume/100);
    }
    //Go back to the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
