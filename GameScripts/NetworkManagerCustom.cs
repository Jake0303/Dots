using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class NetworkManagerCustom : NetworkManager {
	// Matchmaker related
	List<MatchDesc> m_MatchList = new List<MatchDesc>();
	bool m_MatchCreated;
	bool m_MatchJoined;
	MatchInfo m_MatchInfo;
	string m_MatchName = "";
	NetworkMatch m_NetworkMatch;

	// On the server there will be multiple connections, on the client this will only contain one ID



	void Awake()
	{
		m_NetworkMatch = gameObject.AddComponent<NetworkMatch>();
	}

	void Start()
	{
		// While testing with multiple standalone players on one machine this will need to be enabled
		Application.runInBackground = true;
	}

	void OnApplicationQuit()
	{
		NetworkTransport.Shutdown();
	}

	//Reset the game
	public void ResetLevel()
	{
		NetworkManager.singleton.ServerChangeScene ("Game");
	}

  
	public void HostOrJoinGame()
	{
        bool matchFound = false;
        GameObject.Find("MenuManager").GetComponent<MenuManager>().TransitionToLobby();
        string conn = "Connecting";
        GameObject.Find("MenuManager").GetComponent<MenuManager>().DisplayLoadingText(conn);
		m_NetworkMatch.ListMatches(0, 1, "", (response) => {
			m_MatchList = response.matches;
			//Check to see if we should join a match or host one
			foreach (var match in m_MatchList) {
				if (response.success && response.matches.Count > 0 && match.currentSize < 4)
				{
					m_NetworkMatch.JoinMatch (match.networkId, "", OnMatchJoined);
                    GameObject.Find("MenuManager").GetComponent<MenuManager>().StopAllCoroutines();
                    conn = "Joining match";
                    GameObject.Find("MenuManager").GetComponent<MenuManager>().DisplayLoadingText(conn);
                    matchFound = true;
					break;
				}
			}
            //Create match if we cant join one
            if (!matchFound)
            {
                m_NetworkMatch.CreateMatch(m_MatchName, 4, true, "", OnMatchCreate);
                GameObject.Find("MenuManager").GetComponent<MenuManager>().StopAllCoroutines();
                conn = "Waiting for players to join";
                GameObject.Find("MenuManager").GetComponent<MenuManager>().DisplayLoadingText(conn);
            }
		});
    }

    public override void OnMatchCreate(CreateMatchResponse matchInfo)
    {
        base.OnMatchCreate(matchInfo);
    }
}
