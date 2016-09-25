using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    string transitionText;
    public GameObject backButton;
    void Start()
    {
        GameObject.Find("GameName").GetComponent<Text>().text = GLOBALS.GameName;
        backButton = GameObject.Find("BackToMenuButton");
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

    //Show text while we connect to the matchmaking service
    public void DisplayLoadingText(string text)
    {
        if (GameObject.Find("transitionText") != null)
        {
            GameObject.Find("transitionText").GetComponent<Text>().text = text;
            StartCoroutine(FadeTextToFullAlpha(2f, GameObject.Find("transitionText").GetComponent<Text>()));
        }
    }
    public void TransitionToEnterNameScreen()
    {
        GameObject.Find("PlayButton").GetComponent<Button>().enabled = false;
        GameObject.Find("PlayButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("PlayButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("OptionsButton").GetComponent<Button>().enabled = false;
        GameObject.Find("OptionsButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("OptionsButton").GetComponentInChildren<Text>().color = Color.clear;
        if (GameObject.Find("ExitButton"))
        {
            GameObject.Find("ExitButton").GetComponent<Button>().enabled = false;
            GameObject.Find("ExitButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
            GameObject.Find("ExitButton").GetComponentInChildren<Text>().color = Color.clear;
        }
        GameObject.Find("InstructionsButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("InstructionsButton").GetComponentInChildren<Text>().color = Color.clear;
    }
    public void TransitionToLobby()
    {
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
        GameObject.Find("GameName").GetComponent<Text>().text = "";
        GameObject.Find("VersionText").GetComponent<Text>().text = "";
        GameObject.Find("PlayButton").GetComponent<Button>().enabled = false;
        GameObject.Find("PlayButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("PlayButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("OptionsButton").GetComponent<Button>().enabled = false;
        GameObject.Find("OptionsButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("OptionsButton").GetComponentInChildren<Text>().color = Color.clear;
        if (GameObject.Find("ExitButton"))
        {
            GameObject.Find("ExitButton").GetComponent<Button>().enabled = false;
            GameObject.Find("ExitButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
            GameObject.Find("ExitButton").GetComponentInChildren<Text>().color = Color.clear;
        }
        GameObject.Find("InstructionsButton").GetComponent<Button>().enabled = false;
        GameObject.Find("InstructionsButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("InstructionsButton").GetComponentInChildren<Text>().color = Color.clear;
    }
   //Go to the options menu
    public void Options()
    {
        SceneManager.LoadScene("Options");
    }
    //Go to the options menu
    public void Instructions()
    {
        SceneManager.LoadScene("Instructions");
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
    }
    //Quit the game
    public void ExitGame()
    {
        Application.Quit();
        GameObject.Find("AudioManager").GetComponent<Sound>().PlayButtonSound();
    }
}
