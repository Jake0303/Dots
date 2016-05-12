using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {
    AudioSource fxSound; // Emitir sons
    public AudioClip backMusic; // Som de fundo
    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(transform.gameObject);
        // Audio Source responsavel por emitir os sons
        fxSound = GetComponent<AudioSource>();
        //TODO:Enable sound
        //fxSound.Play();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
