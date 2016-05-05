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
    private Color objectColor;
    [SyncVar]
    private GameObject objectID;
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
    //Sync Line position
    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;

    private float lerpRate;
    private float normalLerpRate = 5;
    private float fasterLerpRate = 6;

    private Vector3 lastPos;
    private float threshold = 0.5f;

    private bool animFinished = false;
    private bool playingAnim = false;
    private List<Vector3> syncPosList = new List<Vector3>();
    private float closeEnough = 0.01f;
    //Sync Line rotation
    [SyncVar(hook = "OnLineRotSynced")]
    private float syncLineRotation;

    private float rotLerpRate = 6;

    private float lastLineRot;
    private float rotThreshold = 0.01f;

    private List<float> syncLineRotList = new List<float>();
    private float rotCloseEnough = 0.4f;
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
    void RpcPaint(GameObject obj, Color col)
    {
        obj.GetComponent<Renderer>().enabled = true;
        obj.GetComponent<Renderer>().material = lineMat;
        obj.GetComponent<Renderer>().material.color = col;      // this is the line that actually makes the change in color happen
        obj.GetComponent<LinePlaced>().linePlaced = true;
    }

    [ClientRpc]
    void RpcPaintSameTurn(GameObject obj, Color col)
    {
        obj.GetComponent<Renderer>().enabled = true;
        obj.GetComponent<Renderer>().material = lineMat;
        obj.GetComponent<Renderer>().material.color = col;      // this is the line that actually makes the change in color happen
        obj.GetComponent<LinePlaced>().linePlaced = true;
        GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
        GetComponent<PlayerID>().isPlayersTurn = true;
    }

    //On click place a line at the mouse location
    [Command]
    void CmdPlaceLine(GameObject obj, Color col)
    {
        objNetId = obj.GetComponent<NetworkIdentity>();
        obj.GetComponent<Renderer>().enabled = true;// get the object's network ID
        obj.GetComponent<Renderer>().material = lineMat;
        obj.GetComponent<Renderer>().material.color = col;
        obj.GetComponent<LinePlaced>().linePlaced = true;
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
        while (i < squareLines.Length)
        {
            GameObject line = squareLines[i].gameObject;
            objID = squareLines[i].GetComponent<NetworkIdentity>().netId;
            objNetId = squareLines[i].GetComponent<NetworkIdentity>();
            squareLines[i].GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
            CmdPaintSquare(line);
            i++;
        }

    }

    [Command]
    void CmdPaintSquare(GameObject line)
    {
        line.GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
        pointScored = true;
        //objNetId.RemoveClientAuthority(objNetId.connectionToClient);
        //objNetId.AssignClientAuthority(connectionToClient);
        RpcPaintSquare(line);

    }

    [ClientRpc]
    void RpcPaintSquare(GameObject line)
    {

        line.GetComponent<Renderer>().material.color = GetComponent<PlayerColor>().playerColor;
        //objNetId.RemoveClientAuthority(objNetId.connectionToClient);
        //objNetId.AssignClientAuthority(connectionToClient);
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
                    RaycastHit hit = new RaycastHit();
                    //ray shooting out of the camera from where the mouse is
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //Raycast from the mouse to the level, if hit place a line
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.name.Contains("line") && hit.collider.GetComponent<LinePlaced>().linePlaced == false && !playingAnim)
                        {
                            objectID = GameObject.Find(hit.collider.name);// this gets the object that is hit
                            objectColor = GetComponent<PlayerColor>().playerColor;
                            objectID.GetComponent<LinePlaced>().linePlaced = true;
                            CheckIfSquareIsMade(hit);
                            CmdPlaceLine(objectID, objectColor);
                            objectID.transform.position = new Vector3(objectID.transform.position.x, GLOBALS.LINEHEIGHT, objectID.transform.position.z);
                            animFinished = false;
                        }
                    }
                }
            }
        }
        //Lerp the lines location and rotation for a smooth animation
        if (isLocalPlayer)
        {
            if (objectID != null && !animFinished)
            //if (objectID != null)
            {
                playingAnim = true;
                //if (syncLineRotList.Count > 0)
                //{
                    if (objectID.name.Contains("Horizontal"))
                    {
                        objectID.transform.rotation = Quaternion.Slerp(objectID.transform.rotation, Quaternion.Euler(540, 0, 0), rotLerpRate * Time.deltaTime);
                        //if (Mathf.Abs(objectID.transform.localEulerAngles.x - syncLineRotList[0]) < rotCloseEnough)
                        //{
                          //  syncLineRotList.RemoveAt(0);
                        //}
                    }
                    else
                    {
                        objectID.transform.rotation = Quaternion.Slerp(objectID.transform.rotation, Quaternion.Euler(0, 0, 540), rotLerpRate * Time.deltaTime);
                        //if (Mathf.Abs(objectID.transform.localEulerAngles.z - syncLineRotList[0]) < rotCloseEnough)
                        //{
                          //  syncLineRotList.RemoveAt(0);
                        //}
                    }


                //}
                if (syncPosList.Count > 0)
                {
                    objectID.transform.position = Vector3.Lerp(objectID.transform.position, new Vector3(objectID.transform.position.x, 0, objectID.transform.position.z), Time.deltaTime * lerpRate);

                    if (Vector3.Distance(objectID.transform.position, syncPosList[0]) < closeEnough)
                    {
                        syncPosList.RemoveAt(0);
                    }

                    if (syncPosList.Count > 10)
                    {
                        lerpRate = fasterLerpRate;
                    }
                    else
                    {
                        lerpRate = normalLerpRate;
                    }

                }

            }
        }
        if (objectID != null)
        {
            if (objectID.transform.position.y <0.1 && !animFinished)
            {
                Debug.Log("true");
                animFinished = true;
                playingAnim = false;
                if (isLocalPlayer)
                    CmdNextTurn();
            }
        }
    }
    void FixedUpdate()
    {
        TransmitPosition();
        TransmitRotations();
    }
    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }
    [ClientCallback]
    void TransmitPosition()
    {
        if (objectID != null)
        {
            if (isLocalPlayer && Vector3.Distance(objectID.transform.position, lastPos) > threshold)
            {
                CmdProvidePositionToServer(objectID.transform.position);
                lastPos = objectID.transform.position;
            }
        }
    }

    [Client]
    void SyncPositionValues(Vector3 latestPos)
    {
        syncPos = latestPos;
        syncPosList.Add(syncPos);
    }

    [Command]
    void CmdProvideRotationsToServer(float lineRot)
    {
        syncLineRotation = lineRot;
    }

    [ClientCallback]
    void TransmitRotations()
    {
        if (objectID != null)
        {
            if (isLocalPlayer)
            {
                CmdProvideRotationsToServer(objectID.transform.localEulerAngles.z);
            }
        }
    }
    [Client]
    void OnLineRotSynced(float latestLineRot)
    {
        syncLineRotation = latestLineRot;
        syncLineRotList.Add(syncLineRotation);
    }
}

