using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager {
	private const int NETWORK_PORT = 7777;

	public void ChangeLevel()
	{
		NetworkManager.singleton.ServerChangeScene ("Game");
	}

	public void StartupHost()
	{
		SetPort ();
		NetworkManager.singleton.StartHost ();
	}
	public void StopHosting()
	{
		NetworkManager.singleton.networkAddress = "";
		NetworkManager.singleton.networkPort = 0;
		NetworkManager.singleton.StopHost ();
	}

	public void StopClients()
	{
		NetworkManager.singleton.networkAddress = "";
		NetworkManager.singleton.networkPort = 0;
		NetworkManager.singleton.StopClient ();
	}

	public void JoinGame()
	{
		SetIPAddress ();
		SetPort ();
		NetworkManager.singleton.StartClient ();
	}

	void SetIPAddress()
	{
		string ipAddress = "localhost";
		NetworkManager.singleton.networkAddress = ipAddress;
	}

	void SetPort()
	{
		NetworkManager.singleton.networkPort = NETWORK_PORT;
	}

	void OnLevelWasLoaded(int level)
	{
		if (level == 0) {
			SetupMenuSceneButtons ();
		} else {
			SetupOtherSceneButtons ();
		}
	}
	//Remove listenenrs when switching between scenes or else buttons will keep adding listeners
	void SetupMenuSceneButtons()
	{
		//GameObject.Find ("CreateServerButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("CreateServerButton").GetComponent<Button> ().onClick.AddListener (StartupHost);

		//GameObject.Find ("JoinButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("JoinButton").GetComponent<Button> ().onClick.AddListener (JoinGame);
	}

	void SetupOtherSceneButtons()
	{

	}
}
