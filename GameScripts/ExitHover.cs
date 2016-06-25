using UnityEngine;
using System.Collections;

public class ExitHover : MonoBehaviour {

    //Hide the temporary line if the mouse is not on it
    void OnMouseExit()
    {
        if (GetComponent<LinePlaced>().linePlaced == false)
            GetComponent<Renderer>().enabled = false;
    }
}
