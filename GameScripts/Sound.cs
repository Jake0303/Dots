using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{
    public AudioSource fxSound;
    public AudioClip backMusic;
    void Start()
    {
        //Play BG music on game start
        DontDestroyOnLoad(transform.gameObject);
        fxSound = GetComponent<AudioSource>();
        fxSound.volume = (GLOBALS.Volume / 100);
        fxSound.Play();
    }
}
