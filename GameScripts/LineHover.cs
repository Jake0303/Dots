using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Photon;


public class LineHover : PunBehaviour
{
    public Material hoverMat;
    private RaycastHit hit;
    void Start()
    {
        //empty RaycastHit object which raycast puts the hit details into
        hit = new RaycastHit();
    }
    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            //Check if a player is hovering
            //ray shooting out of the camera from where the mouse is
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Raycast from the mouse to the level
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<LinePlaced>() != null
                    && hit.collider.GetComponent<LinePlaced>().linePlaced == true
                    && hit.collider.GetComponentInChildren<Renderer>().material.name.Contains("glow"))
                {
                    hit.collider.GetComponentInChildren<Renderer>().enabled = false;
                    hit.collider.GetComponentInParent<Light>().enabled = false;
                }
                else if (hit.collider.name.Contains("line")
                    && hit.collider.GetComponent<LinePlaced>() != null
                    && hit.collider.GetComponent<LinePlaced>().linePlaced == false
                    && !GetComponent<PlayerClick>().playingAnim
                    && !GameObject.Find("GameManager").GetComponent<GameOver>().gameOver
                    && !GameObject.Find("GameManager").GetComponent<GameStart>().buildGrid)
                {
                    hit.collider.GetComponentInChildren<Renderer>().enabled = true;
                    hit.collider.GetComponentInChildren<Renderer>().material = hoverMat;
                    hit.collider.GetComponentInParent<Light>().enabled = true;
                    hit.collider.GetComponentInParent<Light>().color = GetComponent<PlayerColor>().playerColor;
                    hit.collider.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", GetComponent<PlayerColor>().playerColor);
                    hit.collider.GetComponentInChildren<Renderer>().material.SetColor("_detailcolor", GetComponent<PlayerColor>().playerColor);
                }
            }
        }
    }
}
