using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//This class removes duplicate objects if attached
public class BackgroundSingleton : MonoBehaviour
{

    private static BackgroundSingleton instance = null;
    public static BackgroundSingleton Instance
    {
        get { return instance; }
    }
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("FadedSquare"))
        {
            Destroy(square);
        }
    }
    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("FadedSquare"))
        {
            Destroy(square);
        }
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
