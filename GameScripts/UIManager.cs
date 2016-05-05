﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour
{
    private GameObject EscapeMenu;
    // On Start show the Waiting for Player text
    void Start()
    {
        if (isLocalPlayer)
        {
            StartCoroutine(DynamicPeriods());
            EscapeMenu = GameObject.Find("EscapeMenu");
            GameObject.Find("EscapeMenu").SetActive(false);
            EscapeMenu.GetComponentInChildren<Button>().onClick.AddListener(() => DisconnectPlayer());
        }

    }
    //When the client has connected, populate the names of each panel for previous players
    public override void OnStartClient()
    {
        base.OnStartClient();
        var names = GameObject.FindGameObjectsWithTag("NameText");
        for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
        {
            foreach (var name in names)
            {
                if (name.name.Contains((i + 1).ToString()))
                {
                    UpdateUI(name.GetComponent<Text>(), GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i], gameObject);
                    break;
                }
            }
        }
    }
    //Add the player to the player list and update their name
    [Command]
    public void CmdAddPlayer(string val)
    {
        GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Add(val);
        GetComponent<PlayerID>().nameSet = true;
        name = val;
        GetComponent<PlayerID>().playerID = val;
        GetComponent<PlayerID>().CmdTellServerMyName(val);
        RpcAddPlayer(val);
    }
    //Update the UI to show who has joined the game
    [ClientRpc]
    public void RpcAddPlayer(string val)
    {
        var names = GameObject.FindGameObjectsWithTag("NameText");
        var players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
        {
            foreach (var name in names)
            {
                if (name.name.Contains((i + 1).ToString()))
                {
                    UpdateUI(name.GetComponent<Text>(), GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i], gameObject);
                    break;
                }
            }
        }
        //After 4 people have connected and set their name, start the game!
        //if (NetworkServer.connections.Count > 3 && !GameObject.Find("GameManager").GetComponent<GameStart>().startGame
          //  && GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count > 3)
            //TESTING FOR 1 player
            if (NetworkServer.connections.Count >= GLOBALS.NUMOFPLAYERSTOSTARTGAME && !GameObject.Find("GameManager").GetComponent<GameStart>().startGame
              && GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count >= GLOBALS.NUMOFPLAYERSTOSTARTGAME)
        {
            //Set the panels for each player
            var panels = GameObject.FindGameObjectsWithTag("Panel");
            for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
            {
            Start:
                foreach (var Apanel in panels)
                {
                    if (Apanel.name.Contains((i + 1).ToString()))
                    {
                        foreach (var player in players)
                        {
                            if (player.GetComponent<PlayerID>().playersPanel == "")
                            {
                                player.GetComponent<PlayerID>().playersPanel = Apanel.name;
                                if (isLocalPlayer)
                                {
                                    GetComponent<PlayerID>().playersPanel = Apanel.name;
                                }
                                i++;
                                goto Start;
                            }
                        }

                    }
                }
            }
            GameObject.Find("GameManager").GetComponent<GameStart>().startGame = true;
        }
    }

    //Set the playername over the server 
    public void SetPlayerName(InputField tempField, GameObject panel)
    {
        if (isLocalPlayer)
        {
            CmdAddPlayer(tempField.GetComponent<InputField>().text);
            panel.SetActive(false);
            var players = GameObject.FindGameObjectsWithTag("Player");
        }
    }
    //Update the name text
    public void UpdateUI(Text textToUpdate, string text, GameObject player)
    {
        //Update the player's UI with their name
        textToUpdate.text = text;
    }
    void Update()
    {
        //Escape menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!EscapeMenu.activeSelf)
            {
                EscapeMenu.SetActive(true);
                if (NetworkServer.connections.Count < 2)
                {
                    if (GLOBALS.ISNETWORKLOCAL)
                    {
                        EscapeMenu.GetComponentInChildren<Button>().onClick.AddListener(() => GameObject.Find("NetworkManager").GetComponent<NetworkManagerLocal>().StopServer());
                    }
                    else
                    {
                        //EscapeMenu.GetComponentInChildren<Button>().onClick.AddListener(() => GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().StopServer());
                    }
                }
            }
            else
                EscapeMenu.SetActive(false);
        }
    }
    //Dynamic period animation
    IEnumerator DynamicPeriods()
    {
        var names = GameObject.FindGameObjectsWithTag("NameText");
        string[] origTexts = new string[4];
        for (int i = 0; i < names.Length; i++)
        {
            if (isLocalPlayer)
                StartCoroutine(FadeTextToFullAlpha(1f, names[i].GetComponent<Text>(), false));
            origTexts[i] = names[i].GetComponent<Text>().text;
        }
        string period = ".";
        for (; ; )
        {
            if (NetworkServer.connections.Count > 3)
                StopAllCoroutines();

            if (names[0].GetComponent<Text>().text.Contains("Waiting"))
                names[0].GetComponent<Text>().text = origTexts[0];
            if (names[1].GetComponent<Text>().text.Contains("Waiting"))
                names[1].GetComponent<Text>().text = origTexts[1];
            if (names[2].GetComponent<Text>().text.Contains("Waiting"))
                names[2].GetComponent<Text>().text = origTexts[2];
            if (names[3].GetComponent<Text>().text.Contains("Waiting"))
                names[3].GetComponent<Text>().text = origTexts[3];
            yield return new WaitForSeconds(0.25f);
            if (names[0].GetComponent<Text>().text.Contains("Waiting"))
                names[0].GetComponent<Text>().text = origTexts[0] + period;
            if (names[1].GetComponent<Text>().text.Contains("Waiting"))
                names[1].GetComponent<Text>().text = origTexts[1] + period;
            if (names[2].GetComponent<Text>().text.Contains("Waiting"))
                names[2].GetComponent<Text>().text = origTexts[2] + period;
            if (names[3].GetComponent<Text>().text.Contains("Waiting"))
                names[3].GetComponent<Text>().text = origTexts[3] + period;
            yield return new WaitForSeconds(0.25f);
            if (names[0].GetComponent<Text>().text.Contains("Waiting"))
                names[0].GetComponent<Text>().text = origTexts[0] + period + period;
            if (names[1].GetComponent<Text>().text.Contains("Waiting"))
                names[1].GetComponent<Text>().text = origTexts[1] + period + period;
            if (names[2].GetComponent<Text>().text.Contains("Waiting"))
                names[2].GetComponent<Text>().text = origTexts[2] + period + period;
            if (names[3].GetComponent<Text>().text.Contains("Waiting"))
                names[3].GetComponent<Text>().text = origTexts[3] + period + period;
            yield return new WaitForSeconds(0.25f);
            if (names[0].GetComponent<Text>().text.Contains("Waiting"))
                names[0].GetComponent<Text>().text = origTexts[0] + period + period + period;
            if (names[1].GetComponent<Text>().text.Contains("Waiting"))
                names[1].GetComponent<Text>().text = origTexts[1] + period + period + period;
            if (names[2].GetComponent<Text>().text.Contains("Waiting"))
                names[2].GetComponent<Text>().text = origTexts[2] + period + period + period;
            if (names[3].GetComponent<Text>().text.Contains("Waiting"))
                names[3].GetComponent<Text>().text = origTexts[3] + period + period + period;
            yield return new WaitForSeconds(0.25f);
        }
    }

    //Fade in text animation
    public IEnumerator FadeTextToFullAlpha(float t, Text i, bool fadeOut)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            if (i != null)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            }
            yield return null;
        }
        if (fadeOut)
            StartCoroutine(FadeOutText(1f, i));
    }
    //Fade out text animation
    IEnumerator FadeOutText(float t, Text i)
    {
        yield return new WaitForSeconds(2f);
        i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a);
        while (i.color.a > 0f)
        {
            if (i != null)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            }
            yield return null;
        }

    }

    void DisconnectPlayer()
    {
        if (isLocalPlayer)
        {
            //Remove player from player list
            for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
            {
                if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == GetComponent<PlayerID>().playerID)
                {
                    CmdRemovePlayerFromList(i);
                    break;
                }
            }
        }
    }

    [Command]
    public void CmdRemovePlayerFromList(int indexToRemove)
    {
        var names = GameObject.FindGameObjectsWithTag("NameText");
        GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.RemoveAt(indexToRemove);
        foreach (var name in names)
        {
            if (name.name.Contains((indexToRemove + 1).ToString()))
            {
                name.GetComponent<Text>().text = "Waiting for player";
                break;
            }
        }
        RpcRemovePlayerFromList(indexToRemove);
    }
    [ClientRpc]
    void RpcRemovePlayerFromList(int indexToRemove)
    {
        var names = GameObject.FindGameObjectsWithTag("NameText");
        foreach (var name in names)
        {
            if (name.name.Contains((indexToRemove + 1).ToString()))
            {
                name.GetComponent<Text>().text = "Waiting for player";
                //Disconnect player 
                if (isLocalPlayer)
                {
                    if (GLOBALS.ISNETWORKLOCAL)
                    {
                        GameObject.Find("NetworkManager").GetComponent<NetworkManagerLocal>().StopHost();
                    }
                    else
                    {
                        GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().m_NetworkMatch.DropConnection(GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().matchInfo.networkId, GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().matchInfo.nodeId, GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().OnMatchDestroyed);
                        GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>().StopHost();   
                    }
                    break;
                }
            }
        }
    }
    //Display popup text for the player
    public void DisplayPopupText(string text)
    {
        GameObject.Find("PopupText").GetComponent<Text>().text = text;
        if(text != "")
            StartCoroutine(FadeTextToFullAlpha(1f, GameObject.Find("PopupText").GetComponent<Text>(), true));
    }

}
