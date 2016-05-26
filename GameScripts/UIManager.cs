﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour
{
    private GameObject EscapeMenu;
    public Coroutine routine;
    // On Start show the Waiting for Player text
    void Start()
    {
        StartCoroutine(DynamicPeriods());
        GameObject.Find("GameManager").GetComponent<GameState>().gameState = GameState.State.Waiting;
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
        GetComponent<PlayerID>().playerScore = 0;
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
        //Start game
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
                                player.GetComponent<PlayerID>().playerScore = 0;
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
    public void SetPlayerName(InputField tempField, GameObject panel, Text errorMsg)
    {
        bool aError = false;
        //If there is an error display a error message
        if (tempField.GetComponent<InputField>().text == "")
        {
            errorMsg.text = "Username cannot be blank";
            aError = true;
            return;
        }
        var names = GameObject.FindGameObjectsWithTag("NameText");
        foreach (var name in names)
        {
            if (name.GetComponent<Text>().text == tempField.GetComponent<InputField>().text)
            {
                errorMsg.text = "Someone already has that name!";
                aError = true;
                return;
            }
        }
        //If there is no error add the player name and update UI
        if (isLocalPlayer && !aError)
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
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EscapeMenu = GameObject.Find("EscapeMenu");
                EscapeMenu.GetComponentInChildren<Button>().onClick.AddListener(() => DisconnectPlayer());
                if (EscapeMenu.GetComponent<RectTransform>().localScale == new Vector3(0, 0, 0))
                {
                    EscapeMenu.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
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
                {
                    EscapeMenu.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                }
            }
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

            if (names[0] != null && names[0].GetComponent<Text>().text.Contains("Waiting"))
                names[0].GetComponent<Text>().text = origTexts[0];
            if (names[1] != null && names[1].GetComponent<Text>().text.Contains("Waiting"))
                names[1].GetComponent<Text>().text = origTexts[1];
            if (names[2] != null && names[2].GetComponent<Text>().text.Contains("Waiting"))
                names[2].GetComponent<Text>().text = origTexts[2];
            if (names[3] != null && names[3].GetComponent<Text>().text.Contains("Waiting"))
                names[3].GetComponent<Text>().text = origTexts[3];
            yield return new WaitForSeconds(0.25f);
            if (names[0] != null && names[0].GetComponent<Text>().text.Contains("Waiting"))
                names[0].GetComponent<Text>().text = origTexts[0] + period;
            if (names[1] != null && names[1].GetComponent<Text>().text.Contains("Waiting"))
                names[1].GetComponent<Text>().text = origTexts[1] + period;
            if (names[2] != null && names[2].GetComponent<Text>().text.Contains("Waiting"))
                names[2].GetComponent<Text>().text = origTexts[2] + period;
            if (names[3] != null && names[3].GetComponent<Text>().text.Contains("Waiting"))
                names[3].GetComponent<Text>().text = origTexts[3] + period;
            yield return new WaitForSeconds(0.25f);
            if (names[0] != null && names[0].GetComponent<Text>().text.Contains("Waiting"))
                names[0].GetComponent<Text>().text = origTexts[0] + period + period;
            if (names[1] != null && names[1].GetComponent<Text>().text.Contains("Waiting"))
                names[1].GetComponent<Text>().text = origTexts[1] + period + period;
            if (names[2] != null && names[2].GetComponent<Text>().text.Contains("Waiting"))
                names[2].GetComponent<Text>().text = origTexts[2] + period + period;
            if (names[3] != null && names[3].GetComponent<Text>().text.Contains("Waiting"))
                names[3].GetComponent<Text>().text = origTexts[3] + period + period;
            yield return new WaitForSeconds(0.25f);
            if (names[0] != null && names[0].GetComponent<Text>().text.Contains("Waiting"))
                names[0].GetComponent<Text>().text = origTexts[0] + period + period + period;
            if (names[1] != null && names[1].GetComponent<Text>().text.Contains("Waiting"))
                names[1].GetComponent<Text>().text = origTexts[1] + period + period + period;
            if (names[2] != null && names[2].GetComponent<Text>().text.Contains("Waiting"))
                names[2].GetComponent<Text>().text = origTexts[2] + period + period + period;
            if (names[3] != null && names[3].GetComponent<Text>().text.Contains("Waiting"))
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
        {
            //StartCoroutine(FadeOutText(1f, i));
        }

    }
    //Fade out text animation
    IEnumerator FadeOutText(float t, Text i)
    {
        yield return new WaitForSeconds(1f);
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
                        GameObject.Find("NetworkManager").GetComponent<NetworkManagerLocal>().StopClient();
                        GameObject.Find("NetworkManager").GetComponent<NetworkManagerLocal>().StopHost();
                        GameObject.Find("NetworkManager").GetComponent<NetworkManagerLocal>().StopServer();
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
    public void DisplayPopupText(string text, bool fadeOutMessage)
    {
        if (isLocalPlayer)
        {
            if (text != "")
            {
                GameObject.Find("PopupText").GetComponent<Text>().text = text;
                routine = StartCoroutine(FadeTextToFullAlpha(1f, GameObject.Find("PopupText").GetComponent<Text>(), fadeOutMessage));
            }
        }
    }

}
