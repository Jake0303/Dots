using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerLocal : NetworkManager
{

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
    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {}

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
}
