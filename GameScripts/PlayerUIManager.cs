using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using Photon;
using UnityEngine.SceneManagement;
using System;

public class PlayerUIManager : PunBehaviour
{
    private GameObject EscapeMenu;
    public Coroutine routine;
    private GameObject[] panels, players, names;
    // On Start show the Waiting for Player text
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        names = GameObject.FindGameObjectsWithTag("NameText");
        panels = GameObject.FindGameObjectsWithTag("Panel");
        GameObject.Find("Disconnect").GetComponent<Button>().onClick.AddListener(() => OnLeaveButtonClick());
        GameObject.Find("YesButton").GetComponent<Button>().onClick.AddListener(() => ForfeitPlayer());
        GameObject.Find("VolumeSlider").GetComponent<Slider>().onValueChanged.AddListener(OnVolumeSliderChanged);
        GameObject.Find("GameManager").GetComponent<GameState>().gameState = GameState.State.Waiting;
        if (IsTablet())
        {
            GameObject.Find("PopupText").GetComponent<Text>().fontSize = 22;
            GameObject.Find("Camera").GetComponent<Camera>().fieldOfView = 71;
        }
        else if ((Screen.orientation == ScreenOrientation.Portrait
            || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            && (Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.IPhonePlayer))
        {
            GameObject.Find("Camera").GetComponent<Camera>().fieldOfView = 90;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameObject.Find("Camera").GetComponent<Camera>().fieldOfView = 60;
        }
        GameObject.Find("OpponentLeftMessage").GetComponent<Text>().text = "";
    }
    public static float GetScreenSize()
    {
        float curDPI = Screen.dpi;
        if (curDPI == 0f) // Win, OSX or WP8
            curDPI = 96f;

        return ((Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height)) / curDPI);
    }
    //TODO not working for iPhone 5s
    public static bool IsTablet()
    {
        return (GetScreenSize() > 6f);
        /*return (GetScreenSize() >= 6f && (Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.IPhonePlayer));*/
    }
    //When the client has connected, populate the names of each panel for previous players
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        players = GameObject.FindGameObjectsWithTag("Player");
        names = GameObject.FindGameObjectsWithTag("NameText");
        panels = GameObject.FindGameObjectsWithTag("Panel");
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
            foreach (var stats in GameObject.FindGameObjectsWithTag("StatsText"))
            {
                if (stats.name.Contains((i + 1).ToString()))
                {
                    //Update UI with Wins and Losses
                    stats.GetComponent<Text>().text = GetComponent<PlayerID>().playersWins + " W "
                        + GetComponent<PlayerID>().playerLosses + " L ";
                }
            }
        }
        DoozyUI.UIManager.EnableBackButton();
    }
    //Add the player to the player list and update their name
    [PunRPC]
    public void CmdAddPlayer(string val)
    {
        GameObject.Find("EventPanelEffect").GetComponent<ParticleSystem>().Stop();
        GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Add(val);
        GetComponent<PlayerID>().nameSet = true;
        gameObject.name = val;
        GetComponent<PlayerID>().playerID = val;
        GetComponent<PlayerID>().playerScore = 0;
        GetComponent<PlayerID>().CmdTellServerMyName(val);
        players = GameObject.FindGameObjectsWithTag("Player");
        names = GameObject.FindGameObjectsWithTag("NameText");
        panels = GameObject.FindGameObjectsWithTag("Panel");
        foreach (var player in players)
        {
            for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
            {
                if (player.name == GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i])
                {
                    foreach (var stats in GameObject.FindGameObjectsWithTag("StatsText"))
                    {
                        if (stats.name.Contains((i + 1).ToString()))
                        {
                            //Update UI with Wins and Losses
                            stats.GetComponent<Text>().text = player.GetComponent<PlayerID>().playersWins + " W "
                                + player.GetComponent<PlayerID>().playerLosses + " L ";
                        }
                    }
                    Start:
                    foreach (var aName in names)
                    {
                        if (aName.name.Contains((i + 1).ToString()))
                        {
                            UpdateUI(aName.GetComponent<Text>(), GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i], gameObject);
                            break;
                        }
                    }
                    if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == player.GetComponent<PlayerID>().playerID)
                    {
                        foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
                        {
                            if (scores.name.Contains((i + 1).ToString()))
                            {
                                //Update UI with score
                                scores.GetComponent<Text>().text = "Score: " + player.GetComponent<PlayerID>().playerScore.ToString();
                            }
                        }
                        for (int j = 0; j < panels.Length; j++)
                        {
                            if (panels[j].name.Contains((i + 1).ToString())
                                && panels[j].GetComponent<PlayerPanel>().owner == ""
                                && player.GetComponent<PlayerID>().playersPanel == "")
                            {
                                panels[j].GetComponent<PlayerPanel>().owner = player.GetComponent<PlayerID>().playerID;
                                player.GetComponent<PlayerID>().playersPanel = panels[j].name;
                                player.GetComponent<PlayerID>().playerScore = 0;
                                panels[j].GetComponent<DoozyUI.UIElement>().Show(false);
                                goto Start;
                            }
                        }
                    }
                }
            }
        }
        //Start game
        if (!GameObject.Find("GameManager").GetComponent<GameStart>().startGame
          && GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count >= GLOBALS.NUMOFPLAYERSTOSTARTGAME)
        {
            GameObject.Find("GameManager").GetComponent<GameState>().gameState = GameState.State.InProgress;
            GameObject.Find("GameManager").GetComponent<GameStart>().startGame = true;
            GameObject.Find("UI").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("EventPanel").transform.localPosition = new Vector3(-2500f, 0f, 0f);
            GameObject.Find("PlayAgainMenu").transform.localPosition = new Vector3(0f, -2500f, 0f);
            GameObject.Find("LoadingGif").transform.localScale = new Vector3(0, 0, 0);
        }
    }


    public void closeEscapeMenu()
    {
        if (GameObject.Find("OpponentLeftMessage").GetComponent<Text>().text == "")
        {
            EscapeMenu = GameObject.Find("EscapeMenu");
            GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
            EscapeMenu.GetComponent<DoozyUI.UIElement>().Hide(false);
        }
    }

    public void openEscapeMenu()
    {
        GameObject.Find("EscapeMenu").GetComponent<Canvas>().overrideSorting = true;
        GameObject.Find("EscapeMenu").GetComponent<Canvas>().sortingOrder = 1;
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
        EscapeMenu = GameObject.Find("EscapeMenu");
        GameObject.Find("VolumeSlider").GetComponent<Slider>().value = GLOBALS.Volume;
        GameObject.Find("VolumeLevel").GetComponent<Text>().text = GLOBALS.Volume.ToString();
        if (!EscapeMenu.GetComponent<DoozyUI.UIElement>().isVisible)
            EscapeMenu.GetComponent<DoozyUI.UIElement>().Show(false);
        else
            closeEscapeMenu();
    }

    public void fbAuthenticated(string name)
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        GameObject.Find("PopupText").transform.localScale = new Vector3(1, 1, 1);
        GameObject.Find("LoadingGif").transform.localScale = new Vector3(1, 1, 1);
        photonView.RPC("CmdAddPlayer", PhotonTargets.AllBuffered, name);
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
        if (photonView.isMine)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                openEscapeMenu();
            }
        }
    }
    //Update the volume when the slider has changed
    public void OnVolumeSliderChanged(float value)
    {
        GLOBALS.Volume = value;
        GameObject.Find("VolumeLevel").GetComponent<Text>().text = GLOBALS.Volume.ToString();
        GameObject.Find("AudioManager").GetComponent<Sound>().bgMusic.volume = (GLOBALS.Volume / 50);
        GameObject.Find("AudioManager").GetComponent<Sound>().PlaySliderSound();
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


    public void OnLeaveButtonClick()
    {
        if (GameObject.Find("GameManager").GetComponent<GameStart>().startGame
            || PhotonNetwork.playerList.Length <= 1
            || GameObject.Find("OpponentLeftMessage").GetComponent<Text>().text != "")
            DisconnectPlayer();
        else
            GameObject.Find("LeaveConfirmation").GetComponent<DoozyUI.UIElement>().Show(false);
    }
    //TODO: Somehow player is getting set to inactive and thus coroutine can't run :(
    public void ForfeitPlayer()
    {
        if (photonView.isMine)
        {
            GetComponent<PlayerID>().playerLosses += 1;
            PlayerPrefs.SetInt("Wins", GetComponent<PlayerID>().playersWins);
            PlayerPrefs.SetInt("Losses", GetComponent<PlayerID>().playerLosses);
            if (GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken != "")
                GameObject.Find("MenuManager").GetComponent<FacebookManager>().StartCoroutine(LeaderbordController.PostScores(GetComponent<PlayerID>().playerID, GetComponent<PlayerID>().playersWins, GetComponent<PlayerID>().playerLosses, GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken, true));
            else
                GameObject.Find("MenuManager").GetComponent<MenuManager>().StartCoroutine(LeaderbordController.PostScores(GetComponent<PlayerID>().guestToken, GetComponent<PlayerID>().playerID, GetComponent<PlayerID>().playersWins, GetComponent<PlayerID>().playerLosses, true));
        }
    }

    public void DisconnectPlayer()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnectedFromPhoton()
    {
        base.OnDisconnectedFromPhoton();
        PhotonNetwork.Destroy(photonView);
        SceneManager.LoadScene(0);
    }


    [PunRPC]
    public void CmdRemovePlayerFromList(int indexToRemove)
    {
        var names = GameObject.FindGameObjectsWithTag("NameText");
        GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.RemoveAt(indexToRemove);
        foreach (var name in names)
        {
            if (name.name.Contains((indexToRemove + 1).ToString()))
            {
                name.GetComponent<Text>().text = "Waiting for an opponent";
                break;
            }
        }
    }
    //Display popup text for the player
    public void DisplayPopupText(string text, bool fadeOutMessage)
    {
        if (photonView.isMine)
        {
            if (text != "")
            {
                GameObject.Find("PopupText").GetComponent<Text>().text = text;
                routine = StartCoroutine(FadeTextToFullAlpha(1f, GameObject.Find("PopupText").GetComponent<Text>(), fadeOutMessage));
            }
        }
    }
    //Display popup box for the player
    public void DisplayPopupBox(string text)
    {
        GameObject.Find("EventPanelEffect").GetComponent<ParticleSystem>().Emit(128);
        GameObject.Find("EventPanelEffect").GetComponent<ParticleSystem>().Simulate(1.0f);
        GameObject.Find("EventPanelEffect").GetComponent<DoozyUI.UIEffect>().playOnAwake = true;
        GameObject.Find("EventPanelEffect").GetComponent<ParticleSystem>().Play();
        if (photonView.isMine)
        {
            GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().moveOut.delay = 1.5f;
            GameObject.Find("EventText").GetComponent<Text>().text = text;
            GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Show(false);
        }
    }
    //When the in game event panel is finished its anim hide the tapGif
    public void OnEventPanelFinish()
    {
        GameObject.Find("TapGif").GetComponent<Image>().enabled = false;
        GameObject.Find("TapGif").GetComponent<LoadingGif>().enabled = false;
    }

    //OnColorBlindAssistCheckbox Changed
    public void OnColorBlindCheckboxChanged(bool val)
    {
        panels = GameObject.FindGameObjectsWithTag("Panel");
        GLOBALS.ColorBlindAssist = val;
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (GameObject.Find("AudioManager") != null)
            GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();

        if (val)
        {
            GetComponent<PlayerColor>().colors[1] = GLOBALS.DarkBlue;
            GetComponent<PlayerColor>().colors[2] = GLOBALS.DarkYellow;
            foreach (var square in GameObject.FindGameObjectsWithTag("CenterSquare"))
            {
                if (square.GetComponentInChildren<Renderer>().material.HasProperty("_MKGlowColor"))
                {
                    if (square.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == GLOBALS.DarkGreen)
                    {
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GLOBALS.DarkBlue);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GLOBALS.DarkBlue);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GLOBALS.DarkBlue);
                    }
                    if (square.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == GLOBALS.DarkRed)
                    {
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GLOBALS.DarkYellow);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GLOBALS.DarkYellow);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GLOBALS.DarkYellow);
                    }
                }
            }
            foreach (var line in GameObject.FindGameObjectsWithTag("Line"))
            {
                if (line.GetComponentInChildren<Renderer>().material.HasProperty("_MKGlowColor"))
                {
                    if (line.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == GLOBALS.DarkGreen)
                    {
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GLOBALS.DarkBlue);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GLOBALS.DarkBlue);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GLOBALS.DarkBlue);
                    }
                    if (line.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == GLOBALS.DarkRed)
                    {
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GLOBALS.DarkYellow);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GLOBALS.DarkYellow);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GLOBALS.DarkYellow);
                    }
                }
            }
            foreach (var player in players)
            {
                if (player.GetComponent<PlayerColor>().playerColor == GLOBALS.DarkGreen)
                {
                    player.GetComponent<PlayerColor>().playerColor = GLOBALS.DarkBlue;
                }
                if (player.GetComponent<PlayerColor>().playerColor == GLOBALS.DarkRed)
                {
                    player.GetComponent<PlayerColor>().playerColor = GLOBALS.DarkYellow;
                }
            }
            foreach (GameObject panel in panels)
            {
                if (panel.GetComponent<Image>().color == GLOBALS.DarkGreen)
                {
                    panel.GetComponent<Image>().color = GLOBALS.DarkBlue;
                }
                if (panel.GetComponent<Image>().color == GLOBALS.DarkRed)
                {
                    panel.GetComponent<Image>().color = GLOBALS.DarkYellow;
                }
            }
        }
        else
        {
            GetComponent<PlayerColor>().colors[1] = Color.green;
            GetComponent<PlayerColor>().colors[2] = GLOBALS.DarkRed;
            foreach (var square in GameObject.FindGameObjectsWithTag("CenterSquare"))
            {
                if (square.GetComponentInChildren<Renderer>().material.HasProperty("_MKGlowColor"))
                {
                    if (square.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == GLOBALS.DarkBlue)
                    {
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GLOBALS.DarkGreen);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GLOBALS.DarkGreen);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GLOBALS.DarkGreen);
                    }
                    if (square.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == GLOBALS.DarkYellow)
                    {
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GLOBALS.DarkRed);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GLOBALS.DarkRed);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GLOBALS.DarkRed);
                    }
                }
            }
            foreach (var line in GameObject.FindGameObjectsWithTag("Line"))
            {
                if (line.GetComponentInChildren<Renderer>().material.HasProperty("_MKGlowColor"))
                {
                    if (line.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == GLOBALS.DarkBlue)
                    {
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GLOBALS.DarkGreen);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GLOBALS.DarkGreen);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GLOBALS.DarkGreen);
                    }
                    if (line.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == GLOBALS.DarkYellow)
                    {
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GLOBALS.DarkRed);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GLOBALS.DarkRed);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GLOBALS.DarkRed);
                    }
                }
            }
            foreach (var player in players)
            {
                if (player.GetComponent<PlayerColor>().playerColor == GLOBALS.DarkBlue)
                {
                    player.GetComponent<PlayerColor>().playerColor = GLOBALS.DarkGreen;
                }
                if (player.GetComponent<PlayerColor>().playerColor == GLOBALS.DarkYellow)
                {
                    player.GetComponent<PlayerColor>().playerColor = GLOBALS.DarkRed;
                }
            }
            foreach (GameObject panel in panels)
            {
                if (panel.GetComponent<Image>().color == GLOBALS.DarkBlue)
                {
                    panel.GetComponent<Image>().color = GLOBALS.DarkGreen;
                }
                if (panel.GetComponent<Image>().color == GLOBALS.DarkYellow)
                {
                    panel.GetComponent<Image>().color = GLOBALS.DarkRed;
                }
            }
        }

    }

}
