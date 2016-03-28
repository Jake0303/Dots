using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class NetworkManagerCustom : NetworkManager {
	private const int NETWORK_PORT = 7777;

	// Matchmaker related
	List<MatchDesc> m_MatchList = new List<MatchDesc>();
	bool m_MatchCreated;
	bool m_MatchJoined;
	MatchInfo m_MatchInfo;
	string m_MatchName = "NewRoom";
	NetworkMatch m_NetworkMatch;

	// Connection/communication related
	int m_HostId = -1;
	// On the server there will be multiple connections, on the client this will only contain one ID
	List<int> m_ConnectionIds = new List<int>();
	byte[] m_ReceiveBuffer;
	string m_NetworkMessage = "Hello world";
	string m_LastReceivedMessage = "";
	NetworkWriter m_Writer;
	NetworkReader m_Reader;
	bool m_ConnectionEstablished;

	const int k_ServerPort = 25000;
	const int k_MaxMessageSize = 65535;

	void Awake()
	{
		m_NetworkMatch = gameObject.AddComponent<NetworkMatch>();
	}

	void Start()
	{
		m_ReceiveBuffer = new byte[k_MaxMessageSize];
		m_Writer = new NetworkWriter();
		// While testing with multiple standalone players on one machine this will need to be enabled
		Application.runInBackground = true;
	}

	void OnApplicationQuit()
	{
		NetworkTransport.Shutdown();
	}


	public void ChangeLevel()
	{
		NetworkManager.singleton.ServerChangeScene ("Game");
	}

	public void HostOrJoinGame()
	{
		m_NetworkMatch.ListMatches(0, 1, "", (response) => {
			m_MatchList = response.matches;
			//Check to see if we should join a match or host one
			foreach (var match in m_MatchList) {
				if (response.success && response.matches.Count > 0 && match.currentSize < 4)
				{
					m_NetworkMatch.JoinMatch (match.networkId, "", OnMatchJoined);
					return;
				}
			}
			//Create match if we cant join one
			m_NetworkMatch.CreateMatch(m_MatchName, 4, true, "", OnMatchCreate);
		});


	}


	void OnLevelWasLoaded(int level)
	{
		if (level == 0) {
			SetupMenuSceneButtons ();
		}
	}
	//Remove listeners when switching between scenes or else buttons will keep adding listeners
	void SetupMenuSceneButtons()
	{

	}
}
