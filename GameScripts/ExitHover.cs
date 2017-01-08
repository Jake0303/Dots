using UnityEngine;
using System.Collections;

public class ExitHover : MonoBehaviour
{
    public Material hoverMat;
#if UNITY_WEBGL
    //Hide the temporary line if the mouse is not on it
    void OnMouseExit()
    {
        if (GetComponent<LinePlaced>().linePlaced == false && name.Contains("line"))
        {
            GetComponentInChildren<Renderer>().enabled = false;
            GetComponentInParent<Light>().enabled = false;
        }
    }
#endif
}
