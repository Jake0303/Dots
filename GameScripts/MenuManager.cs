using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    string transitionText;
    public GameObject backButton;
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


            #region EXAMPLE 1 - notifications

            case "Example_1_Notification_1":
                DoozyUI.UIManager.ShowNotification("Example_1_Notification_1", 5f, false, "This is a 5 seconds notification! \n - click on it to close -");
                break;

            case "Example_1_Notification_2":
                DoozyUI.UIManager.ShowNotification("Example_1_Notification_2", -1f, false, "About", "DoozyUI is a sure way to create blazing fast user interfaces with all kinds of animations that you can imagine.", null, new string[1] { "CloseNotificationButton" }, new string[1] { "Got it!" });
                break;

            case "Example_1_Notification_3":
                DoozyUI.UIManager.ShowNotification("Example_1_Notification_3", -1f, false, new string[2] { "OkButton", "CancelButton" }, new string[2] { "Ok", "Cancel" });
                break;

            case "Example_1_Notification_4":
                randomIconIndex = Random.Range(0, icons.Length); //we get a random icon index so that we can show/simulate a different notification each time
                DoozyUI.UIManager.ShowNotification("Example_1_Notification_4", 3f, true, iconNames[randomIconIndex], icons[randomIconIndex]);
                break;

            case "Example_1_Notification_5":
                DoozyUI.UIManager.ShowNotification("Example_1_Notification_5", 4f, true, icons[Random.Range(0, icons.Length)]); //we select a random sprite to show as the main icon
                break;

            case "Example_1_Notification_6":
                DoozyUI.UIManager.ShowNotification("Example_1_Notification_6", -1f, true, "Click anywhere to close this notification!");
                break;

            #endregion

            #region EXAMPLE 2 - notifications

            case "Example_2_Notification_1":
                DoozyUI.UIManager.ShowNotification("Example_2_Notification_1", 5f, false, "This is a 5 seconds notification! \n - click on it to close -");
                break;

            case "Example_2_Notification_2":
                DoozyUI.UIManager.ShowNotification("Example_2_Notification_2", -1f, false, "One Button Notification", "Press the OK button to close this notification!", null, new string[1] { "CloseNotificationButton" }, new string[1] { "OK" });
                break;

            case "Example_2_Notification_3":
                DoozyUI.UIManager.ShowNotification("Example_2_Notification_3", -1f, false, "Level Completed", new string[2] { "NextLevelButton", "ReplayButton" }, new string[2] { "Next Level", "Replay" });
                break;

            case "Example_2_Notification_4":
                DoozyUI.UIManager.ShowNotification("Example_2_Notification_4", 3f, true, icons[Random.Range(0, icons.Length)]);
                break;

            case "Example_2_Notification_5":
                randomIconIndex = Random.Range(0, icons.Length); //we get a random icon index so that we can show/simulate a different notification each time
                DoozyUI.UIManager.ShowNotification("Example_2_Notification_5", 4f, true, iconNames[randomIconIndex], icons[randomIconIndex]);
                break;

            case "Example_2_Notification_6":
                DoozyUI.UIManager.ShowNotification("Example_2_Notification_6", -1f, true, "About", "DoozyUI is a sure way to create blazing fast user interfaces with all kinds of animations that you can imagine.", null, new string[1] { "CloseNotificationButton" }, new string[1] { "OK" });
                break;

            #endregion

            #region EXAMPLE 3 - notifications

            case "Example_3_Notification_1":
                DoozyUI.UIManager.ShowNotification("Example_3_Notification_1", 5f, false, "Quick notification! \n - click to close -");
                break;

            case "Example_3_Notification_2":
                DoozyUI.UIManager.ShowNotification("Example_3_Notification_2", -1f, false, "Three Button Notification", "This notification has three buttons \n click any of them to close", null, new string[3] { "Button1", "Button2", "Button3" }, new string[3] { "one", "two", "three" });
                break;

            case "Example_3_Notification_3":
                DoozyUI.UIManager.ShowNotification("Example_3_Notification_3", -1f, false, new string[2] { "OkButton", "CancelButton" }, new string[2] { "Ok", "Cancel" });
                break;

            case "Example_3_Notification_4":
                randomIconIndex = Random.Range(0, icons.Length); //we get a random icon index so that we can show/simulate a different notification each time
                DoozyUI.UIManager.ShowNotification("Example_3_Notification_4", 3f, true, iconNames[randomIconIndex], icons[randomIconIndex]);
                break;

            case "Example_3_Notification_5":
                randomIconIndex = Random.Range(0, icons.Length); //we get a random icon index so that we can show/simulate a different notification each time
                DoozyUI.UIManager.ShowNotification("Example_3_Notification_5", 3f, true, icons[randomIconIndex]);
                break;

            case "Example_3_Notification_6":
                DoozyUI.UIManager.ShowNotification("Example_3_Notification_6", -1f, true, "About", "DoozyUI is a sure way to create blazing fast user interfaces with all kinds of animations that you can imagine.", null, new string[1] { "CloseNotificationButton" }, new string[1] { "OK" });
                break;

            #endregion

            #region EXAMPLE 4 - notifications

            case "Example_4_Notification_1":
                DoozyUI.UIManager.ShowNotification("Example_4_Notification_1", 5f, false, "", "Quick notification \n 5 seconds lifetime");
                break;

            case "Example_4_Notification_2":
                DoozyUI.UIManager.ShowNotification("Example_4_Notification_2", 4f, false, "", "Your message here! \n click to hide");
                break;

            case "Example_4_Notification_3":
                DoozyUI.UIManager.ShowNotification("Example_4_Notification_3", -1, false, "Modal window", "To close this window, click any button. \n This notification has 3 buttons available, but we chose to use only 2.", null, new string[2] { "button 1", "button 2" }, new string[2] { "1", "2" });
                break;

            case "Example_4_Notification_4":
                DoozyUI.UIManager.ShowNotification("Example_4_Notification_4", 2f, true);
                break;

            case "Example_4_Notification_5":
                DoozyUI.UIManager.ShowNotification("Example_4_Notification_5", 3f, true);
                break;

            case "Example_4_Notification_6":
                DoozyUI.UIManager.ShowNotification("Example_4_Notification_6", 5f, true, "Doozy UI", "COMPLETE UI MANAGEMENT SYSTEM");
                break;

                #endregion
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

    private void UpdateMusicState()
    {
    }

    void Start()
    {
        GameObject.Find("Title").GetComponent<Text>().text = GLOBALS.GameName;
        backButton = GameObject.Find("BackToMenuButton");
        // Initialize volume slider
        if (GameObject.Find("VolumeSlider") != null)
            GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GLOBALS.Volume;
        if(Application.platform != RuntimePlatform.Android
            && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            GameObject.Find("ContentText").GetComponent<Text>().text = "Place lines by clicking your mouse \n and placing your cursor between \n 2 dots. \n \n \n \n \n \n \n " +
                " Earn points by completing squares \n" + " on the grid of dots. \n \n Win by having the most points \n in a given game. ";
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

    public void ReloadLeaderBoard()
    {
        if (LeaderbordController.leaderBoardError)
        {
            StartCoroutine(GameObject.Find("scores").GetComponent<LeaderbordController>().GetScores());
        }
    }

    void Update()
    {
        if (Input.deviceOrientation == DeviceOrientation.Portrait) {
        }
    }
    //Quit the game
    public void ExitGame()
    {
        Application.Quit();
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
    }
}
