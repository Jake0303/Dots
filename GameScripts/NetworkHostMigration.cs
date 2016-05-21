using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkHostMigration : NetworkMigrationManager
{
    void Start()
    {
        GetComponent<NetworkManager>().SetupMigrationManager(this);
    }
    protected override void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange)
    {
        base.OnClientDisconnectedFromHost(conn, out sceneChange);
        GameObject.Find("EscapeMenu").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        GameObject.Find("EscapeMenuText").GetComponent<Text>().text = "Your opponent has left!";
    }
    protected override void OnServerHostShutdown()
    {
        base.OnServerHostShutdown();
        GameObject.Find("EscapeMenu").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        GameObject.Find("EscapeMenuText").GetComponent<Text>().text = "Your opponent has left!";
    }
}
