using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManagerSingleton : MonoBehaviour
{

    private static NetworkManagerSingleton instance = null;
    public static NetworkManagerSingleton Instance
    {
        get { return instance; }
    }
    void Start()
    {
        RegisterButtons();
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            RegisterButtons();
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void RegisterButtons()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        //GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().PlayClicked()));
        GameObject.Find("PlayAsGuestButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));

        //Remove all ;isteneres prevents duplicate event calls
        GameObject.Find("LetsPlayButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("LetsPlayButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("LetsPlayButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().PlayButtonClicked()));

        GameObject.Find("facebookLoginButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("facebookLoginButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<FacebookManager>().FBButtonClick()));

        GameObject.Find("InstructionsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("LeaderboardsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("LeaderboardsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().UpdateLeaderboardData()));
        GameObject.Find("LBackToMenuButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("LoginBackToMenuButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("OptionsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("InstructionsOKButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("OptionsOKButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("BackToMenuButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
        GameObject.Find("VolumeSlider").GetComponent<Slider>().onValueChanged.AddListener(GameObject.Find("MenuManager").GetComponent<MenuManager>().OnVolumeSliderChanged);
        GameObject.Find("SoundOFF").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().TurnOnSound()));
        GameObject.Find("SoundON").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().TurnOffSound()));
        GameObject.Find("ColorBlindAssistCheckbox").GetComponent<Toggle>().onValueChanged.AddListener(GameObject.Find("MenuManager").GetComponent<MenuManager>().OnColorBlindCheckboxChanged);
        if (GameObject.Find("FullscreenCheckbox") != null)
            GameObject.Find("FullscreenCheckbox").GetComponent<Toggle>().onValueChanged.AddListener(GameObject.Find("MenuManager").GetComponent<MenuManager>().OnFullscreenCheckboxChanged);

        if (GLOBALS.Volume > 0)
        {
            GameObject.Find("SoundOFF").GetComponent<DoozyUI.UIElement>().Hide(false);
            GameObject.Find("SoundON").GetComponent<DoozyUI.UIElement>().Show(false);
        }
        else
        {
            GameObject.Find("SoundOFF").GetComponent<DoozyUI.UIElement>().Show(false);
            GameObject.Find("SoundON").GetComponent<DoozyUI.UIElement>().Hide(false);
        }
        if (GameObject.Find("NetworkManager").GetComponent<NetworkManager>().findAnotherMatch)
        {
            GameObject.Find("MainMenu").GetComponent<DoozyUI.UIElement>().Hide(false);
            GameObject.Find("ConnectingMenu").GetComponent<DoozyUI.UIElement>().Show(false);
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().JoinGame(false);
        }
        Button[] buttons = GameObject.Find("UI").GetComponentsInChildren<Button>();

        foreach (Button but in buttons)
        {
            if (but.gameObject.name == "PlayAsGuestButton")
            {
                but.onClick.AddListener(() => GameObject.Find("NetworkManager").GetComponent<NetworkManager>().JoinGame(false));
            }
        }
    }
}
