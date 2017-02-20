using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
            DeviceChange.OnOrientationChange += MyOrientationChange;
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DeviceChange.OnOrientationChange += MyOrientationChange;
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void MyOrientationChange(DeviceOrientation orientation)
    {
        if ((orientation == DeviceOrientation.Portrait
            || orientation == DeviceOrientation.PortraitUpsideDown)
            && (SceneManager.GetActiveScene().buildIndex == 0))
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = 55;
        }
        else if ((orientation != DeviceOrientation.Portrait
            && orientation != DeviceOrientation.PortraitUpsideDown)
            && SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = 30;
        }
    }
}
