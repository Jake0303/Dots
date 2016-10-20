using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{
    public AudioClip music;
    public AudioSource bgMusic,buttonClick,sliderChanged;
    void Start()
    {
        //Play BG music on game start
        DontDestroyOnLoad(transform.gameObject);
        bgMusic = GetComponents<AudioSource>()[0];
        buttonClick = GetComponents<AudioSource>()[1];
        sliderChanged = GetComponents<AudioSource>()[2];
        bgMusic.volume = (GLOBALS.Volume / 50);
        bgMusic.Play();
    }
    //Play button click sound
    public void PlayButtonSound()
    {
        buttonClick.volume = (GLOBALS.Volume / 25.0f);
        buttonClick.Play();
    }
    //Play slider changed sound
    public void PlaySliderSound()
    {
        sliderChanged.volume = (GLOBALS.Volume / 100);
        sliderChanged.Play();
    }
}
