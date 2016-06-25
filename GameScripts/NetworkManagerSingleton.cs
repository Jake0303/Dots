using UnityEngine;
using System.Collections;

public class NetworkManagerSingleton : MonoBehaviour {

    private static NetworkManagerSingleton instance = null;
    public static NetworkManagerSingleton Instance
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
