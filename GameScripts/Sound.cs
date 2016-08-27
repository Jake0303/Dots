using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{
    public AudioSource bgMusic,buttonClick;
    void Start()
    {
        //Play BG music on game start
        DontDestroyOnLoad(transform.gameObject);
        bgMusic = GetComponents<AudioSource>()[0];
        bgMusic.volume = (GLOBALS.Volume / 100);
        bgMusic.Play();
    }
    //Play button click sound
    public void PlayButtonSound()
    {
        buttonClick = GetComponents<AudioSource>()[1];
        buttonClick.volume = (GLOBALS.Volume / 100);
        buttonClick.Play();
    }
}
