using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour
{
    
    [SyncVar (hook="OnCounterChanged")]
    public int counter;
    // Update the UI for the player score to 0
    void Start()
    {
        StartCoroutine(DynamicPeriods());
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
    }
    void OnCounterChanged(int num)
    {
        counter = num;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();

        var names = GameObject.FindGameObjectsWithTag("NameText");
        var players = GameObject.FindGameObjectsWithTag("Player");
        counter = 0;
            foreach (var name in names)
            {
                if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count > counter && name.name.Contains((counter + 1).ToString()))
                {
                    UpdateUI(name.GetComponent<Text>(), GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[counter], gameObject);
                    counter++;
                }
            }
    }

    [Command]
    public void CmdAddPlayer(string val)
    {
            GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Add(val);
            GetComponent<PlayerID>().nameSet = true;
            gameObject.name = val;
            GetComponent<PlayerID>().playerID = val;
            GetComponent<PlayerID>().CmdTellServerMyName(val);
            RpcAddPlayer(val);
    }
    [ClientRpc]
    public void RpcAddPlayer(string val)
    {
        Debug.Log("player added: " + val);
        var names = GameObject.FindGameObjectsWithTag("NameText");
        
            foreach (var name in names)
            {
                if (GetComponent<PlayerID>().nameSet
                    && name.name.Contains((counter + 1).ToString())
                    && GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count > counter)
                {
                    UpdateUI(name.GetComponent<Text>(), GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[counter], gameObject);
                    counter++;
                    break;
                }
            }
    }
    public void SetPlayerName()
    {
        if (isLocalPlayer)
        {
            CmdAddPlayer(GameObject.Find("EnterNameInputField").GetComponent<InputField>().text);
            Debug.Log("Local");

        }
        else if (isServer)
        {
            GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Add(GameObject.Find("EnterNameInputField").GetComponent<InputField>().text);
            GetComponent<PlayerID>().nameSet = true;
            gameObject.name = GameObject.Find("EnterNameInputField").GetComponent<InputField>().text;
            GetComponent<PlayerID>().playerID = GameObject.Find("EnterNameInputField").GetComponent<InputField>().text;
            GetComponent<PlayerID>().CmdTellServerMyName(GameObject.Find("EnterNameInputField").GetComponent<InputField>().text);
            RpcAddPlayer(GameObject.Find("EnterNameInputField").GetComponent<InputField>().text);
            Debug.Log("Server");

        }
        else
        {
            GetComponent<PlayerID>().OnNameChanged(true);
            Debug.Log("Not local or server");

        }
        GameObject.Find("enterNamePanel").SetActive(false);
    }


    //Update the score text
    public void UpdateScore(GameObject player)
    {
        //Update the player's UI with their score
        var scores = GameObject.FindGameObjectsWithTag("ScoreText");
        foreach (var score in scores)
        {
            if (score.name.Contains(player.GetComponent<PlayerID>().playerTurnOrder.ToString()))
            {
                //UpdateUI(score.GetComponent<Text>(), player.GetComponent<PlayerID>().playerScore.ToString(), player);
            }
        }
    }

    //Update the name text
    public void UpdateUI(Text textToUpdate, string text, GameObject player)
    {
        //Update the player's UI with their name
        textToUpdate.text = text;
        if (isLocalPlayer)
        {
            CmdUpdateUI(textToUpdate.tag, text, player);
        }
    }
    //Tell the server to update the UI
    [Command]
    void CmdUpdateUI(string textToUpdate, string text, GameObject player)
    {
        var uiObjects = GameObject.FindGameObjectsWithTag(textToUpdate);
        for (int i = 0; i < uiObjects.Length; i++)
        {
            //Updating names
            if (uiObjects[i].name.Contains(player.GetComponent<PlayerID>().playerTurnOrder.ToString()))
            {
                uiObjects[i].GetComponent<Text>().text = text;
            }
        }
        RpcUpdateUI(textToUpdate, text, player);
    }

    //Tell the server to update the UI and replicate to all clients
    [ClientRpc]
    void RpcUpdateUI(string textToUpdate, string text, GameObject player)
    {
        var uiObjects = GameObject.FindGameObjectsWithTag(textToUpdate);
        for (int i = 0; i < uiObjects.Length; i++)
        {
            if (uiObjects[i].name.Contains(player.GetComponent<PlayerID>().playerTurnOrder.ToString()))
            {
                uiObjects[i].GetComponent<Text>().text = text;
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
            StartCoroutine(FadeTextToFullAlpha(2f, names[i].GetComponent<Text>()));
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

    //Fade text animation for the connecting text
    IEnumerator FadeTextToFullAlpha(float t, Text i)
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

}
