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
    private bool isPointScored = false;
    private bool doubleSquare = false;
    [SerializeField]
    public GameObject lineHorizontal, lineVertical, centerSquare;
    private GameObject newLineHorizontal, newLineVertical;
    [SerializeField]
    public GameObject line, square;
    [SerializeField]
    private GameObject linePlaceEffect, squarePlaceEffect;
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
    float gravity = 39.2f;//x4 gravity
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
            stream.SendNext(playingSquareAnim);
            stream.SendNext(animFinished);
            stream.SendNext(squareID);

        }
        else
        {
            // Network player, receive data
            this.pointScored = (bool)stream.ReceiveNext();
            this.playingSquareAnim = (bool)stream.ReceiveNext();
            this.animFinished = (bool)stream.ReceiveNext();
            this.squareID = (string)stream.ReceiveNext();
        }
    }
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
        }
    }
    [PunRPC]
    void RpcSameTurn()
    {
        GameObject.Find("GameManager").GetComponent<TurnTimer>().ResetTimer();
        GetComponent<PlayerID>().isPlayersTurn = true;
        pointScored = false;
        playingAnim = false;
        doubleSquare = false;

    }
    [PunRPC]
    void RpcNextTurn()
    {
        GameObject.Find("GameManager").GetComponent<TurnTimer>().nextTurn = true;
        doubleSquare = false;
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
            StartCoroutine("StartSquareAnim", squareID);
        }
    }
    [PunRPC]
    void CmdStopSquareAnim(string squareId, string tempSquare)
    {
        foreach (var square in GameObject.FindGameObjectsWithTag("CenterSquare"))
        {
            square.transform.rotation = Quaternion.identity;
            square.transform.position = new Vector3(square.transform.position.x, 0, square.transform.position.z);
        }

        GameObject.Find(squareID).GetComponent<Renderer>().enabled = true;// get the object's network ID
        GameObject.Find(squareID).GetComponent<Renderer>().material = lineMat;
        GameObject.Find(squareID).GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(squareID).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(squareID).GetComponent<AudioSource>().volume = (GLOBALS.Volume/10);
        GameObject.Find(squareID).GetComponent<AudioSource>().Play();
        //Play Effect
        GameObject squareEffectLeftTop = Instantiate(squarePlaceEffect, new Vector3(GameObject.Find(squareID).transform.position.x - (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(squareID).transform.position.y * 5f), GameObject.Find(squareID).transform.position.z + (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(squareID).transform.rotation) as GameObject;
        squareEffectLeftTop.GetComponent<ParticleSystem>().startColor = squareColor;
        GameObject squareEffectLeftBot = Instantiate(squarePlaceEffect, new Vector3(GameObject.Find(squareID).transform.position.x - (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(squareID).transform.position.y * 5f), GameObject.Find(squareID).transform.position.z - (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(squareID).transform.rotation) as GameObject;
        squareEffectLeftBot.GetComponent<ParticleSystem>().startColor = squareColor;
        GameObject squareEffectRightTop = Instantiate(squarePlaceEffect, new Vector3(GameObject.Find(squareID).transform.position.x + (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(squareID).transform.position.y * 5f), GameObject.Find(squareID).transform.position.z + (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(squareID).transform.rotation) as GameObject;
        squareEffectRightTop.GetComponent<ParticleSystem>().startColor = squareColor;
        GameObject squareEffectRightBot = Instantiate(squarePlaceEffect, new Vector3(GameObject.Find(squareID).transform.position.x + (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(squareID).transform.position.y * 5f), GameObject.Find(squareID).transform.position.z - (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(squareID).transform.rotation) as GameObject;
        squareEffectRightBot.GetComponent<ParticleSystem>().startColor = squareColor;


        GameObject tempEffectLeftTop = Instantiate(squarePlaceEffect, new Vector3(GameObject.Find(tempSquare).transform.position.x - (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(tempSquare).transform.position.y * 5f), GameObject.Find(tempSquare).transform.position.z + (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(tempSquare).transform.rotation) as GameObject;
        tempEffectLeftTop.GetComponent<ParticleSystem>().startColor = squareColor;
        GameObject tempEffectLeftBot = Instantiate(squarePlaceEffect, new Vector3(GameObject.Find(tempSquare).transform.position.x - (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(tempSquare).transform.position.y * 5f), GameObject.Find(tempSquare).transform.position.z - (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(tempSquare).transform.rotation) as GameObject;
        tempEffectLeftBot.GetComponent<ParticleSystem>().startColor = squareColor;
        GameObject tempEffectRightTop = Instantiate(squarePlaceEffect, new Vector3(GameObject.Find(tempSquare).transform.position.x + (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(tempSquare).transform.position.y * 5f), GameObject.Find(tempSquare).transform.position.z + (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(tempSquare).transform.rotation) as GameObject;
        tempEffectRightTop.GetComponent<ParticleSystem>().startColor = squareColor;
        GameObject tempEffectRightBot = Instantiate(squarePlaceEffect, new Vector3(GameObject.Find(tempSquare).transform.position.x + (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(tempSquare).transform.position.y * 5f), GameObject.Find(tempSquare).transform.position.z - (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(tempSquare).transform.rotation) as GameObject;
        tempEffectRightBot.GetComponent<ParticleSystem>().startColor = squareColor;
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(squareEffectLeftTop);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(squareEffectLeftBot);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(squareEffectRightTop);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(squareEffectRightBot); GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(tempEffectLeftTop);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(tempEffectLeftBot);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(tempEffectRightTop);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(tempEffectRightBot);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(GameObject.Find(tempSquare));

        squareAnimFinished = true;
        playingSquareAnim = false;
    }
    [PunRPC]
    void CmdStopAnim()
    {
        animFinished = true;
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
        GameObject.Find(obj).GetComponents<AudioSource>()[0].volume = (GLOBALS.Volume / 175);
        GameObject.Find(obj).GetComponents<AudioSource>()[0].Play();
        if (obj.Contains("Vertical"))
        {
            //Play Effect
            GameObject leftLineEffect = Instantiate(linePlaceEffect, new Vector3(GameObject.Find(obj).transform.position.x + (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(obj).transform.position.y * 2.5f), GameObject.Find(obj).transform.position.z), GameObject.Find(obj).transform.rotation) as GameObject;
            leftLineEffect.GetComponent<ParticleSystem>().startColor = objectColor;

            GameObject rightLineEffect = Instantiate(linePlaceEffect, new Vector3(GameObject.Find(obj).transform.position.x - (GLOBALS.DOTDISTANCE / 2), GameObject.Find(obj).transform.position.y * 2.5f, GameObject.Find(obj).transform.position.z), GameObject.Find(obj).transform.rotation) as GameObject;
            rightLineEffect.GetComponent<ParticleSystem>().startColor = objectColor;
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(leftLineEffect);
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(rightLineEffect);
        }
        else
        {
            //Play Effect
            GameObject leftLineEffect = Instantiate(linePlaceEffect, new Vector3(GameObject.Find(obj).transform.position.x, (GameObject.Find(obj).transform.position.y * 2.5f), GameObject.Find(obj).transform.position.z + (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(obj).transform.rotation) as GameObject;
            leftLineEffect.GetComponent<ParticleSystem>().startColor = objectColor;

            GameObject rightLineEffect = Instantiate(linePlaceEffect, new Vector3(GameObject.Find(obj).transform.position.x, GameObject.Find(obj).transform.position.y * 2.5f, GameObject.Find(obj).transform.position.z - (GLOBALS.DOTDISTANCE / 2)), GameObject.Find(obj).transform.rotation) as GameObject;
            rightLineEffect.GetComponent<ParticleSystem>().startColor = objectColor;
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(leftLineEffect);
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(rightLineEffect);
        }
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
            //Determine if double square
            if (pointScored)
            {
                doubleSquare = true;
            }
            pointScored = true;
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
        if (photonView.isMine && GetComponent<PlayerID>().isPlayersTurn
            && Input.GetMouseButtonDown(0)
            && GameObject.Find("EscapeMenu") != null
            //Escape Menu not open
            && GameObject.Find("EscapeMenu").GetComponent<RectTransform>().localScale == new Vector3(0, 0, 0))
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
                    GameObject.Find(objectID).GetComponents<AudioSource>()[1].volume = GLOBALS.Volume / 125; ;
                    GameObject.Find(objectID).GetComponents<AudioSource>()[1].Play();
                    photonView.RPC("CmdSelectObject", PhotonTargets.AllBuffered, hit.collider.name);
                    photonView.RPC("CmdPlayAnim", PhotonTargets.AllBuffered, hit.collider.name);
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
            newLineHorizontal = Instantiate(lineHorizontal, new Vector3(GameObject.Find(line).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(line).transform.position.z), GameObject.Find(line).transform.rotation) as GameObject;
            newLineHorizontal.name = "temp";
            newLineHorizontal.transform.position = new Vector3(GameObject.Find(line).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(line).transform.position.z);
            newLineHorizontal.transform.rotation = GameObject.Find(line).transform.rotation;
            newLineHorizontal.GetComponent<Renderer>().enabled = true;
            newLineHorizontal.GetComponent<Renderer>().material = lineMat;
            newLineHorizontal.GetComponent<Renderer>().material.SetColor("_MKGlowColor", objectColor);
            newLineHorizontal.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", objectColor);
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(newLineHorizontal);
        }
        else
        {
            newLineVertical = Instantiate(lineVertical, new Vector3(GameObject.Find(line).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(line).transform.position.z), GameObject.Find(line).transform.rotation) as GameObject;
            newLineVertical.name = "temp";
            newLineVertical.transform.position = new Vector3(GameObject.Find(line).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(line).transform.position.z);
            newLineVertical.transform.rotation = GameObject.Find(line).transform.rotation;
            newLineVertical.GetComponent<Renderer>().enabled = true;
            newLineVertical.GetComponent<Renderer>().material = lineMat;
            newLineVertical.GetComponent<Renderer>().material.SetColor("_MKGlowColor", objectColor);
            newLineVertical.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", objectColor);
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(newLineVertical);
        }
    }
    //Play the line animation of falling from the sky and rotating
    IEnumerator StartLineAnim(string line, RaycastHit hit)
    {
        Vector3 velocity = new Vector3(0, 1, 0);
        float rotateSpeed = 0;
        SpawnLineForAnim(line);
        while (!animFinished)
        {
            // apply gravity 

            velocity.y -= gravity * Time.deltaTime;
            rotateSpeed += 10;
            // calculate new position

            transform.position += velocity * Time.deltaTime;
            //Lerp the lines location and rotation for a smooth animation
            if (newLineHorizontal != null && objectID != null && GameObject.Find(objectID).name.Contains("Horizontal"))
            {
                //Perfectly rotate lines so they are at the right position when hitting the ground
                newLineHorizontal.transform.Rotate(120 * Time.deltaTime, 0, 0);
                if (newLineHorizontal.transform.position.y > 0)
                {
                    newLineHorizontal.transform.position += velocity * Time.deltaTime;
                }

                if (newLineHorizontal.transform.position.y < 0.001 && !animFinished)
                {
                    if (photonView.isMine)
                    {
                        if (newLineHorizontal != null)
                            newLineHorizontal.GetComponent<Renderer>().enabled = false;
                        if (newLineVertical != null)
                            newLineVertical.GetComponent<Renderer>().enabled = false;

                        GameObject.Find(objectID).GetComponent<Renderer>().enabled = true;// get the object's network ID
                        GameObject.Find(objectID).GetComponent<Renderer>().material = lineMat;
                        GameObject.Find(objectID).GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
                        GameObject.Find(objectID).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
                        GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                        photonView.RPC("CmdPlaceLine", PhotonTargets.AllBuffered, objectID, objectColor.ToString());
                        animFinished = true;
                        CheckIfSquareIsMade(hit);
                        if (!pointScored)
                        {
                            CmdNextTurn();
                            photonView.RPC("CmdStopAnim", PhotonTargets.AllBuffered);
                        }
                        else
                        {
                            photonView.RPC("CmdStopAnim", PhotonTargets.AllBuffered);
                        }
                    }
                }
                if (animFinished)
                {
                    if (newLineHorizontal != null)
                        newLineHorizontal.GetComponent<Renderer>().enabled = false;
                    if (newLineVertical != null)
                        newLineVertical.GetComponent<Renderer>().enabled = false;
                    GameObject.Find("temp").GetComponent<Renderer>().enabled = false;
                }
            }
            //Lerp vertical line 
            else
            {
                if (newLineVertical != null)
                {
                    //Perfectly rotate lines so they are at the right position when hitting the ground
                    newLineVertical.transform.Rotate(0, 0, 120 * Time.deltaTime);
                    if (newLineVertical.transform.position.y > 0)
                    {
                        newLineVertical.transform.position += velocity * Time.deltaTime;
                    }
                    if (newLineVertical.transform.position.y < 0.001 && !animFinished)
                    {

                        if (photonView.isMine)
                        {
                            if (newLineHorizontal != null)
                                newLineHorizontal.GetComponent<Renderer>().enabled = false;
                            if (newLineVertical != null)
                                newLineVertical.GetComponent<Renderer>().enabled = false;

                            GameObject.Find(objectID).GetComponent<Renderer>().enabled = true;// get the object's network ID
                            GameObject.Find(objectID).GetComponent<Renderer>().material = lineMat;
                            GameObject.Find(objectID).GetComponent<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
                            GameObject.Find(objectID).GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
                            GameObject.Find(objectID).GetComponent<LinePlaced>().linePlaced = true;
                            photonView.RPC("CmdPlaceLine", PhotonTargets.AllBuffered, objectID, objectColor.ToString());
                            animFinished = true;
                            CheckIfSquareIsMade(hit);
                            if (!pointScored)
                            {
                                CmdNextTurn();
                                photonView.RPC("CmdStopAnim", PhotonTargets.AllBuffered);
                            }
                            else
                            {
                                photonView.RPC("CmdStopAnim", PhotonTargets.AllBuffered);
                            }
                        }
                    }

                }
            }
            if (animFinished)
            {
                if (newLineHorizontal != null)
                    newLineHorizontal.GetComponent<Renderer>().enabled = false;
                if (newLineVertical != null)
                    newLineVertical.GetComponent<Renderer>().enabled = false;
                GameObject.Find("temp").GetComponent<Renderer>().enabled = false;
            }
            yield return new WaitForSeconds(0.01f);
        }
        if (animFinished)
        {
            if (newLineHorizontal != null)
                newLineHorizontal.GetComponent<Renderer>().enabled = false;
            if (newLineVertical != null)
                newLineVertical.GetComponent<Renderer>().enabled = false;
            GameObject.Find("temp").GetComponent<Renderer>().enabled = false;
        }
    }
    //Spawn a temporary square for an animation
    GameObject SpawnSquareForAnim(string square)
    {
        GameObject newSquare = Instantiate(centerSquare, new Vector3(GameObject.Find(square).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(square).transform.position.z), GameObject.Find(square).transform.rotation) as GameObject;
        newSquare.name = "tempSquare" + GameObject.Find(square).transform.position.x + "" + GameObject.Find(square).transform.position.z;
        newSquare.transform.position = new Vector3(GameObject.Find(square).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(square).transform.position.z);
        newSquare.transform.rotation = GameObject.Find(square).transform.rotation;
        newSquare.GetComponent<Renderer>().enabled = true;// get the object's network ID
        newSquare.GetComponent<Renderer>().material = lineMat;
        newSquare.GetComponent<Renderer>().material.SetColor("_MKGlowColor", squareColor);
        newSquare.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", squareColor);
        return newSquare;
    }
    //Play the square animation of falling from the sky and rotating
    IEnumerator StartSquareAnim(string square)
    {
        Vector3 velocity = new Vector3(0, 1, 0);
        GameObject newSquare = SpawnSquareForAnim(square);
        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(newSquare);
        while (!squareAnimFinished && newSquare != null)
        {
            // apply gravity 

            velocity.y -= gravity * Time.deltaTime;
            //Lerp the square location and rotation for a smooth animation
            newSquare.transform.Rotate(120 * Time.deltaTime, 0, 0);
            if (newSquare.transform.position.y > 0)
            {
                newSquare.transform.position += velocity * Time.deltaTime;
            }
            if (newSquare.transform.position.y < 1 && !squareAnimFinished && photonView.isMine)
            {
                newSquare.transform.rotation = Quaternion.identity;
                photonView.RPC("CmdStopSquareAnim", PhotonTargets.AllBuffered, squareID, newSquare.name);
                CmdNextTurn();
                squareAnimFinished = true;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
}

