using UnityEngine;
using System.Collections;
//This class removes duplicate objects if attached
public class BackgroundSingleton : MonoBehaviour
{

    private static BackgroundSingleton instance = null;
    public static BackgroundSingleton Instance
    {
        get { return instance; }
    }
    void OnLevelWasLoaded(int level)
    {
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("FadedSquare"))
        {
            Destroy(square);
        }
    }
    void Start()
    {
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
