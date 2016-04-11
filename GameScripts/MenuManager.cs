using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    string transitionText;
    GameObject enterNameInputField,playButton;
    void Start()
    {
        enterNameInputField = GameObject.Find("EnterNameInputField");
        enterNameInputField.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        enterNameInputField.GetComponentInChildren<Text>().text = "";
        enterNameInputField.SetActive(false);
        playButton = GameObject.Find("LetsPlayButton");
        playButton.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        playButton.GetComponentInChildren<Text>().text = "";
        playButton.SetActive(false);
    }
    //Dynamic period animation
    IEnumerator DynamicPeriods(Text text)
    {
        transitionText = text.text;
        string period = ".";
        for (; ; )
        {
            text.text = transitionText;
            yield return new WaitForSeconds(0.25f);
            text.text = transitionText + period;
            yield return new WaitForSeconds(0.25f);
            text.text = transitionText + period + period;
            yield return new WaitForSeconds(0.25f);
            text.text = transitionText + period + period + period;
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

    //Show text while we connect to the matchmaking service
    public void DisplayLoadingText(string text)
    {

        GameObject.Find("transitionText").GetComponent<Text>().text = text;
        StartCoroutine(FadeTextToFullAlpha(2f, GameObject.Find("transitionText").GetComponent<Text>()));
        StartCoroutine(DynamicPeriods(GameObject.Find("transitionText").GetComponent<Text>()));
    }
    public void TransitionToEnterNameScreen()
    {
        GameObject.Find("PlayButton").GetComponent<Button>().enabled = false;
        GameObject.Find("PlayButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("PlayButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("OptionsButton").GetComponent<Button>().enabled = false;
        GameObject.Find("OptionsButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("OptionsButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("ExitButton").GetComponent<Button>().enabled = false;
        GameObject.Find("ExitButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("ExitButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("infoText").GetComponent<Text>().text = "Please enter a username";
        enterNameInputField.SetActive(true);
        enterNameInputField.GetComponentInChildren<CanvasRenderer>().SetAlpha(1);
        enterNameInputField.GetComponentInChildren<Text>().text = "Enter a username...";
        playButton.SetActive(true);
        playButton.GetComponentInChildren<CanvasRenderer>().SetAlpha(1);
        playButton.GetComponentInChildren<Text>().text = "Lets play!";
    }
    public void TransitionToLobby()
    {
        GameObject.Find("Dots").GetComponent<Text>().text = "";
        GameObject.Find("PlayButton").GetComponent<Button>().enabled = false;
        GameObject.Find("PlayButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("PlayButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("OptionsButton").GetComponent<Button>().enabled = false;
        GameObject.Find("OptionsButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("OptionsButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("ExitButton").GetComponent<Button>().enabled = false;
        GameObject.Find("ExitButton").GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        GameObject.Find("ExitButton").GetComponentInChildren<Text>().color = Color.clear;
        GameObject.Find("infoText").GetComponent<Text>().text = "";
        
        enterNameInputField = GameObject.Find("EnterNameInputField");
        enterNameInputField.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        enterNameInputField.GetComponentInChildren<Text>().text = "";
        /*enterNameInputField.SetActive(false);*/
        playButton = GameObject.Find("LetsPlayButton");
        playButton.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        playButton.GetComponentInChildren<Text>().text = "";
        playButton.SetActive(false);
    }
}
