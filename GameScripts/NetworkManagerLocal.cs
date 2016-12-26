using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;
using Photon;
using UnityEngine.SceneManagement;

public class NetworkManagerLocal : PunBehaviour
{
    public bool AutoConnect = true;
    public GameObject newPlayer;
    private bool ConnectInUpdate = false;

    void Start()
    {
        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
        PhotonNetwork.automaticallySyncScene = true;
        SceneManager.sceneLoaded += SceneLoaded;
    }
    void Update()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            string conn = "Connecting";
            GameObject.Find("MenuManager").GetComponent<MenuManager>().DisplayLoadingText(conn);
            PhotonNetwork.ConnectUsingSettings(GLOBALS.Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
            ConnectInUpdate = false;

        }
    }
    //Join lobby
    public void JoinGame()
    {
        ConnectInUpdate = true;
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        string conn = "Joining match";
        GameObject.Find("MenuManager").GetComponent<MenuManager>().DisplayLoadingText(conn);
        //Tries to join any random game:
        PhotonNetwork.JoinRandomRoom();
        //Fails if there are no matching games: OnPhotonRandomJoinFailed
        if (GameObject.Find("BackToMenuButton"))
        {
            GameObject.Find("BackToMenuButton").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("BackToMenuButton").GetComponent<Button>().enabled = true;
            GameObject.Find("BackToMenuButton").GetComponent<Button>().onClick.AddListener(() => ShowMainMenu());
        }
    }
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        base.OnFailedToConnectToPhoton(cause);
        string conn = "There is a Network Issue, please check your Internet connection.";
        //Make transition text fit the screen
        GameObject.Find("transitionText").GetComponent<Text>().fontSize = 35;
        GameObject.Find("transitionText").GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
        GameObject.Find("MenuManager").GetComponent<MenuManager>().DisplayLoadingText(conn);
        if (GameObject.Find("BackToMenuButton"))
        {
            GameObject.Find("BackToMenuButton").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("BackToMenuButton").GetComponent<Button>().enabled = true;
            GameObject.Find("BackToMenuButton").GetComponent<Button>().onClick.AddListener(() => ShowMainMenu());
        }
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        base.OnConnectionFail(cause);
        string conn = "Our servers are full! Please try again later.";
        //Make transition text fit the screen
        if (GameObject.Find("trainsitionText"))
        {
            GameObject.Find("transitionText").GetComponent<Text>().fontSize = 35;
            GameObject.Find("transitionText").GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
        }
        GameObject.Find("MenuManager").GetComponent<MenuManager>().DisplayLoadingText(conn);
        if (GameObject.Find("BackToMenuButton"))
        {
            GameObject.Find("BackToMenuButton").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("BackToMenuButton").GetComponent<Button>().enabled = true;
            GameObject.Find("BackToMenuButton").GetComponent<Button>().onClick.AddListener(() => ShowMainMenu());
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.isMasterClient)
        {
            LoadLevel();
        }
    }
    //If joining a match failed, create one
    void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 2 }, null);
    }

    //When the main menu is loaded add the listeners for each button
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Mainmenu
        if (scene.buildIndex == 0)
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            GameObject.Find("PlayAsGuestButton").GetComponent<Button>().onClick.AddListener((() => JoinGame()));
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("PlayAsGuestButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("facebookLoginButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<FacebookManager>().FBButtonClick()));
            GameObject.Find("InstructionsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("LeaderboardsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("LBackToMenuButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("LoginBackToMenuButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("OptionsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("InstructionsOKButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("OptionsOKButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("BackToMenuButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound()));
            GameObject.Find("VolumeSlider").GetComponent<Slider>().onValueChanged.AddListener(GameObject.Find("MenuManager").GetComponent<MenuManager>().OnVolumeSliderChanged);
            GameObject.Find("SoundOFF").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().TurnOnSound()));
            GameObject.Find("SoundON").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().TurnOffSound()));
            /*
            if (GameObject.Find("ExitButton"))
            {
                GameObject.Find("ExitButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().ExitGame()));
            }*/
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
        }
        //Game
        else
        {
            GameObject.Find("PopupText").GetComponent<Text>().text = "Waiting for an opponent";
            SpawnPlayer();
        }
    }

    //Reset the game
    public void LoadLevel()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void ShowMainMenu()
    {
        ConnectInUpdate = false;
        PhotonNetwork.Disconnect();
        //Make transition text fit the screen
        GameObject.Find("ConnectingMenu").GetComponent<DoozyUI.UIElement>().Hide(true);
        GameObject.Find("MainMenu").GetComponent<DoozyUI.UIElement>().Show(false);
        GameObject.Find("transitionText").GetComponent<Text>().fontSize = 56;
        GameObject.Find("transitionText").GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;

    }
    //Let the player know if the opponent disconnected
    public override void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        base.OnDisconnectedFromPhoton();
        if (GameObject.Find("EscapeMenu") != null)
        {
            GameObject.Find("EscapeMenu").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("EscapeMenuText").GetComponent<Text>().text = "Your opponent has left!";
            GameObject.Find("VolumeSlider").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("EscapeMenu").GetComponentInChildren<Button>().onClick.AddListener(() => DisconnectPlayer());
        }
    }

    void DisconnectPlayer()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
    //Spawn the player
    void SpawnPlayer()
    {
        SpawnOnNetwork(transform.position, transform.rotation);
    }

    void SpawnOnNetwork(Vector3 pos, Quaternion rot)
    {
        newPlayer = (GameObject)PhotonNetwork.Instantiate("Prefabs/Player", pos, rot, 0);
    }
}
