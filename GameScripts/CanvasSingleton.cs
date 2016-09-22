using UnityEngine;
using System.Collections;
//This class removes duplicate objects if attached
public class CanvasSingleton : MonoBehaviour
{

    private static CanvasSingleton instance = null;
    public static CanvasSingleton Instance
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
