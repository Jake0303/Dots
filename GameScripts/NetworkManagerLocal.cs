using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;

public class NetworkManagerLocal : NetworkManager
{
    [SerializeField]
    private GameObject _playerPrefab = null;
    private bool isPlayer = false;
    //Setup Host Migration
    public void SetupMigrationManager(UnityEngine.Networking.NetworkMigrationManager man)
    {
        base.SetupMigrationManager(man);
        Setup();
    }
    //Create a new player object when someone has connected
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
    //Start an online match as a host
    public void StartupHost()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
    }
    //Join a match
    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        isPlayer = true;
        NetworkManager.singleton.StartClient();
    }
    //Ready the player when they join a game
    public override void OnClientConnect(NetworkConnection conn)
    {
        if (NetworkClient.active && !ClientScene.ready && isPlayer)
        {
            ClientScene.Ready(this.client.connection);

            if (ClientScene.localPlayers.Count == 0)
            {
                ClientScene.AddPlayer(0);
            }
        }
    }
    void SetIPAddress()
    {
        string ipAddress = "localhost";
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }
    //When the main menu is loaded add the listeners for each button
    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().TransitionToLobby()));
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => StartupHost()));
            GameObject.Find("OptionsButton").GetComponent<Button>().onClick.AddListener((() =>  GameObject.Find("MenuManager").GetComponent<MenuManager>().Options()));
            GameObject.Find("ExitButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().ExitGame()));
            GameObject.Find("InstructionsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().Instructions()));
        }
    }
    //Setup NetworkServer
    private void Setup()
    {
        NetworkServer.RegisterHandler(MsgType.AddPlayer, OnClientAddPlayer);
    }
    //Reset the game
    public void ResetLevel()
    {
        NetworkManager.singleton.ServerChangeScene("Game");
    }
    //Let the player know if the opponent disconnected
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        GameObject.Find("EscapeMenu").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        GameObject.Find("EscapeMenuText").GetComponent<Text>().text = "Your opponent has left!";
        GameObject.Find("EscapeMenu").GetComponentInChildren<Button>().onClick.AddListener(() => DisconnectPlayer());
    }
    //Disconnect the player
    void DisconnectPlayer()
    {
        GetComponent<NetworkManagerLocal>().StopServer();
        GetComponent<NetworkManagerLocal>().StopHost();

    }
    private void SpawnPlayer(NetworkConnection conn) // spawn a new player for the desired connection
     {
         GameObject playerObj = GameObject.Instantiate(_playerPrefab); // instantiate on server side
         NetworkServer.AddPlayerForConnection(conn, playerObj, 0); // spawn on the clients and set owner
     }
 
    private void OnClientAddPlayer(NetworkMessage netMsg)
    {
        AddPlayerMessage msg = netMsg.ReadMessage<AddPlayerMessage>();
        if (msg.playerControllerId == 0) // if you wanna check this
        {
            Debug.Log("Spawning player...");
            SpawnPlayer(netMsg.conn); // the above function
        }
    }
}
