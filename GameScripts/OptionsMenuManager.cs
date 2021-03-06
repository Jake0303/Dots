using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class OptionsMenuManager : MonoBehaviour {

	// Initialize volume slider
	void Start () {
        if (GameObject.Find("VolumeSlider") != null)
            GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GLOBALS.Volume;
	}
    //Update the volume when the slider has changed
	public void OnVolumeSliderChanged(float value)
    {
        GLOBALS.Volume = value;
        GameObject.Find("VolumeLevel").GetComponent<Text>().text = GLOBALS.Volume.ToString();
        GameObject.Find("AudioManager").GetComponent<Sound>().bgMusic.volume = (GLOBALS.Volume/50);
        GameObject.Find("AudioManager").GetComponent<Sound>().PlaySliderSound();
    }
    //Go back to the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
    }
}
