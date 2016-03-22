using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class LoadOnClick : MonoBehaviour {
	//Initializing the loading screne in between scenes
	public GameObject loadingImage;
	public GameObject menuPanel;
	private bool isInTransition = false;
	private bool pauseToggle = false;
	private static bool joinScene;
	private GameObject mainCanvas;
	private CanvasGroup canvasGroup;

	public void LoadScene(int level)
	{
		//Create Server
		if (level == 1) {
			joinScene = false;
			loadingImage.SetActive (true);
			SceneManager.LoadScene (level);
		} 
		//Join Game
		else if (level == 3) {
			joinScene = true;
			loadingImage.SetActive (true);
			SceneManager.LoadScene (1);
		}
		//If we are at the create server screen
		if (SceneManager.GetActiveScene().name == "BuildGrid") {

			//Grab the slider values for the grid size
			GameStart.gridHeight = (int)GameObject.Find ("GridSizeSlider").GetComponent<Slider> ().value;
			GameStart.gridWidth = (int)GameObject.Find ("GridSizeSlider").GetComponent<Slider> ().value;

			//Start game,spawn player
			if (level == 2) {
				SceneManager.LoadScene (level);
				loadingImage.SetActive (true);
				joinScene = false;
			}
			//Back to main menu
			else if(level == 0)
			{
				SceneManager.LoadScene (level);
				loadingImage.SetActive (true);
				joinScene = false;
			}
			else if(level == 3)
				joinScene = true;
			//Makes the exit button discconect from host
			if (GameObject.Find ("MenuPanel") != null && GameObject.Find ("MenuPanel").activeSelf) {
				GameObject.Find ("ExitButton").GetComponent<Button> ().onClick.RemoveAllListeners ();

			}
		}

	}



	public void Start()
	{
		HandleMenu ();
	}
	public void Update()
	{
		//Debug.Log (joinScene);
		if (Input.GetKeyDown (KeyCode.Escape)) {
			HandleMenu ();

		}

		//If the player clicks join, hide the create server UI and show the join scene
		if (joinScene && GameObject.Find ("Canvas").GetComponent<CanvasGroup> () != null) {
			GameObject.Find ("Canvas").GetComponent<CanvasGroup> ().alpha = 0;
			GameObject.Find ("Canvas").GetComponent<CanvasGroup> ().blocksRaycasts = false;
			GameObject.Find ("Canvas").GetComponent<CanvasGroup> ().interactable = false;
			//Else show create server ui and hide the join server ui
		} else {
			if(SceneManager.GetActiveScene().name == "MainMenu")
			{
			if(GameObject.Find ("JoinGameCanvas"))
			{
			GameObject.Find ("JoinGameCanvas").GetComponent<CanvasGroup> ().alpha = 0;
			GameObject.Find ("JoinGameCanvas").GetComponent<CanvasGroup> ().blocksRaycasts = false;
			GameObject.Find ("JoinGameCanvas").GetComponent<CanvasGroup> ().interactable = false;
			}
			if(GameObject.Find ("Canvas").GetComponent<CanvasGroup>() != null)
			{
			GameObject.Find ("Canvas").GetComponent<CanvasGroup> ().alpha = 1;
			GameObject.Find ("Canvas").GetComponent<CanvasGroup> ().blocksRaycasts = true;
			GameObject.Find ("Canvas").GetComponent<CanvasGroup> ().interactable = true;
			}
			}
		}


	}    
	
	public void HandlePause()
	{

		if (pauseToggle) {
			Time.timeScale = 1;

		} else {
			Time.timeScale = 0;

		}
		pauseToggle = !pauseToggle;
	}
	//Toggle hide/show menu
	public void HandleMenu()
	 {
		GameObject menuPanel = GameObject.Find ("MenuPanel");
		if(!isInTransition)
		{
			//toogle menu
			if(menuPanel!=null)
			menuPanel.SetActive(!menuPanel.activeSelf);
			
			isInTransition = true;
			
			StartCoroutine(WaitDisableTransition());
		}
		//if (MenuPanel.activeSelf) {
		//  HandlePause();
		//}

	}
		
		IEnumerator WaitDisableTransition()
		{
		if (Time.timeScale == 1) {
			//doesn't work with Time.timeScale = 0
			yield return new WaitForSeconds (0.35f);
		} else {

			//alternative when Time.timeScale = 0
		   float time = Time.realtimeSinceStartup;
		   while(Time.realtimeSinceStartup - time < 0.35f)
		   yield return new WaitForEndOfFrame();

		}
			DisableTransition();

		}
		
		public void DisableTransition()
		{
			isInTransition = false;
		}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene (0);
	}





}
