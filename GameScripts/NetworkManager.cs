using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;
using Photon;
using UnityEngine.SceneManagement;
using DoozyUI;
using UnityEngine.EventSystems;

public class NetworkManager : PunBehaviour
{
    public bool AutoConnect = true;
    public GameObject newPlayer;
    public bool ConnectInUpdate = false;
    public bool findAnotherMatch = false;

    void Start()
    {
        PhotonHandler.BestRegionCodeInPreferences = CloudRegionCode.none;
        AutoConnect = true;
        ConnectInUpdate = false;
        newPlayer = null;
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
    public void JoinGame(bool fbButtonClicked)
    {
        if (GameObject.Find("BackToMenuButton"))
        {
            GameObject.Find("BackToMenuButton").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("BackToMenuButton").GetComponent<Button>().enabled = true;
            GameObject.Find("BackToMenuButton").GetComponent<Button>().onClick.AddListener(() => ShowMainMenu());
        }
        GameObject.Find("PlayAsGuestButton").GetComponent<UIButton>().useOnClickAnimations = true;
        GameObject.Find("PlayAsGuestButton").GetComponent<UIButton>().StartOnClickAnimations();
        bool fbInfoFound = false;
        bool infoFound = false;
        if (!fbButtonClicked)
            GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken = "";
        if (LeaderbordController.data.list != null)
        {
            foreach (var aData in LeaderbordController.data.list)
            {
                if (GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken != ""
                    && aData["FBUserID"].str == GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken
                    && fbButtonClicked)
                {
                    if (aData["Username"].str != "")
                    {
                        fbInfoFound = true;
                        break;
                    }
                }
                else if (PlayerPrefs.GetString("GuestID") != ""
                    && aData["GuestID"].str == PlayerPrefs.GetString("GuestID")
                    && !fbButtonClicked)
                {
                    infoFound = true;
                    break;
                }
            }
        }
        if ((!fbInfoFound && fbButtonClicked)
            || (!infoFound && !fbButtonClicked))
        {
            GameObject.Find("EnterNickMenu").GetComponent<UIElement>().Show(false);
            if (Application.isMobilePlatform
                && Screen.orientation == ScreenOrientation.Portrait)
            {
                GameObject.Find("Notification").transform.localPosition = new Vector3(0, -550f, 0);
            }
            if (!fbButtonClicked)
                GameObject.Find("NotificationMenu").GetComponent<UIElement>().Show(false);
        }
        else
        {
            GameObject.Find("ConnectingMenu").GetComponent<UIElement>().Show(false);
            AutoConnect = true;
            ConnectInUpdate = true;
        }
        GameObject.Find("LoginMenu").GetComponent<UIElement>().Hide(false);
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        string conn = "Joining match";
        GameObject.Find("MenuManager").GetComponent<MenuManager>().DisplayLoadingText(conn);
        //Tries to join any random game:
        PhotonNetwork.JoinRandomRoom();
        //Fails if there are no matching games: OnPhotonRandomJoinFailed
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
            GameObject.Find("ConnectingGif").transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        base.OnCustomAuthenticationFailed(debugMessage);
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
            GameObject.Find("ConnectingGif").transform.localScale = new Vector3(0, 0, 0);
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
            GameObject.Find("ConnectingGif").transform.localScale = new Vector3(0, 0, 0);
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
    public void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Mainmenu
        if (scene.buildIndex == 0)
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            newPlayer = null;
            ConnectInUpdate = false;
        }
        //Game
        else if (scene.buildIndex == 1)
        {
            ConnectInUpdate = false;
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
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }

    public void OnConnectingMenuAnimFinish()
    {
        //Make transition text fit the screen
        GameObject.Find("transitionText").GetComponent<Text>().fontSize = 56;
        GameObject.Find("transitionText").GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        GameObject.Find("ConnectingGif").transform.localScale = new Vector3(1, 1, 1);
    }

    //Spawn the player on the network
    void SpawnPlayer()
    {
        if (newPlayer == null)
            newPlayer = (GameObject)PhotonNetwork.Instantiate("Prefabs/Player", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
        ConnectInUpdate = false;
    }


    //Let the player know if the opponent disconnected
    public override void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        base.OnDisconnectedFromPhoton();
        if (GameObject.Find("EscapeMenu") != null && !GameObject.Find("GameManager").GetComponent<GameOver>().gameDone)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var aPlayer in players)
            {
                aPlayer.GetComponent<PlayerID>().playersWins += 1;
                PlayerPrefs.SetInt("Wins", aPlayer.GetComponent<PlayerID>().playersWins);
                PlayerPrefs.SetInt("Losses", aPlayer.GetComponent<PlayerID>().playerLosses);
                if (GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken != "")
                    GameObject.Find("MenuManager").GetComponent<FacebookManager>().StartCoroutine(LeaderbordController.PostScores(aPlayer.GetComponent<PlayerID>().playerID, aPlayer.GetComponent<PlayerID>().playersWins, GetComponent<PlayerID>().playerLosses, GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken, false));
                else
                    GameObject.Find("MenuManager").GetComponent<MenuManager>().StartCoroutine(LeaderbordController.PostScores(aPlayer.GetComponent<PlayerID>().guestToken, aPlayer.GetComponent<PlayerID>().playerID, aPlayer.GetComponent<PlayerID>().playersWins, aPlayer.GetComponent<PlayerID>().playerLosses, false));
                //Update UI with Wins and Losses
                GameObject.Find(aPlayer.GetComponent<PlayerID>().playersPanel).GetComponentsInChildren<Text>()[3].text = aPlayer.GetComponent<PlayerID>().playersWins + " W "
                    + aPlayer.GetComponent<PlayerID>().playerLosses + " L ";
                break;
            }

            GameObject.Find("OpponentLeftMessage").GetComponent<Text>().text = "Your opponent has left! \nYou win!";
            GameObject.Find("VolumeSlider").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("ColorBlindAssistCheckbox").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("FindAnotherMatchButton").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("EscapeMenu").GetComponent<DoozyUI.UIElement>().Show(false);
        }
    }
}
