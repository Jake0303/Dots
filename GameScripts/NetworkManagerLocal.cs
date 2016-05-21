using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerLocal : NetworkManager
{
    public void SetupMigrationManager(UnityEngine.Networking.NetworkMigrationManager man)
    {
        base.SetupMigrationManager(man);
    }
    public void StartupHost()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        NetworkManager.singleton.StartClient();
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
        else
        {
            SetupOtherSceneButtons();
        }
    }

    //Reset the game
    public void ResetLevel()
    {
        NetworkManager.singleton.ServerChangeScene("Game");
    }

    void SetupOtherSceneButtons()
    {
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        GameObject.Find("EscapeMenu").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        GameObject.Find("EscapeMenuText").GetComponent<Text>().text = "Your opponent has left!";
        GameObject.Find("EscapeMenu").GetComponentInChildren<Button>().onClick.AddListener(() => DisconnectPlayer());
    }
    void DisconnectPlayer()
    {
        GetComponent<NetworkManagerLocal>().StopServer();
        GetComponent<NetworkManagerLocal>().StopHost();

    }
}
