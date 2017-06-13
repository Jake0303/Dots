using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Sound : MonoBehaviour
{
    public AudioSource bgMusic, buttonClick, sliderChanged;

    void Start()
    {
        //Play BG music on game start
        DontDestroyOnLoad(transform.gameObject);
        buttonClick = GetComponents<AudioSource>()[1];
        sliderChanged = GetComponents<AudioSource>()[2];
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            bgMusic.Stop();
            bgMusic = GetComponents<AudioSource>()[0];
            if (GLOBALS.Volume > 0)
                StartCoroutine(FadeIn(bgMusic, 6f));
        }
        else
        {
            bgMusic.Stop();
            bgMusic = GetComponents<AudioSource>()[3];
            if (GLOBALS.Volume > 0)
                StartCoroutine(FadeIn(bgMusic, 2f));
        }
    }

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 0.01f;
        audioSource.Play();
        audioSource.volume = startVolume;
        while (audioSource.volume < (GLOBALS.Volume / 100.0f))
        {
            audioSource.volume += startVolume * Time.deltaTime * FadeTime;

            yield return null;
        }
    }

    //Play button click sound
    public void PlayButtonSound()
    {
        // Initialize volume slider
        if (GameObject.Find("VolumeSlider") != null)
            GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GLOBALS.Volume;
        buttonClick.volume = (GLOBALS.Volume / 45.0f);
        buttonClick.Play();
    }
    //Play slider changed sound
    public void PlaySliderSound()
    {
        sliderChanged.volume = (GLOBALS.Volume / 100);
        sliderChanged.Play();
    }
}
