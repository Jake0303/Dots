using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InstructionsMenuManager : MonoBehaviour {

    //Go back to the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
