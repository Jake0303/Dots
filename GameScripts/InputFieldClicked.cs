using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class InputFieldClicked : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if ((Screen.orientation == ScreenOrientation.Portrait
            || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            && (SceneManager.GetActiveScene().buildIndex == 0)
            && Application.isMobilePlatform)
        {
            GameObject.Find("EnterNickMenu").transform.localPosition = new Vector3(0, 200f, 0);
        }
    }
}