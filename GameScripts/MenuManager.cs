using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    string transitionText;
    public GameObject backButton, mainCamera;
    #region Public Variables
    [Space(20)]
    public Sprite[] icons;
    public string[] iconNames;
    #endregion

    #region Private Variables
    private WaitForSeconds checkMusicInterval = new WaitForSeconds(0.5f); //listener update time
    private int randomIconIndex = 0;
    #endregion


    //For this method to work we have an UITrigger attached to this gameObject and it is set to listen for All Game Events.
    //Also the method selected in the trigger is from the 'Dynamic string' section and NOT from the 'Static parameter' list. (this is important)
    public void OnGameEvent(string gameEvent)
    {
        switch (gameEvent)
        {
            case "Notification_DoozyUI": //shows the DoozyUI notification
                                         //DoozyUI.UIManager.ShowNotification("Notification_DoozyUI", 5f, false);
                break;

            case "UpdateSoundSettings":
                UpdateMusicState();
                break;
        }
    }

    //For this method to work we have an UITrigger attached to this gameObject and it is set to listen for All Button Clicks.
    //Also the method selected in the trigger is from the 'Dynamic string' section and NOT from the 'Static parameter' section. (this is important)
    public void OnButtonClick(string buttonName)
    {
        if (GameObject.Find("MainMenu") && GameObject.Find("MainMenu").GetComponent<DoozyUI.UIElement>().GetInAnimations.moveIn.delay != 0.25f)
            GameObject.Find("MainMenu").GetComponent<DoozyUI.UIElement>().GetInAnimations.moveIn.delay = 0.25f;

        switch (buttonName)
        {
            case "ToggleMusic":
                UpdateMusicState();
                break;
        }
    }

    /// <summary>
    /// If it finds the song with the specified soundName in a Resources folder it will create, configure and return an AudioSource.
    /// </summary>
    /// <param name="soundName">The filename of a sound file in a Resources folder.</param>
    /// <returns></returns>
    private AudioSource SetupMusic(string soundName)
    {
        if (System.String.IsNullOrEmpty(soundName) == false)
        {
            AudioClip clip = Resources.Load(soundName) as AudioClip;
            if (clip == null)
            {
                Debug.Log("[DoozyUI] There is no file with the name [" + soundName + "] in any of the Resources folders.");
            }
            else
            {
                var tempGO = new GameObject("Music - " + clip.name); // create the temp object
                tempGO.transform.position = Vector3.zero; // set its position
                var aSource = tempGO.AddComponent<AudioSource>(); ; // add an audio source
                //aSource.mute = !DoozyUI.UIManager.isMusicOn;  //we check if the music is on or off
                aSource.clip = clip; // define the clip
                aSource.loop = true;
                return aSource; // return the AudioSource reference
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if the music is turned on or off at specified intervals.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckMusicState()
    {
        while (true)
        {
            yield return checkMusicInterval;
            UpdateMusicState();
        }
    }



    void MyOrientationChangeCode(DeviceOrientation orientation)
    {
    }


    private void UpdateMusicState()
    {
    }

    void Start()
    {
        DoozyUI.UIManager.DisableBackButton();
        DeviceChange.OnOrientationChange += MyOrientationChange;
        GameObject.Find("Title").GetComponent<Text>().text = GLOBALS.GameName;
        mainCamera = GameObject.Find("Main Camera");
        backButton = GameObject.Find("BackToMenuButton");
        // Initialize volume slider
        if (GameObject.Find("VolumeSlider") != null)
            GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GLOBALS.Volume;
        if (Application.platform != RuntimePlatform.Android
            && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            GameObject.Find("ContentText").GetComponent<Text>().text = "Place lines by clicking your mouse \n and placing your cursor between \n 2 dots. \n \n \n \n \n \n \n " +
                " Earn points by completing squares \n" + " on the grid of dots. \n \n Win by having the most points \n in a given game. ";
        }
        else if ((Screen.orientation == ScreenOrientation.Portrait
            || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            && (SceneManager.GetActiveScene().buildIndex == 0))
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = 55;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = 30;
        }
    }


    void MyOrientationChange(DeviceOrientation orientation)
    {
        if ((orientation == DeviceOrientation.Portrait
            || orientation == DeviceOrientation.PortraitUpsideDown)
            && (SceneManager.GetActiveScene().buildIndex == 0))
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = 55;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = 30;
        }
    }


    //Update the volume when the slider has changed
    public void OnVolumeSliderChanged(float value)
    {
        GLOBALS.Volume = value;
        GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GLOBALS.Volume;
        GameObject.Find("VolumeLevel").GetComponent<Text>().text = GLOBALS.Volume.ToString();
        if (GameObject.Find("AudioManager").GetComponent<Sound>().bgMusic != null)
            GameObject.Find("AudioManager").GetComponent<Sound>().bgMusic.volume = (GLOBALS.Volume / 50);
        if (value > 0)
        {
            GameObject.Find("SoundOFF").GetComponent<DoozyUI.UIElement>().Hide(true);
            GameObject.Find("SoundON").GetComponent<DoozyUI.UIElement>().Show(false);
        }
        else
        {
            GameObject.Find("SoundOFF").GetComponent<DoozyUI.UIElement>().Show(false);
            GameObject.Find("SoundON").GetComponent<DoozyUI.UIElement>().Hide(false);
        }
    }

    public void TurnOnSound()
    {
        OnVolumeSliderChanged(25);
        GameObject.Find("AudioManager").GetComponent<Sound>().PlaySliderSound();
        GameObject.Find("SoundOFF").GetComponent<DoozyUI.UIElement>().Hide(true);
        GameObject.Find("SoundON").GetComponent<DoozyUI.UIElement>().Show(false);
    }

    public void TurnOffSound()
    {
        OnVolumeSliderChanged(0);
        GameObject.Find("SoundOFF").GetComponent<DoozyUI.UIElement>().Show(false);
        GameObject.Find("SoundON").GetComponent<DoozyUI.UIElement>().Hide(false);
    }
    //Fade text animation for the connecting text
    IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            if (i != null)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            }
            yield return null;
        }
    }

    //Show text while we connect to the matchmaking service
    public void DisplayLoadingText(string text)
    {
        if (GameObject.Find("transitionText") != null)
        {
            GameObject.Find("transitionText").GetComponent<Text>().text = text;
            StartCoroutine(FadeTextToFullAlpha(2f, GameObject.Find("transitionText").GetComponent<Text>()));
        }
    }
    public void TransitionToEnterNameScreen()
    {
        GameObject.Find("PlayButton").GetComponent<Button>().enabled = false;
        GameObject.Find("PlayButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("PlayButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("OptionsButton").GetComponent<Button>().enabled = false;
        GameObject.Find("OptionsButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("OptionsButton").GetComponentInChildren<Text>().color = Color.clear;
        if (GameObject.Find("ExitButton"))
        {
            GameObject.Find("ExitButton").GetComponent<Button>().enabled = false;
            GameObject.Find("ExitButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
            GameObject.Find("ExitButton").GetComponentInChildren<Text>().color = Color.clear;
        }
        GameObject.Find("InstructionsButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("InstructionsButton").GetComponentInChildren<Text>().color = Color.clear;
    }


    public void TopLeaderboardHandleClick()
    {
        GameObject.Find("LeaderboardPanel").transform.localPosition -= new Vector3(0, 30, 0);

    }

    public void BotLeaderboardHandleClick()
    {
        GameObject.Find("LeaderboardPanel").transform.localPosition += new Vector3(0, 30, 0);
    }


    public void ReloadLeaderBoard()
    {
        if (LeaderbordController.leaderBoardError)
        {
            StartCoroutine(GameObject.Find("scores").GetComponent<LeaderbordController>().GetScores());
        }
    }

    //Quit the game
    public void ExitGame()
    {
        Application.Quit();
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
    }
}
