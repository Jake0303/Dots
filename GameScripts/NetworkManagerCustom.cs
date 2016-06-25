using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class NetworkManagerCustom : NetworkManager {
	// Matchmaker related
	public List<MatchDesc> m_MatchList = new List<MatchDesc>();
	bool m_MatchCreated;
	bool m_MatchJoined;
	public MatchInfo m_MatchInfo;
	string m_MatchName = "Test";
	public NetworkMatch m_NetworkMatch;
    public List<string> playerNames = new List<string>();
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
        m_NetworkMatch.ListMatches(0, 1, "", (response) =>
        {
            m_MatchList = response.matches;
            //Check to see if we should join a match or host one
            foreach (var match in m_MatchList)
            {
                m_NetworkMatch.DestroyMatch(match.networkId, OnMatchDestroyed);
            }
        });
		NetworkTransport.Shutdown();
	}

	//Reset the game
	public void ResetLevel()
	{
		NetworkManager.singleton.ServerChangeScene ("Game");
	}
    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().TransitionToLobby()));
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener((() => HostOrJoinGame()));
            GameObject.Find("OptionsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().Options()));
            GameObject.Find("ExitButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().ExitGame()));
            GameObject.Find("InstructionsButton").GetComponent<Button>().onClick.AddListener((() => GameObject.Find("MenuManager").GetComponent<MenuManager>().Instructions()));
        }
    }
  
	public void HostOrJoinGame()
	{
        bool matchFound = false;
        GameObject.Find("MenuManager").GetComponent<MenuManager>().TransitionToLobby();
        string conn = "Connecting";
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
    public void OnMatchDestroyed(BasicResponse destroyMatchResponse)
    {
        Debug.Log("Cancel success: " + destroyMatchResponse.success);
    }
}
