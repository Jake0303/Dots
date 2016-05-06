using UnityEngine;
using System.Collections;

public class LineHover : MonoBehaviour
{
    public Material hoverMat;
    //Player Hover
    //Hide the temporary line if the mouse is not on it
    void OnMouseExit()
    {
        if (GetComponent<Renderer>().material.color == new Color(1.0f, 0f, 0f, 0.5f))
            GetComponent<Renderer>().enabled = false;


    }
    //When the mouse is between 2 dots show a temporary line
    void OnMouseOver()
    {
        if (GetComponent<Renderer>().enabled != true && !GameObject.Find("GameManager").GetComponent<GameStart>().buildGrid)
        {
            GetComponent<Renderer>().enabled = true;
            GetComponent<Renderer>().material = hoverMat;
            GetComponent<Renderer>().material.color = new Color(1.0f, 0f, 0f, 0.5f);
        }
    }
}
