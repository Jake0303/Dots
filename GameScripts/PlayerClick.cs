using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon;


public class PlayerClick : PunBehaviour
{
    [SerializeField]
    public Material hoverMat;
    //Material for the line placed
    [SerializeField]
    public Material lineMat;
    //Radius of the square hitbox
    
    private float squareRadius = GLOBALS.DOTDISTANCE / 2;
    
    private Color objectColor, squareColor;
    public string objectID;
    public string squareID;
    private RaycastHit hit;
    [SerializeField]
    private Collider[] hitCollidersRight;
    [SerializeField]
    private Collider[] hitCollidersLeft;
    [SerializeField]
    private Collider[] hitCollidersTop;
    [SerializeField]
    private Collider[] hitCollidersBottom;
    private GameObject[] scores;
    public bool pointScored = false;
    [SerializeField]
    public GameObject lineHorizontal, lineVertical, centerSquare;

    [SerializeField]
    public GameObject line, square;
    /*
     * Sync Line position
     */
    private float lerpRate;
    private float normalLerpRate = 6;
    private float fasterLerpRate = 9;

    private float threshold = 0.5f;
    public bool animFinished = false;
    public bool squareAnimFinished = false;
    public bool playingAnim = false;
    public bool playingSquareAnim = false;
    private float closeEnough = 1f;

    /*
     * Sync Line rotation
     */
    private float rotLerpRate = 6;

    private float lastLineRot;
    private float rotThreshold = 0.01f;

    private float rotCloseEnough = 0.1f;

    private PhotonView photonView;
    void Start()
    {
        lerpRate = normalLerpRate;
        photonView = this.GetComponent<PhotonView>();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        scores = GameObject.FindGameObjectsWithTag("ScoreText");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(pointScored);
            stream.SendNext(playingAnim);
            stream.SendNext(playingSquareAnim);
            stream.SendNext(animFinished);
            stream.SendNext(squareAnimFinished);
            stream.SendNext(squareID);

        }
        else
        {
            // Network player, receive data
            this.pointScored = (bool)stream.ReceiveNext();
            this.playingAnim = (bool)stream.ReceiveNext();
            this.playingSquareAnim= (bool)stream.ReceiveNext();
            this.animFinished = (bool)stream.ReceiveNext();
            this.squareAnimFinished = (bool)stream.ReceiveNext();
            this.squareID = (string)stream.ReceiveNext();
        }
    }
    [PunRPC]
    void CmdNextTurn()
    {
        if (!pointScored)
        {
            photonView.RPC("RpcNextTurn", PhotonTargets.AllBuffered);
        }
        else
        {
            if (photonView.isMine && GetComponent<PlayerID>().playerScore != GLOBALS.POINTSTOWIN)
            {
                this.GetComponent<UIManager>().DisplayPopupText("Good job! Please place another line.", true);

            }
            photonView.RPC("RpcSameTurn", PhotonTargets.AllBuffered);
            pointScored = false;
        }
    }
    [PunRPC]
    void RpcSameTurn()
    {
        GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
        GetComponent<PlayerID>().isPlayersTurn = true;
        pointScored = false;
    }
    [PunRPC]
    void RpcNextTurn()
    {
        GameObject.Find("GameManager").GetComponent<TurnTimer>().nextTurn = true;
    }
    [PunRPC]
    void RpcPaint(string obj, string col)
    {
        GameObject.Find(obj).GetComponent<Renderer>().enabled = true;
        GameObject.Find(obj).GetComponent<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponent<Renderer>().material.SetColor("_MKGlowColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponent<LinePlaced>().linePlaced = true;
    }

    [PunRPC]
    void RpcPaintSameTurn(string obj, string col)
    {
        GameObject.Find(obj).GetComponent<Renderer>().enabled = true;
        GameObject.Find(obj).GetComponent<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponent<Renderer>().material.SetColor("_MKGlowColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponent<LinePlaced>().linePlaced = true;
        GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
        GetComponent<PlayerID>().isPlayersTurn = true;
    }
    [PunRPC]
    void CmdPlayAnim(string line)
    {
        animFinished = false;
        playingAnim = true;
        if (playingAnim)
        {
            StartCoroutine(StartLineAnim(line, hit));
        }
    }
    [PunRPC]
    void CmdPlaySquareAnim()
    {
        squareAnimFinished = false;
        playingSquareAnim = true;
        if (playingSquareAnim)
        {
            StartCoroutine(StartSquareAnim(squareID));
        }
    }
    [PunRPC]
    void CmdStopSquareAnim()
    {
        squareAnimFinished = true;
        playingSquareAnim = false;
    }
    [PunRPC]
    void CmdStopAnim()
    {
        animFinished = true;
        playingAnim = false;
    }
    [PunRPC]
    void RpcPlayAnim()
    {
        playingAnim = true;
        animFinished = false;
    }
    //On click place a line at the mouse location
    [PunRPC]
    void CmdPlaceLine(string obj, string col)
    {
        GameObject.Find(obj).GetComponent<Renderer>().enabled = true;// get the object's network ID
        GameObject.Find(obj).GetComponent<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponent<Renderer>().material.SetColor("_MKGlowColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponent<LinePlaced>().linePlaced = true;
        if (!pointScored)
        {
            photonView.RPC("RpcPaint", PhotonTargets.AllBuffered, obj, col);// use a Client RPC function to "paint" the object on all clients
        }
        else
        {
            photonView.RPC("RpcPaintSameTurn", PhotonTargets.AllBuffered, obj, col);// use a Client RPC function to "paint" the 
        }
    }


    //Tell the server the current players score
    [PunRPC]
    void CmdTellServerYourScore(int score)
    {
        GetComponent<PlayerID>().playerScore = score;
        for (int i = 0; i < GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count; i++)
        {
            if (GameObject.Find("GameManager").GetComponent<GameStart>().playerNames[i] == GetComponent<PlayerID>().playerID)
            {
                foreach (var scores in GameObject.FindGameObjectsWithTag("ScoreText"))
                {
                    if (scores.name.Contains((i + 1).ToString()))
                    {
                        //Update UI with score
                        scores.GetComponent<Text>().text = score.ToString();
                        return;
                    }
                }
            }
        }
    }
    //Check if a square is made and if so award a point to the player
    void CheckIfSquareIsMade(RaycastHit hit)
    {
        //Check if square is made
        //Check horizontal line hitboxes
        if (hit.collider.name.Contains("linesHorizontal") && hit.collider.GetComponent<Renderer>().enabled)
        {

            Vector3 centerOfSquareRight = new Vector3(hit.collider.transform.localPosition.x + (GLOBALS.DOTDISTANCE / 2), hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z);
            hitCollidersRight = Physics.OverlapSphere(centerOfSquareRight, squareRadius);
            int i = 0;
            int howManyLines = 0;
            //In the hitbox, check how many lines
            while (i < hitCollidersRight.Length)
            {
                if (hitCollidersRight[i].name.Contains("lines") && hitCollidersRight[i].GetComponent<Renderer>().enabled
                    && hitCollidersRight[i].GetComponent<LinePlaced>().linePlaced
                   || hitCollidersRight[i].name.Contains("square"))
                {
                    hitCollidersRight[i].GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                    howManyLines++;
                }
                i++;
            }

            //If there is a 4 lines in the hitbox a square is made and the player gets a point.
            if (howManyLines == 4)
            {
                GetComponent<PlayerID>().playerScore += 1;
                photonView.RPC("CmdTellServerYourScore", PhotonTargets.AllBuffered, GetComponent<PlayerID>().playerScore);//Update the player's UI with their score
                hit.collider.GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                PaintSquare(hitCollidersRight);
                GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
                pointScored = true;
            }

            //Left hitbox
            Vector3 centerOfSquareLeft = new Vector3(hit.collider.transform.localPosition.x - (GLOBALS.DOTDISTANCE / 2), hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z);
            hitCollidersLeft = Physics.OverlapSphere(centerOfSquareLeft, squareRadius);
            int j = 0;
            howManyLines = 0;
            while (j < hitCollidersLeft.Length)
            {
                if (hitCollidersLeft[j].name.Contains("lines") && hitCollidersLeft[j].GetComponent<Renderer>().enabled
                    && hitCollidersLeft[j].GetComponent<LinePlaced>().linePlaced
                   || hitCollidersLeft[j].name.Contains("square"))
                {
                    hitCollidersLeft[j].GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                    howManyLines++;
                }
                j++;
            }
            if (howManyLines == 4)
            {
                GetComponent<PlayerID>().playerScore += 1;
                photonView.RPC("CmdTellServerYourScore", PhotonTargets.AllBuffered, GetComponent<PlayerID>().playerScore);//Update the player's UI with their score
                hit.collider.GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                PaintSquare(hitCollidersLeft);
                GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
                pointScored = true;

            }
        }
        //Same as above just for vertical lines
        if (hit.collider.name.Contains("linesVertical") && hit.collider.GetComponent<Renderer>().enabled)
        {

            Vector3 centerOfSquareBottom = new Vector3(hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z + (GLOBALS.DOTDISTANCE / 2));
            hitCollidersBottom = Physics.OverlapSphere(centerOfSquareBottom, squareRadius);
            int i = 0;
            int howManyLines = 0;
            while (i < hitCollidersBottom.Length)
            {
                if (hitCollidersBottom[i].name.Contains("lines") && hitCollidersBottom[i].GetComponent<Renderer>().enabled
                    && hitCollidersBottom[i].GetComponent<LinePlaced>().linePlaced
                   || hitCollidersBottom[i].name.Contains("square"))
                {
                    hitCollidersBottom[i].GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                    howManyLines++;
                }
                i++;
            }
            if (howManyLines == 4)
            {
                GetComponent<PlayerID>().playerScore += 1;
                photonView.RPC("CmdTellServerYourScore", PhotonTargets.AllBuffered, GetComponent<PlayerID>().playerScore);//Update the player's UI with their score
                hit.collider.GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                PaintSquare(hitCollidersBottom);
                GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
                pointScored = true;

            }
            Vector3 centerOfSquareTop = new Vector3(hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z - (GLOBALS.DOTDISTANCE / 2));
            hitCollidersTop = Physics.OverlapSphere(centerOfSquareTop, squareRadius);
            int j = 0;
            howManyLines = 0;
            while (j < hitCollidersTop.Length)
            {
                if (hitCollidersTop[j].name.Contains("lines") && hitCollidersTop[j].GetComponent<Renderer>().enabled
                    && hitCollidersTop[j].GetComponent<LinePlaced>().linePlaced
                   || hitCollidersTop[j].name.Contains("square"))
                {
                    hitCollidersTop[j].GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                    howManyLines++;
                }
                j++;
            }
            if (howManyLines == 4)
            {
                GetComponent<PlayerID>().playerScore += 1;
                photonView.RPC("CmdTellServerYourScore", PhotonTargets.AllBuffered, GetComponent<PlayerID>().playerScore);//Update the player's UI with their score
                hit.collider.GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                PaintSquare(hitCollidersTop);
                GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
                pointScored = true;

            }
        }

    }
    //Paint the square that was made the players color
    void PaintSquare(Collider[] squareLines)
    {
        int i = 0;
        GameObject[] squares = GameObject.FindGameObjectsWithTag("CenterSquare");
        GameObject squareFound = null;
        while (i < squareLines.Length)
        {
            line = squareLines[i].gameObject;
            squareLines[i].GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
            squareLines[i].GetComponent<Renderer>().material.SetColor("_MKTexColor", GetComponent<PlayerColor>().playerColor);
            if (squareFound == null)
            {
                foreach (var aSquare in squares)
                {
                    if (!aSquare.name.Contains("temp") && aSquare.name.Contains(line.name))
                    {
                        square = aSquare;
                        squareFound = square;
                        squareID = squareFound.name;
                        squareColor = GetComponent<PlayerColor>().playerColor;
                        photonView.RPC("CmdPaintSquare", PhotonTargets.AllBuffered, square.name);
                        break;
                    }
                }
            }
            if (!line.name.Contains("temp") && !line.name.Contains("Centre"))
                photonView.RPC("CmdPaintLines", PhotonTargets.AllBuffered, line.name);
            else if (line.name.Contains("temp"))
            {
                line.GetComponent<Renderer>().enabled = false;
            }
            i++;
        }
        photonView.RPC("CmdPlaySquareAnim", PhotonTargets.AllBuffered);
    }
    //Tell the server to paint the square
    [PunRPC]
    void CmdPaintSquare(string aSquare)
    {
        if (aSquare != null)
        {
            square = GameObject.Find(aSquare);
            squareID = GameObject.Find(aSquare).name;
            squareColor = GetComponent<PlayerColor>().playerColor;
            square.GetComponent<Renderer>().material = lineMat;
            square.GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
            square.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
            pointScored = true;
            //RpcPaintSquare(square);
        }
    }
    //Tell the clients to paint the square
    [PunRPC]
    void RpcPaintSquare(string aSquare)
    {
        if (aSquare != null)
        {
            square = GameObject.Find(aSquare);
            squareID = GameObject.Find(aSquare).name;
            squareColor = GetComponent<PlayerColor>().playerColor;
            square.GetComponent<Renderer>().material = lineMat;
            square.GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
            square.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
            pointScored = true;
        }
    }
    //Paint the square the player made
    [PunRPC]
    void CmdPaintLines(string line)
    {

        GameObject.Find(line).GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(line).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
        //RpcPaintLine(line);
    }

    [PunRPC]
    void RpcPaintLine(string line)
    {

        GameObject.Find(line).GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(line).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
    }

    // Update is called once per frame
    void Update()
    {
        //Must be the players turn to place a line
        if (photonView.isMine && GetComponent<PlayerID>().isPlayersTurn && Input.GetMouseButtonDown(0))
        {
            //empty RaycastHit object which raycast puts the hit details into
            hit = new RaycastHit();
            //ray shooting out of the camera from where the mouse is
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Raycast from the mouse to the level, if hit place a line
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name.Contains("line")
                    && hit.collider.GetComponent<LinePlaced>().linePlaced == false
                    && !playingAnim
                    && !GameObject.Find("GameManager").GetComponent<GameOver>().gameOver
                    && !GameObject.Find("GameManager").GetComponent<GameStart>().buildGrid)
                {
                    objectID = hit.collider.name;// this gets the object that is hit
                    hit.collider.GetComponent<Renderer>().enabled = false;
                    objectColor = GetComponent<PlayerColor>().playerColor;
                    GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                    photonView.RPC("CmdSelectObject", PhotonTargets.AllBuffered, hit.collider.name);
                    photonView.RPC("CmdPlayAnim", PhotonTargets.AllBuffered,hit.collider.name);
                }
            }
        }
    }
    [PunRPC]
    void CmdSelectObject(string name)
    {
        objectID = name;
        objectColor = GetComponent<PlayerColor>().playerColor;
        GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
    }
    //Spawn a temporary line for an animation
    void SpawnLineForAnim(string line)
    {
        if (GameObject.Find(line).name.Contains("Horizontal"))
        {
            lineHorizontal = Instantiate(lineHorizontal, new Vector3(GameObject.Find(line).transform.position.x, 50, GameObject.Find(line).transform.position.z), GameObject.Find(line).transform.rotation) as GameObject;
            lineHorizontal.name = "temp";
            lineHorizontal.transform.position = new Vector3(GameObject.Find(line).transform.position.x, 50, GameObject.Find(line).transform.position.z);
            lineHorizontal.transform.rotation = GameObject.Find(line).transform.rotation;
            lineHorizontal.GetComponent<Renderer>().enabled = true;
            lineHorizontal.GetComponent<Renderer>().material = lineMat;
            lineHorizontal.GetComponent<Renderer>().material.SetColor("_MKGlowColor", objectColor);
            lineHorizontal.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", objectColor);
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(lineHorizontal);
        }
        else
        {
            lineVertical = Instantiate(lineVertical, new Vector3(GameObject.Find(line).transform.position.x, 50, GameObject.Find(line).transform.position.z), GameObject.Find(line).transform.rotation) as GameObject;
            lineVertical.name = "temp";
            lineVertical.transform.position = new Vector3(GameObject.Find(line).transform.position.x, 50, GameObject.Find(line).transform.position.z);
            lineVertical.transform.rotation = GameObject.Find(line).transform.rotation;
            lineVertical.GetComponent<Renderer>().enabled = true;
            lineVertical.GetComponent<Renderer>().material = lineMat;
            lineVertical.GetComponent<Renderer>().material.SetColor("_MKGlowColor", objectColor);
            lineVertical.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", objectColor);
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(lineVertical);
        }
    }
    //Play the line animation of falling from the sky and rotating
    IEnumerator StartLineAnim(string line, RaycastHit hit)
    {
        SpawnLineForAnim(line);
        while (!animFinished)
        {
            //Lerp the lines location and rotation for a smooth animation
            if (GameObject.Find(objectID).name.Contains("Horizontal") && lineHorizontal != null)
            {
                lineHorizontal.transform.rotation = Quaternion.Slerp(lineHorizontal.transform.rotation, Quaternion.Euler(540, 0, 0), rotLerpRate * Time.deltaTime);
                if (lineHorizontal.transform.position.y > 0)
                {
                    lineHorizontal.transform.position = Vector3.Lerp(lineHorizontal.transform.position, new Vector3(lineHorizontal.transform.position.x, 0, lineHorizontal.transform.position.z), Time.deltaTime * lerpRate);
                }

                if (lineHorizontal.transform.position.y < 0.01 && !animFinished)
                {
                    if (photonView.isMine && GetComponent<PlayerID>().isPlayersTurn)
                    {
                        if (lineHorizontal != null)
                            lineHorizontal.GetComponent<Renderer>().enabled = false;
                        if (lineVertical != null)
                            lineVertical.GetComponent<Renderer>().enabled = false;

                        GameObject.Find(objectID).GetComponent<Renderer>().enabled = true;// get the object's network ID
                        GameObject.Find(objectID).GetComponent<Renderer>().material = lineMat;
                        GameObject.Find(objectID).GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
                        GameObject.Find(objectID).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
                        GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                        photonView.RPC("CmdPlaceLine", PhotonTargets.AllBuffered, objectID, objectColor.ToString());
                        photonView.RPC("CmdStopAnim", PhotonTargets.AllBuffered);
                        animFinished = true;
                        CheckIfSquareIsMade(hit);
                        if (!pointScored)
                        {
                            photonView.RPC("CmdNextTurn", PhotonTargets.AllBuffered);
                        }
                    }
                }
                if (animFinished)
                {
                    if (lineHorizontal != null)
                        lineHorizontal.GetComponent<Renderer>().enabled = false;
                    if (lineVertical != null)
                        lineVertical.GetComponent<Renderer>().enabled = false;
                    GameObject.Find("temp").GetComponent<Renderer>().enabled = false;
                }
            }
            //Lerp vertical line 
            else
            {
                if (lineVertical != null)
                {
                    lineVertical.transform.rotation = Quaternion.Slerp(lineVertical.transform.rotation, Quaternion.Euler(0, 0, 540), rotLerpRate * Time.deltaTime);
                    if (lineVertical.transform.position.y > 0)
                    {
                        lineVertical.transform.position = Vector3.Lerp(lineVertical.transform.position, new Vector3(lineVertical.transform.position.x, 0, lineVertical.transform.position.z), Time.deltaTime * lerpRate);
                    }

                    if (lineVertical.transform.position.y < 0.01 && !animFinished)
                    {
                        if (photonView.isMine && GetComponent<PlayerID>().isPlayersTurn)
                        {
                            if (lineHorizontal != null)
                                lineHorizontal.GetComponent<Renderer>().enabled = false;
                            if (lineVertical != null)
                                lineVertical.GetComponent<Renderer>().enabled = false;
                            GameObject.Find(objectID).GetComponent<Renderer>().enabled = true;// get the object's network ID
                            GameObject.Find(objectID).GetComponent<Renderer>().material = lineMat;
                            GameObject.Find(objectID).GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
                            GameObject.Find(objectID).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
                            GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                            photonView.RPC("CmdPlaceLine", PhotonTargets.AllBuffered, objectID, objectColor.ToString());
                            photonView.RPC("CmdStopAnim", PhotonTargets.AllBuffered);
                            animFinished = true;
                            CheckIfSquareIsMade(hit);
                            if (!pointScored)
                            {
                                photonView.RPC("CmdNextTurn", PhotonTargets.AllBuffered);
                            }
                        }

                    }

                }
            }
            if (animFinished)
            {
                if (lineHorizontal != null)
                    lineHorizontal.GetComponent<Renderer>().enabled = false;
                if (lineVertical != null)
                    lineVertical.GetComponent<Renderer>().enabled = false;
                GameObject.Find("temp").GetComponent<Renderer>().enabled = false;
            }
            yield return new WaitForSeconds(0.01f);
        }
        if (animFinished)
        {
            if (lineHorizontal != null)
                lineHorizontal.GetComponent<Renderer>().enabled = false;
            if (lineVertical != null)
                lineVertical.GetComponent<Renderer>().enabled = false;
            GameObject.Find("temp").GetComponent<Renderer>().enabled = false;
        }
    }
    //Spawn a temporary square for an animation
    void SpawnSquareForAnim(string square)
    {
        centerSquare = Instantiate(GameObject.Find(square), new Vector3(GameObject.Find(square).transform.position.x, 50, GameObject.Find(square).transform.position.z), GameObject.Find(square).transform.rotation) as GameObject;
        centerSquare.name = "tempSquare";
        centerSquare.transform.position = new Vector3(GameObject.Find(square).transform.position.x, 50, GameObject.Find(square).transform.position.z);
        centerSquare.transform.rotation = GameObject.Find(square).transform.rotation;
        centerSquare.GetComponent<Renderer>().enabled = true;// get the object's network ID
        centerSquare.GetComponent<Renderer>().material = lineMat;
        centerSquare.GetComponent<Renderer>().material.SetColor("_MKGlowColor", squareColor);
        centerSquare.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", squareColor);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(centerSquare);


    }
    //Play the square animation of falling from the sky and rotating
    IEnumerator StartSquareAnim(string square)
    {
        SpawnSquareForAnim(square);
        while (!squareAnimFinished && centerSquare != null)
        {
            //Lerp the square location and rotation for a smooth animation
            centerSquare.transform.rotation = Quaternion.Slerp(centerSquare.transform.rotation, Quaternion.Euler(540, 0, 0), rotLerpRate * Time.deltaTime);
            if (centerSquare.transform.position.y > 0)
            {
                centerSquare.transform.position = Vector3.Lerp(centerSquare.transform.position, new Vector3(centerSquare.transform.position.x, 0, centerSquare.transform.position.z), Time.deltaTime * lerpRate);
            }
            if (centerSquare.transform.position.y < 0.01 && !squareAnimFinished)
            {
                if (photonView.isMine && GetComponent<PlayerID>().isPlayersTurn)
                {
                    if (centerSquare != null)
                        centerSquare.GetComponent<Renderer>().enabled = false;
                    GameObject.Find(squareID).GetComponent<Renderer>().enabled = true;// get the object's network ID
                    GameObject.Find(squareID).GetComponent<Renderer>().material = lineMat;
                    GameObject.Find(squareID).GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
                    GameObject.Find(squareID).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
                    photonView.RPC("CmdStopSquareAnim", PhotonTargets.AllBuffered);
                    photonView.RPC("CmdNextTurn", PhotonTargets.AllBuffered);
                    squareAnimFinished = true;
                }
            }
            if (squareAnimFinished)
            {
                if (centerSquare != null)
                    centerSquare.GetComponent<Renderer>().enabled = false;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
}

