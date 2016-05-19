using UnityEngine;
using System.Collections;

public class MenuSingleton : MonoBehaviour {

    private static MenuSingleton instance = null;
    public static MenuSingleton Instance
    {
        get { return instance; }
    }
    void Start()
    {
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
