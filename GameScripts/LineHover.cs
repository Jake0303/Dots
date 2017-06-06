using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class LineHover : PunBehaviour
{
    public Material hoverMat;
    private RaycastHit hit;
    public GameObject leaveConfMenu;
    void Start()
    {
        //empty RaycastHit object which raycast puts the hit details into
        hit = new RaycastHit();
        leaveConfMenu = GameObject.Find("LeaveConfirmation");
    }
    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine && !Application.isMobilePlatform)
        {
            //Check if a player is hovering
            //ray shooting out of the camera from where the mouse is
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Raycast from the mouse to the level
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<LinePlaced>() != null
                    && hit.collider.GetComponent<LinePlaced>().linePlaced == true
                    && hit.collider.GetComponentInChildren<Renderer>().material.name.Contains("HoverMat"))
                {
                    hit.collider.GetComponentInChildren<Renderer>().enabled = false;
                    //hit.collider.GetComponentInParent<Light>().enabled = false;
                }
                else if (hit.collider.name.Contains("line")
                    && hit.collider.GetComponent<LinePlaced>() != null
                    && hit.collider.GetComponent<LinePlaced>().linePlaced == false
                    && !GetComponent<PlayerClick>().playingAnim
                    && !GameObject.Find("GameManager").GetComponent<GameOver>().gameOver
                    && !GameObject.Find("GameManager").GetComponent<GameStart>().buildGrid
                    && leaveConfMenu != null
                    && !leaveConfMenu.GetComponent<DoozyUI.UIElement>().isVisible)
                {
                    hit.collider.GetComponentInChildren<Renderer>().enabled = true;
                    hit.collider.GetComponentInChildren<Renderer>().material = hoverMat;
                    hit.collider.GetComponentInChildren<Renderer>().material.SetColor("_Color", new Color(GetComponent<PlayerColor>().playerColor.r, GetComponent<PlayerColor>().playerColor.g, GetComponent<PlayerColor>().playerColor.b, 0.7f));
                    hit.collider.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
                    hit.collider.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
                    hit.collider.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GetComponent<PlayerColor>().playerColor);
                    hit.collider.GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", 0.1f);
                    hit.collider.GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", 0.1f);
                }
            }
        }
    }
}
