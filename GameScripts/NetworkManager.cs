using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;
using Photon;
using UnityEngine.SceneManagement;

public class NetworkManager : PunBehaviour
{
    public bool AutoConnect = true;
    public GameObject newPlayer;
    public bool ConnectInUpdate = false;
    public bool findAnotherMatch = false;

    void Start()
    {
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
    public void JoinGame()
    {
        AutoConnect = true;
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
        PhotonNetwork.Disconnect();
        //Make transition text fit the screen
        GameObject.Find("ConnectingMenu").GetComponent<DoozyUI.UIElement>().Hide(true);
        GameObject.Find("MainMenu").GetComponent<DoozyUI.UIElement>().Show(false);
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
        if (GameObject.Find("EscapeMenu") != null)
        {
            GameObject.Find("OpponentLeftMessage").GetComponent<Text>().text = "Your opponent has left!";
            GameObject.Find("VolumeSlider").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("Toggle").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("FindAnotherMatchButton").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("EscapeMenu").GetComponent<DoozyUI.UIElement>().Show(false);
        }
    }
}
