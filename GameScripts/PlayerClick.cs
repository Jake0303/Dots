using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerClick : NetworkBehaviour
{
    [SerializeField]
    public Material hoverMat;
    //Material for the line placed
    [SerializeField]
    public Material lineMat;
    //Radius of the square hitbox
    [SyncVar]
    private float squareRadius = GameStart.dotDistance / 2;
    [SyncVar]
    private Color objectColor, squareColor;
    [SyncVar(hook = "OnObjectClicked")]
    public string objectID;
    [SyncVar(hook = "OnSquareClicked")]
    public string squareID;
    private NetworkIdentity objNetId;
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
    [SyncVar(hook = "OnPointChanged")]
    public bool pointScored = false;
    [SerializeField]
    public GameObject lineHorizontal, lineVertical, centerSquare;

    [SerializeField]
    public GameObject line, square;
    /*
     * Sync Line position
     */
    private float lerpRate;
    private float normalLerpRate = 5;
    private float fasterLerpRate = 8;

    private float threshold = 0.5f;
    [SyncVar(hook = "OnAnimFinished")]
    public bool animFinished = false;
    [SyncVar(hook = "OnSquareAnimFinished")]
    public bool squareAnimFinished = false;
    [SyncVar(hook = "OnPlayAnim")]
    public bool playingAnim = false;
    [SyncVar(hook = "OnPlaySquareAnim")]
    public bool playingSquareAnim = false;
    private float closeEnough = 1f;

    /*
     * Sync Line rotation
     */
    private float rotLerpRate = 6;

    private float lastLineRot;
    private float rotThreshold = 0.01f;

    private float rotCloseEnough = 0.1f;



    void Start()
    {
        lerpRate = normalLerpRate;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        scores = GameObject.FindGameObjectsWithTag("ScoreText");
    }
    void OnPointChanged(bool point)
    {
        pointScored = point;
    }
    void OnPlayAnim(bool anim)
    {
        playingAnim = anim;
        if (playingAnim)
        {
            StartCoroutine(StartLineAnim(objectID, hit));
        }
    }
    void OnPlaySquareAnim(bool anim)
    {
        playingSquareAnim = anim;
        if (playingSquareAnim)
        {
            StartCoroutine(StartSquareAnim(squareID));
        }
    }
    void OnAnimFinished(bool anim)
    {
        animFinished = anim;
    }
    void OnSquareAnimFinished(bool anim)
    {
        squareAnimFinished = anim;
    }
    void OnObjectClicked(string obj)
    {
        objectID = obj;

    }
    void OnSquareClicked(string sq)
    {
        squareID = sq;
    }
    [Command]
    void CmdNextTurn()
    {
        if (!pointScored)
        {
            RpcNextTurn();
        }
        else
        {
            RpcSameTurn();
            pointScored = false;
        }
    }
    [ClientRpc]
    void RpcSameTurn()
    {
        GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
        GetComponent<PlayerID>().isPlayersTurn = true;
        pointScored = false;
    }
    [ClientRpc]
    void RpcNextTurn()
    {
        GameObject.Find("GameManager").GetComponent<TurnTimer>().nextTurn = true;
    }
    [ClientRpc]
    void RpcPaint(string obj, Color col)
    {
        GameObject.Find(obj).GetComponent<Renderer>().enabled = true;
        GameObject.Find(obj).GetComponent<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponent<Renderer>().material.color = col;      // this is the line that actually makes the change in color happen
        GameObject.Find(obj).GetComponent<LinePlaced>().linePlaced = true;
    }

    [ClientRpc]
    void RpcPaintSameTurn(string obj, Color col)
    {
        GameObject.Find(obj).GetComponent<Renderer>().enabled = true;
        GameObject.Find(obj).GetComponent<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponent<Renderer>().material.color = col;      // this is the line that actually makes the change in color happen
        GameObject.Find(obj).GetComponent<LinePlaced>().linePlaced = true;
        GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
        GetComponent<PlayerID>().isPlayersTurn = true;
    }
    [Command]
    void CmdPlayAnim()
    {
        animFinished = false;
        playingAnim = true;
        //RpcPlayAnim();
    }
    [Command]
    void CmdPlaySquareAnim()
    {
        squareAnimFinished = false;
        playingSquareAnim = true;
    }
    [Command]
    void CmdStopSquareAnim()
    {
        squareAnimFinished = true;
        playingSquareAnim = false;
    }
    [Command]
    void CmdStopAnim()
    {
        animFinished = true;
        playingAnim = false;
    }
    [ClientRpc]
    void RpcPlayAnim()
    {
        playingAnim = true;
        animFinished = false;
    }
    //On click place a line at the mouse location
    [Command]
    void CmdPlaceLine(string obj, Color col)
    {
        objNetId = GameObject.Find(obj).GetComponent<NetworkIdentity>();
        GameObject.Find(obj).GetComponent<Renderer>().enabled = true;// get the object's network ID
        GameObject.Find(obj).GetComponent<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponent<Renderer>().material.color = col;
        GameObject.Find(obj).GetComponent<LinePlaced>().linePlaced = true;
        if (objNetId.clientAuthorityOwner == null)
            objNetId.AssignClientAuthority(connectionToClient);
        if (!pointScored)
        {
            RpcPaint(obj, col);// use a Client RPC function to "paint" the object on all clients
        }
        else
        {
            RpcPaintSameTurn(obj, col);
        }
    }


    //Tell the server the current players score
    [Command]
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
    //Tell all the clients their current score
    [ClientRpc]
    void RpcUpdateScore(GameObject player, int score, GameObject scores)
    {
        //scores.GetComponent<Text>().text = score.ToString();
    }
    //Check if a square is made and if so award a point to the player
    void CheckIfSquareIsMade(RaycastHit hit)
    {
        //Check if square is made
        //Check horizontal line hitboxes
        if (hit.collider.name.Contains("linesHorizontal") && hit.collider.GetComponent<Renderer>().enabled)
        {

            Vector3 centerOfSquareRight = new Vector3(hit.collider.transform.localPosition.x + (GameStart.dotDistance / 2), hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z);
            hitCollidersRight = Physics.OverlapSphere(centerOfSquareRight, squareRadius);
            int i = 0;
            int howManyLines = 0;
            //In the hitbox, check how many lines
            while (i < hitCollidersRight.Length)
            {
                if (hitCollidersRight[i].name.Contains("lines") && hitCollidersRight[i].GetComponent<Renderer>().enabled
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
                CmdTellServerYourScore(GetComponent<PlayerID>().playerScore);
                //Update the player's UI with their score
                hit.collider.GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                PaintSquare(hitCollidersRight);
                GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
                pointScored = true;
            }

            //Left hitbox
            Vector3 centerOfSquareLeft = new Vector3(hit.collider.transform.localPosition.x - (GameStart.dotDistance / 2), hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z);
            hitCollidersLeft = Physics.OverlapSphere(centerOfSquareLeft, squareRadius);
            int j = 0;
            howManyLines = 0;
            while (j < hitCollidersLeft.Length)
            {
                if (hitCollidersLeft[j].name.Contains("lines") && hitCollidersLeft[j].GetComponent<Renderer>().enabled
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
                CmdTellServerYourScore(GetComponent<PlayerID>().playerScore);
                hit.collider.GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                PaintSquare(hitCollidersLeft);
                GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
                pointScored = true;

            }
        }
        //Same as above just for vertical lines
        if (hit.collider.name.Contains("linesVertical") && hit.collider.GetComponent<Renderer>().enabled)
        {

            Vector3 centerOfSquareBottom = new Vector3(hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z + (GameStart.dotDistance / 2));
            hitCollidersBottom = Physics.OverlapSphere(centerOfSquareBottom, squareRadius);
            int i = 0;
            int howManyLines = 0;
            while (i < hitCollidersBottom.Length)
            {
                if (hitCollidersBottom[i].name.Contains("lines") && hitCollidersBottom[i].GetComponent<Renderer>().enabled
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
                CmdTellServerYourScore(GetComponent<PlayerID>().playerScore);
                hit.collider.GetComponent<LineID>().lineID = "square " + hit.collider.transform.localPosition;
                PaintSquare(hitCollidersBottom);
                GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
                pointScored = true;

            }
            Vector3 centerOfSquareTop = new Vector3(hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z - (GameStart.dotDistance / 2));
            hitCollidersTop = Physics.OverlapSphere(centerOfSquareTop, squareRadius);
            int j = 0;
            howManyLines = 0;
            while (j < hitCollidersTop.Length)
            {
                if (hitCollidersTop[j].name.Contains("lines") && hitCollidersTop[j].GetComponent<Renderer>().enabled
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
                CmdTellServerYourScore(GetComponent<PlayerID>().playerScore);
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
        NetworkInstanceId objID;
        objID = new NetworkInstanceId();
        int i = 0;
        GameObject[] squares = GameObject.FindGameObjectsWithTag("CenterSquare");
        GameObject squareFound = null;
        while (i < squareLines.Length)
        {
            line = squareLines[i].gameObject;
            objID = squareLines[i].GetComponent<NetworkIdentity>().netId;
            objNetId = squareLines[i].GetComponent<NetworkIdentity>();
            squareLines[i].GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
            if (squareFound == null)
            {
                foreach (var aSquare in squares)
                {
                    if (!aSquare.name.Contains("temp") && aSquare.name.Contains(line.name))
                    {
                        square = aSquare;
                        squareFound = square;
                        squareID = squareFound.name;
                        CmdPlaySquareAnim();
                        squareColor = GetComponent<PlayerColor>().playerColor;
                        break;
                    }
                }
            }
            CmdPaintSquare(line, square);
            i++;
        }
    }
    //Paint the square the player made
    [Command]
    void CmdPaintSquare(GameObject line, GameObject aSquare)
    {
        if (line != null)
        {
            line.GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
            if (aSquare != null)
            {
                square = aSquare;
                squareID = aSquare.name;
                squareColor = GetComponent<PlayerColor>().playerColor;
            }
            pointScored = true;
            RpcPaintSquare(line, aSquare);
        }
    }

    [ClientRpc]
    void RpcPaintSquare(GameObject line, GameObject aSquare)
    {
        if (line != null)
        {
            line.GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
            if (aSquare != null)
            {
                square = aSquare;
                squareID = aSquare.name;
                squareColor = GetComponent<PlayerColor>().playerColor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        //Must be the players turn to place a line
        if (GetComponent<PlayerID>().isPlayersTurn)
        {
            //Check if a player is clicking, if hovering over a line place one
            if (Input.GetMouseButtonDown(0))
            {
                if (isLocalPlayer)
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
                            objectColor = GetComponent<PlayerColor>().playerColor;
                            GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                            CmdSelectObject(hit.collider.name);
                            CmdPlayAnim();
                            objectColor = GetComponent<PlayerColor>().playerColor;
                            GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                        }
                    }
                }
            }
        }
    }
    [Command]
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
            lineHorizontal = Instantiate(GameObject.Find(line), Vector3.zero, GameObject.Find(line).transform.rotation) as GameObject;
            lineHorizontal.name = "temp";
            lineHorizontal.transform.position = new Vector3(GameObject.Find(line).transform.position.x, 50, GameObject.Find(line).transform.position.z);
            lineHorizontal.transform.rotation = GameObject.Find(line).transform.rotation;
            lineHorizontal.GetComponent<Renderer>().enabled = true;// get the object's network ID
            lineHorizontal.GetComponent<Renderer>().material = lineMat;
            lineHorizontal.GetComponent<Renderer>().material.color = objectColor;
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(lineHorizontal);
        }
        else
        {
            lineVertical = Instantiate(GameObject.Find(line), Vector3.zero, GameObject.Find(line).transform.rotation) as GameObject;
            lineVertical.name = "temp";
            lineVertical.transform.position = new Vector3(GameObject.Find(line).transform.position.x, 50, GameObject.Find(line).transform.position.z);
            lineVertical.transform.rotation = GameObject.Find(line).transform.rotation;
            lineVertical.GetComponent<Renderer>().enabled = true;// get the object's network ID
            lineVertical.GetComponent<Renderer>().material = lineMat;
            lineVertical.GetComponent<Renderer>().material.color = objectColor;
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
                    if (isLocalPlayer && GetComponent<PlayerID>().isPlayersTurn)
                    {
                        if (lineHorizontal != null)
                            lineHorizontal.GetComponent<Renderer>().enabled = false;
                        if (lineVertical != null)
                            lineVertical.GetComponent<Renderer>().enabled = false;

                        GameObject.Find(objectID).GetComponent<Renderer>().enabled = true;// get the object's network ID
                        GameObject.Find(objectID).GetComponent<Renderer>().material = lineMat;
                        GameObject.Find(objectID).GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
                        GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                        CmdPlaceLine(objectID, objectColor);
                        CheckIfSquareIsMade(hit);
                        CmdStopAnim();
                        animFinished = true;
                        if (!pointScored)
                        {
                            CmdNextTurn();
                        }
                    }
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
                        if (isLocalPlayer && GetComponent<PlayerID>().isPlayersTurn)
                        {
                            if (lineHorizontal != null)
                                lineHorizontal.GetComponent<Renderer>().enabled = false;
                            if (lineVertical != null)
                                lineVertical.GetComponent<Renderer>().enabled = false;
                            GameObject.Find(objectID).GetComponent<Renderer>().enabled = true;// get the object's network ID
                            GameObject.Find(objectID).GetComponent<Renderer>().material = lineMat;
                            GameObject.Find(objectID).GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
                            GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                            CmdPlaceLine(objectID, objectColor);
                            CheckIfSquareIsMade(hit);
                            CmdStopAnim();
                            animFinished = true;
                            if (!pointScored)
                            {
                                CmdNextTurn();
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
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    //Spawn a temporary square for an animation
    void SpawnSquareForAnim(string square)
    {
        centerSquare = Instantiate(GameObject.Find(square), Vector3.zero, GameObject.Find(square).transform.rotation) as GameObject;
        centerSquare.name = "tempSquare";
        centerSquare.transform.position = new Vector3(GameObject.Find(square).transform.position.x, 50, GameObject.Find(square).transform.position.z);
        centerSquare.transform.rotation = GameObject.Find(square).transform.rotation;
        centerSquare.GetComponent<Renderer>().enabled = true;// get the object's network ID
        centerSquare.GetComponent<Renderer>().material = lineMat;
        centerSquare.GetComponent<Renderer>().material.color = squareColor;
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(centerSquare);


    }
    //Play the line animation of falling from the sky and rotating
    IEnumerator StartSquareAnim(string square)
    {
        SpawnSquareForAnim(square);
        while (!squareAnimFinished)
        {
            //Lerp the square location and rotation for a smooth animation
            centerSquare.transform.rotation = Quaternion.Slerp(centerSquare.transform.rotation, Quaternion.Euler(540, 0, 0), rotLerpRate * Time.deltaTime);
            if (centerSquare.transform.position.y > 0)
            {
                centerSquare.transform.position = Vector3.Lerp(centerSquare.transform.position, new Vector3(centerSquare.transform.position.x, 0, centerSquare.transform.position.z), Time.deltaTime * lerpRate);
            }
            if (centerSquare.transform.position.y < 0.01 && !squareAnimFinished)
            {
                if (isLocalPlayer && GetComponent<PlayerID>().isPlayersTurn)
                {
                    if (centerSquare != null)
                        centerSquare.GetComponent<Renderer>().enabled = false;
                    GameObject.Find(squareID).GetComponent<Renderer>().enabled = true;// get the object's network ID
                    GameObject.Find(squareID).GetComponent<Renderer>().material = lineMat;
                    GameObject.Find(squareID).GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
                    CmdStopSquareAnim();
                    CmdNextTurn();
                    squareAnimFinished = true;
                }
            }
            if (squareAnimFinished)
            {
                if (centerSquare != null)
                    centerSquare.GetComponent<Renderer>().enabled = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}

