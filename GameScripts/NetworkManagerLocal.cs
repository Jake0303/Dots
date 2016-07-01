using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;
using Photon;

public class NetworkManagerLocal : PunBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab = null;
    public bool AutoConnect = true;

    public byte Version = 1;
    private bool ConnectInUpdate = true;
    private PhotonView photonView;

    void Start()
    {
        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
        PhotonNetwork.automaticallySyncScene = true;
        photonView = this.GetComponent<PhotonView>();

    }
    void Update()
    {
        if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            ConnectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        }
    }
    //Join lobby
    public void JoinGame()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        //Tries to join any random game:
        PhotonNetwork.JoinRandomRoom();
        //Fails if there are no matching games: OnPhotonRandomJoinFailed
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if(photonView.isMine)
            LoadLevel();
    }
    //If joining a match failed, create one
    void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 2 }, null);
    }

    //When the main menu is loaded add the listeners for each button
    void OnLevelWasLoaded(int level)
    {
        //Mainmenu
        if (level == 0)
        {
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().TransitionToLobby()));
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => JoinGame()));
            GameObject.Find("OptionsButton").GetComponent<Button>().onClick.AddListener((() =>  GameObject.Find("MenuManager").GetComponent<MenuManager>().Options()));
            GameObject.Find("ExitButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().ExitGame()));
            GameObject.Find("InstructionsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().Instructions()));
        }
        //Game
        else if(level == 1)
        {
            SpawnPlayer();
            GameObject.Find("PopupText").GetComponent<Text>().text = "Waiting for players";
        }
    }

    //Reset the game
    public void LoadLevel()
    {
        PhotonNetwork.LoadLevel("Game");
    }
    //Let the player know if the opponent disconnected
    public override void OnDisconnectedFromPhoton()
    {
 	    base.OnDisconnectedFromPhoton();
        GameObject.Find("EscapeMenu").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        GameObject.Find("EscapeMenuText").GetComponent<Text>().text = "Your opponent has left!";
        //TODO add disconnect popup
        //GameObject.Find("EscapeMenu").GetComponentInChildren<Button>().onClick.AddListener(() => DisconnectPlayer());
    }
    //Spawn the player
    void SpawnPlayer()
    {
        // Manually allocate PhotonViewID
        int id1 = PhotonNetwork.AllocateViewID();

        SpawnOnNetwork(transform.position, transform.rotation, id1, PhotonNetwork.player);
    }

    void SpawnOnNetwork(Vector3 pos, Quaternion rot, int id1, PhotonPlayer np)
    {
        GameObject newPlayer = PhotonNetwork.Instantiate("Prefabs/Player", pos, rot,0) as GameObject;
    }
}
