using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using Photon;
using UnityEngine.SceneManagement;
using System;

public class UIManager : PunBehaviour
{
    private GameObject EscapeMenu;
    public Coroutine routine;
    // On Start show the Waiting for Player text
    void Start()
    {
        GameObject.Find("VolumeSlider").GetComponent<Slider>().onValueChanged.AddListener(OnVolumeSliderChanged);
        GameObject.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(OnColorBlindCheckboxChanged);
        GameObject.Find("GameManager").GetComponent<GameState>().gameState = GameState.State.Waiting;
        if ((Screen.orientation == ScreenOrientation.Portrait
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
    }
    //When the client has connected, populate the names of each panel for previous players
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        var players = GameObject.FindGameObjectsWithTag("Player");
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
        GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Add(val);
        GetComponent<PlayerID>().nameSet = true;
        name = val;
        GetComponent<PlayerID>().playerID = val;
        //Get Player Wins & Losses
        foreach (var aData in LeaderbordController.data.list)
        {
            if (aData["FBUserID"].str == GameObject.Find("MenuManager").GetComponent<FacebookManager>().accessToken
                && aData["Username"].str == GetComponent<PlayerID>().playerID)
            {
                GetComponent<PlayerID>().playersWins = Int32.Parse(aData["Wins"].str);
                GetComponent<PlayerID>().playerLosses = Int32.Parse(aData["Losses"].str);
            }
        }
        GetComponent<PlayerID>().playerScore = 0;
        GetComponent<PlayerID>().CmdTellServerMyName(val);
        var names = GameObject.FindGameObjectsWithTag("NameText");
        var players = GameObject.FindGameObjectsWithTag("Player");
        var panels = GameObject.FindGameObjectsWithTag("Panel");
        for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
        {
            foreach (var aName in names)
            {
                if (aName.name.Contains((i + 1).ToString()))
                {
                    UpdateUI(aName.GetComponent<Text>(), GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i], gameObject);
                    break;
                }
            }
        }
        foreach (var player in players)
        {
            for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
            {
                if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == player.GetComponent<PlayerID>().playerID)
                {
                    foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
                    {
                        if (scores.name.Contains((i + 1).ToString()))
                        {
                            //Update UI with score
                            scores.GetComponent<Text>().text = player.GetComponent<PlayerID>().playerScore.ToString();
                        }
                    }
                    foreach (var stats in GameObject.FindGameObjectsWithTag("StatsText"))
                    {
                        if (stats.name.Contains((i + 1).ToString()))
                        {
                            //Update UI with Wins and Losses
                            stats.GetComponent<Text>().text = player.GetComponent<PlayerID>().playersWins + " W "
                                + player.GetComponent<PlayerID>().playerLosses + " L ";
                        }
                    }
                }
            }
        }
        //Start game
        if (!GameObject.Find("GameManager").GetComponent<GameStart>().startGame
          && GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count >= GLOBALS.NUMOFPLAYERSTOSTARTGAME)
        {
            //Set the panels for each player
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
                                Apanel.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                                if (photonView.isMine)
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
            GameObject.Find("UI").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("EventPanel").transform.localPosition = new Vector3(-2500f, 0f, 0f);
            GameObject.Find("PlayAgainMenu").transform.localPosition = new Vector3(0f, -2500f, 0f);
            GameObject.Find("LoadingGif").transform.localScale = new Vector3(0, 0, 0);
        }
    }


    //Set the playername over the server 
    public void SetPlayerName(InputField tempField, GameObject panel, Text errorMsg)
    {
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
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
                errorMsg.text = "That name is taken!";
                aError = true;
                return;
            }
        }
        //If there is no error add the player name and update UI
        if (photonView.isMine && !aError)
        {
            PlayerPrefs.SetString("Username", tempField.GetComponent<InputField>().text);
            photonView.RPC("CmdAddPlayer", PhotonTargets.AllBuffered, tempField.GetComponent<InputField>().text);
            PhotonNetwork.player.name = tempField.GetComponent<InputField>().text;
            GameObject.Find("EnterNamePanel").GetComponent<DoozyUI.UIElement>().Hide(false);
            GameObject.Find("PopupText").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("LoadingGif").transform.localScale = new Vector3(1, 1, 1);
            GameObject.Find("Toggle").GetComponent<Toggle>().isOn = GLOBALS.ColorBlindAssist;
        }
    }

    public void closeEscapeMenu()
    {
        if (GameObject.Find("OpponentLeftMessage").GetComponent<Text>().text != "Your opponent has left!")
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
        GLOBALS.ColorBlindAssist = val;
        var players = GameObject.FindGameObjectsWithTag("Player");
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();

        if (val)
        {
            GetComponent<PlayerColor>().colors[1] = Color.blue;
            GetComponent<PlayerColor>().colors[2] = Color.yellow;
            foreach (var square in GameObject.FindGameObjectsWithTag("CenterSquare"))
            {
                if (square.GetComponentInChildren<Renderer>().material.HasProperty("_MKGlowColor"))
                {
                    if (square.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == Color.green)
                    {
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", Color.blue);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", Color.blue);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", Color.blue);
                    }
                    if (square.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == Color.red)
                    {
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", Color.yellow);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", Color.yellow);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", Color.yellow);
                    }
                }
            }
            foreach (var line in GameObject.FindGameObjectsWithTag("Line"))
            {
                if (line.GetComponentInChildren<Renderer>().material.HasProperty("_MKGlowColor"))
                {
                    if (line.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == Color.green)
                    {
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", Color.blue);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", Color.blue);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", Color.blue);
                    }
                    if (line.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == Color.red)
                    {
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", Color.yellow);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", Color.yellow);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", Color.yellow);
                    }
                }
            }
            foreach (var player in players)
            {
                if (player.GetComponent<PlayerColor>().playerColor == Color.green)
                {
                    player.GetComponent<PlayerColor>().playerColor = Color.blue;
                }
                if (player.GetComponent<PlayerColor>().playerColor == Color.red)
                {
                    player.GetComponent<PlayerColor>().playerColor = Color.yellow;
                }
            }
        }
        else
        {
            GetComponent<PlayerColor>().colors[1] = Color.green;
            GetComponent<PlayerColor>().colors[2] = Color.red;
            foreach (var square in GameObject.FindGameObjectsWithTag("CenterSquare"))
            {
                if (square.GetComponentInChildren<Renderer>().material.HasProperty("_MKGlowColor"))
                {
                    if (square.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == Color.blue)
                    {
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", Color.green);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", Color.green);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", Color.green);
                    }
                    if (square.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == Color.yellow)
                    {
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", Color.red);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", Color.red);
                        square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", Color.red);
                    }
                }
            }
            foreach (var line in GameObject.FindGameObjectsWithTag("Line"))
            {
                if (line.GetComponentInChildren<Renderer>().material.HasProperty("_MKGlowColor"))
                {
                    if (line.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == Color.blue)
                    {
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", Color.green);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", Color.green);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", Color.green);
                    }
                    if (line.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowColor") == Color.yellow)
                    {
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", Color.red);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", Color.red);
                        line.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", Color.red);
                    }
                }
            }
            foreach (var player in players)
            {
                if (player.GetComponent<PlayerColor>().playerColor == Color.blue)
                {
                    player.GetComponent<PlayerColor>().playerColor = Color.green;
                }
                if (player.GetComponent<PlayerColor>().playerColor == Color.yellow)
                {
                    player.GetComponent<PlayerColor>().playerColor = Color.red;
                }
            }
        }

    }

}
