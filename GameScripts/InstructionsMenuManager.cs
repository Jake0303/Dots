using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InstructionsMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("GameName").GetComponent<Text>().text = GLOBALS.GameName;

	}

    //Go back to the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
