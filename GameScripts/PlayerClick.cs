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
    private bool doubleSquare = false;
    [SerializeField]
    public GameObject lineHorizontal, lineVertical, centerSquare;
    private GameObject newLineHorizontal, newLineVertical;
    [SerializeField]
    public GameObject line, square;
    [SerializeField]
    private GameObject linePlaceEffect, squarePlaceEffect;
    private GameObject leftLineEffect, rightLineEffect, squareEffectLeftTop, squareEffectRightTop, squareEffectLeftBot, squareEffectRightBot;
    private GameObject escapeMenu, eventPanel, playAgainMenu, gameManager;
    private Ray ray;

    /*
     * Sync Line position
     */
    public bool animFinished = false;
    public bool squareAnimFinished = false;
    public bool playingAnim = false;
    public bool playingSquareAnim = false;
    float gravity = 39.2f;//x4 gravity
    /*
     * Sync Line rotation
     */
    private float lastLineRot;

    private new PhotonView photonView;
    void Start()
    {
        scores = GameObject.FindGameObjectsWithTag("ScoreText");
        escapeMenu = GameObject.Find("EscapeMenu");
        gameManager = GameObject.Find("GameManager");
        eventPanel = GameObject.Find("EventPanel");
        playAgainMenu = GameObject.Find("PlayAgainMenu");
        photonView = this.GetComponent<PhotonView>();

        leftLineEffect = Instantiate(linePlaceEffect, new Vector3(999, 999, 999), Quaternion.identity) as GameObject;
        leftLineEffect.GetComponent<Renderer>().enabled = false;
        rightLineEffect = Instantiate(linePlaceEffect, new Vector3(999, 999, 999), Quaternion.identity) as GameObject;
        rightLineEffect.GetComponent<Renderer>().enabled = false;

        squareEffectLeftTop = Instantiate(squarePlaceEffect, new Vector3(999, 999, 999), Quaternion.identity) as GameObject;
        squareEffectLeftTop.GetComponent<Renderer>().enabled = false;

        squareEffectRightTop = Instantiate(squarePlaceEffect, new Vector3(999, 999, 999), Quaternion.identity) as GameObject;
        squareEffectRightTop.GetComponent<Renderer>().enabled = false;

        squareEffectRightBot = Instantiate(squarePlaceEffect, new Vector3(999, 999, 999), Quaternion.identity) as GameObject;
        squareEffectRightBot.GetComponent<Renderer>().enabled = false;

        squareEffectLeftBot = Instantiate(squarePlaceEffect, new Vector3(999, 999, 999), Quaternion.identity) as GameObject;
        squareEffectLeftBot.GetComponent<Renderer>().enabled = false;
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(pointScored);
            stream.SendNext(squareID);

        }
        else
        {
            // Network player, receive data
            this.pointScored = (bool)stream.ReceiveNext();
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
        GameObject.Find(obj).GetComponentInChildren<Renderer>().enabled = true;
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_RimColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponent<LinePlaced>().linePlaced = true;
    }

    [PunRPC]
    void RpcPaintSameTurn(string obj, string col)
    {
        GameObject.Find(obj).GetComponentInChildren<Renderer>().enabled = true;
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_RimColor", ColorExtensions.ParseColor(col));
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

        GameObject.Find(squareID).GetComponentInChildren<Renderer>().enabled = true;// get the object's network ID
        GameObject.Find(squareID).GetComponentInChildren<Renderer>().material = lineMat;
        GameObject.Find(squareID).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(squareID).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(squareID).GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(squareID).GetComponent<AudioSource>().volume = (GLOBALS.Volume * 1.5f);
        GameObject.Find(squareID).GetComponent<AudioSource>().Play();
        //Play Effect
        squareEffectLeftTop.GetComponent<Renderer>().enabled = true;
        squareEffectLeftTop.transform.position = new Vector3(GameObject.Find(squareID).transform.position.x - (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(squareID).transform.position.y * 5f), GameObject.Find(squareID).transform.position.z + (GLOBALS.DOTDISTANCE / 2));
        var main = squareEffectLeftTop.GetComponent<ParticleSystem>().main;
        main.startColor = squareColor;
        squareEffectLeftTop.GetComponent<ParticleSystem>().Play();

        squareEffectLeftBot.GetComponent<Renderer>().enabled = true;
        squareEffectLeftBot.transform.position = new Vector3(GameObject.Find(squareID).transform.position.x - (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(squareID).transform.position.y * 5f), GameObject.Find(squareID).transform.position.z - (GLOBALS.DOTDISTANCE / 2));
        main = squareEffectLeftBot.GetComponent<ParticleSystem>().main;
        main.startColor = squareColor;
        squareEffectLeftBot.GetComponent<ParticleSystem>().Play();

        squareEffectRightTop.GetComponent<Renderer>().enabled = true;
        squareEffectRightTop.transform.position = new Vector3(GameObject.Find(squareID).transform.position.x + (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(squareID).transform.position.y * 5f), GameObject.Find(squareID).transform.position.z + (GLOBALS.DOTDISTANCE / 2));
        main = squareEffectRightTop.GetComponent<ParticleSystem>().main;
        main.startColor = squareColor;
        squareEffectRightTop.GetComponent<ParticleSystem>().Play();

        squareEffectRightBot.GetComponent<Renderer>().enabled = true;
        squareEffectRightBot.transform.position = new Vector3(GameObject.Find(squareID).transform.position.x + (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(squareID).transform.position.y * 5f), GameObject.Find(squareID).transform.position.z - (GLOBALS.DOTDISTANCE / 2));
        main = squareEffectRightBot.GetComponent<ParticleSystem>().main;
        main.startColor = squareColor;
        squareEffectRightBot.GetComponent<ParticleSystem>().Play();

        GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(GameObject.Find(tempSquare));

        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player.GetComponent<PlayerID>().playerScore >= GLOBALS.POINTSTOWIN)
            {
                GameObject.Find("GameManager").GetComponent<GameOver>().gameOver = true;
                if (photonView.isMine)
                {
                    GameObject.Find("EventText").GetComponent<Text>().text = "Congrats, You won the game!";
                }
                else
                {
                    GameObject.Find("EventText").GetComponent<Text>().text = "You lost!";
                }
                GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Show(false);
                break;
            }
        }


        squareAnimFinished = true;
        playingSquareAnim = false;
    }
    [PunRPC]
    void CmdStopAnim()
    {
        playingAnim = false;
        animFinished = true;
    }
    //On click place a line at the mouse location
    [PunRPC]
    void CmdPlaceLine(string obj, string col)
    {
        GameObject.Find(obj).GetComponentInChildren<Renderer>().enabled = true;
        //GameObject.Find(obj).GetComponentInParent<Light>().enabled = true;
        GameObject.Find(obj).GetComponentInParent<Light>().color = GetComponent<PlayerColor>().playerColor;
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material = lineMat;
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponentInChildren<Renderer>().material.SetColor("_RimColor", ColorExtensions.ParseColor(col));
        GameObject.Find(obj).GetComponent<LinePlaced>().linePlaced = true;
        GameObject.Find(obj).GetComponents<AudioSource>()[0].volume = (GLOBALS.Volume / 175);
        GameObject.Find(obj).GetComponents<AudioSource>()[0].Play();
        leftLineEffect.GetComponent<Renderer>().enabled = true;
        rightLineEffect.GetComponent<Renderer>().enabled = true;
        if (obj.Contains("Vertical"))
        {

            //Play Effect
            leftLineEffect.transform.position = new Vector3(GameObject.Find(obj).transform.position.x + (GLOBALS.DOTDISTANCE / 2), (GameObject.Find(obj).transform.position.y * 2.5f), GameObject.Find(obj).transform.position.z);
            var main = leftLineEffect.GetComponent<ParticleSystem>().main;
            main.startColor = objectColor;
            leftLineEffect.GetComponent<ParticleSystem>().Play();

            rightLineEffect.transform.position = new Vector3(GameObject.Find(obj).transform.position.x - (GLOBALS.DOTDISTANCE / 2), GameObject.Find(obj).transform.position.y * 2.5f, GameObject.Find(obj).transform.position.z);
            main = rightLineEffect.GetComponent<ParticleSystem>().main;
            main.startColor = objectColor;
            rightLineEffect.GetComponent<ParticleSystem>().Play();
        }
        else
        {

            //Play Effect
            leftLineEffect.transform.position = new Vector3(GameObject.Find(obj).transform.position.x, (GameObject.Find(obj).transform.position.y * 2.5f), GameObject.Find(obj).transform.position.z + (GLOBALS.DOTDISTANCE / 2));
            var main = leftLineEffect.GetComponent<ParticleSystem>().main;
            main.startColor = objectColor;
            leftLineEffect.GetComponent<ParticleSystem>().Play();

            rightLineEffect.transform.position = new Vector3(GameObject.Find(obj).transform.position.x, GameObject.Find(obj).transform.position.y * 2.5f, GameObject.Find(obj).transform.position.z - (GLOBALS.DOTDISTANCE / 2));
            main = rightLineEffect.GetComponent<ParticleSystem>().main;
            main.startColor = objectColor;
            rightLineEffect.GetComponent<ParticleSystem>().Play();
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
                foreach (var scoreTxt in scores)
                {
                    if (scoreTxt.name.Contains((i + 1).ToString()))
                    {
                        //Update UI with score
                        scoreTxt.GetComponent<Text>().text = score.ToString();
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
        if (hit.collider
            && hit.collider.GetComponentInChildren<Renderer>())
        {
            if (hit.collider.name.Contains("linesHorizontal")
            && hit.collider.GetComponentInChildren<Renderer>().enabled)
            {

                Vector3 centerOfSquareRight = new Vector3(hit.collider.transform.localPosition.x + (GLOBALS.DOTDISTANCE / 2), hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z);
                hitCollidersRight = Physics.OverlapSphere(centerOfSquareRight, squareRadius);
                int i = 0;
                int howManyLines = 0;
                //In the hitbox, check how many lines
                while (i < hitCollidersRight.Length)
                {
                    if (hitCollidersRight[i].name.Contains("lines") && hitCollidersRight[i].GetComponentInChildren<Renderer>().enabled
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
                    if (hitCollidersLeft[j].name.Contains("lines") && hitCollidersLeft[j].GetComponentInChildren<Renderer>().enabled
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
            if (hit.collider.name.Contains("linesVertical") && hit.collider.GetComponentInChildren<Renderer>().enabled)
            {

                Vector3 centerOfSquareBottom = new Vector3(hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.y, hit.collider.transform.localPosition.z + (GLOBALS.DOTDISTANCE / 2));
                hitCollidersBottom = Physics.OverlapSphere(centerOfSquareBottom, squareRadius);
                int i = 0;
                int howManyLines = 0;
                while (i < hitCollidersBottom.Length)
                {
                    if (hitCollidersBottom[i].name.Contains("lines") && hitCollidersBottom[i].GetComponentInChildren<Renderer>().enabled
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
                    if (hitCollidersTop[j].name.Contains("lines") && hitCollidersTop[j].GetComponentInChildren<Renderer>().enabled
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
            squareLines[i].GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
            squareLines[i].GetComponentInChildren<Renderer>().material.SetColor("_MKTexColor", GetComponent<PlayerColor>().playerColor);
            squareLines[i].GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GetComponent<PlayerColor>().playerColor);
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
            if (!line.name.Contains("temp") && !line.name.Contains("Centre") && !line.name.Contains("Cube"))
                photonView.RPC("CmdPaintLines", PhotonTargets.AllBuffered, line.name);
            else if (line.name.Contains("temp"))
            {
                line.GetComponentInChildren<Renderer>().enabled = false;
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
            square.GetComponentInChildren<Renderer>().material = lineMat;
            //square.GetComponent<Light>().enabled = true;
            square.GetComponent<Light>().color = GetComponent<PlayerColor>().playerColor;
            square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
            square.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
            square.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GetComponent<PlayerColor>().playerColor);
            //Determine if double square
            if (pointScored)
            {
                doubleSquare = true;
            }
            pointScored = true;
        }
    }
    //Paint the square the player made
    [PunRPC]
    void CmdPaintLines(string line)
    {
        if (GameObject.Find(line).GetComponentInParent<Light>() != null)
        {
            //GameObject.Find(line).GetComponentInParent<Light>().enabled = true;
            GameObject.Find(line).GetComponentInParent<Light>().color = GetComponent<PlayerColor>().playerColor;
        }
        GameObject.Find(line).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(line).GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", GetComponent<PlayerColor>().playerColor);
        GameObject.Find(line).GetComponentInChildren<Renderer>().material.SetColor("_RimColor", GetComponent<PlayerColor>().playerColor);

    }

    // Update is called once per frame
    void Update()
    {
        //Must be the players turn to place a line
        if (photonView.isMine && GetComponent<PlayerID>().isPlayersTurn
            && Input.GetMouseButtonDown(0)
            && escapeMenu != null
            //Escape Menu not open
            && escapeMenu.GetComponent<RectTransform>().localScale == new Vector3(0, 0, 0))
        {
            //empty RaycastHit object which raycast puts the hit details into
            hit = new RaycastHit();
            //ray shooting out of the camera from where the mouse is
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Raycast from the mouse to the level, if hit place a line
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name.Contains("line")
                    && hit.collider.GetComponent<LinePlaced>().linePlaced == false
                    && !playingAnim
                    && !gameManager.GetComponent<GameOver>().gameOver
                    && !gameManager.GetComponent<GameStart>().buildGrid
                    && !eventPanel.GetComponent<DoozyUI.UIElement>().isVisible
                    && !playAgainMenu.GetComponent<DoozyUI.UIElement>().isVisible)
                {
                    objectID = hit.collider.name;// this gets the object that is hit
                    hit.collider.GetComponentInChildren<Renderer>().enabled = false;
                    objectColor = GetComponent<PlayerColor>().playerColor;
                    GameObject.Find(objectID).GetComponents<AudioSource>()[1].volume = GLOBALS.Volume / 100;
                    GameObject.Find(objectID).GetComponents<AudioSource>()[1].Play();
                    photonView.RPC("CmdSelectObject", PhotonTargets.AllBuffered, hit.collider.name);
                    photonView.RPC("CmdPlayAnim", PhotonTargets.AllBuffered, hit.collider.name);
                }
            }
            GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Hide(false);
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
            newLineHorizontal.GetComponentInChildren<Renderer>().enabled = true;
            newLineHorizontal.GetComponentInChildren<Renderer>().material = lineMat;
            newLineHorizontal.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", objectColor);
            newLineHorizontal.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", objectColor);
            newLineHorizontal.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", objectColor);
            //newLineHorizontal.GetComponent<Light>().enabled = true;
            newLineHorizontal.GetComponent<Light>().color = objectColor;
            GameObject.Find("GameManager").GetComponent<GameStart>().objectsToDelete.Add(newLineHorizontal);
        }
        else
        {
            newLineVertical = Instantiate(lineVertical, new Vector3(GameObject.Find(line).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(line).transform.position.z), GameObject.Find(line).transform.rotation) as GameObject;
            newLineVertical.name = "temp";
            newLineVertical.transform.position = new Vector3(GameObject.Find(line).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(line).transform.position.z);
            newLineVertical.transform.rotation = GameObject.Find(line).transform.rotation;
            newLineVertical.GetComponentInChildren<Renderer>().enabled = true;
            newLineVertical.GetComponentInChildren<Renderer>().material = lineMat;
            newLineVertical.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", objectColor);
            newLineVertical.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", objectColor);
            newLineVertical.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", objectColor);
            //newLineVertical.GetComponent<Light>().enabled = true;
            newLineVertical.GetComponent<Light>().color = objectColor;
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

                if (newLineHorizontal.transform.position.y < 0.00001f && !animFinished)
                {
                    if (photonView.isMine)
                    {
                        photonView.RPC("CmdPlaceLine", PhotonTargets.AllBuffered, objectID, objectColor.ToString());
                        CheckIfSquareIsMade(hit);
                        if (!pointScored)
                            CmdNextTurn();
                    }
                    animFinished = true;

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
                    if (newLineVertical.transform.position.y < 0.00001f && !animFinished)
                    {
                        if (photonView.isMine)
                        {
                            photonView.RPC("CmdPlaceLine", PhotonTargets.AllBuffered, objectID, objectColor.ToString());
                            CheckIfSquareIsMade(hit);
                            if (!pointScored)
                                CmdNextTurn();
                        }
                        animFinished = true;

                    }

                }
            }
            yield return new WaitForSeconds(0.0001f);
        }
        if (animFinished)
        {
            if (newLineHorizontal != null)
            {
                newLineHorizontal.GetComponent<Light>().enabled = false;
                newLineHorizontal.GetComponentInChildren<Renderer>().enabled = false;
            }
            if (newLineVertical != null)
            {
                newLineVertical.GetComponent<Light>().enabled = false;
                newLineVertical.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
    }
    //Spawn a temporary square for an animation
    GameObject SpawnSquareForAnim(string square)
    {
        GameObject newSquare = Instantiate(centerSquare, new Vector3(GameObject.Find(square).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(square).transform.position.z), GameObject.Find(square).transform.rotation) as GameObject;
        newSquare.name = "tempSquare" + GameObject.Find(square).transform.position.x + "" + GameObject.Find(square).transform.position.z;
        newSquare.transform.position = new Vector3(GameObject.Find(square).transform.position.x, GLOBALS.LINEHEIGHT, GameObject.Find(square).transform.position.z);
        newSquare.transform.rotation = GameObject.Find(square).transform.rotation;
        newSquare.GetComponentInChildren<Renderer>().enabled = true;// get the object's network ID
        newSquare.GetComponentInChildren<Renderer>().material = lineMat;
        newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", squareColor);
        newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", squareColor);
        newSquare.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", squareColor);
        //newSquare.GetComponent<Light>().enabled = true;
        newSquare.GetComponent<Light>().color = objectColor;

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
            if (newSquare.transform.position.y < 0.0001 && !squareAnimFinished && photonView.isMine)
            {
                newSquare.transform.rotation = Quaternion.identity;
                newSquare.GetComponent<Light>().enabled = false;
                CmdNextTurn();
                photonView.RPC("CmdStopSquareAnim", PhotonTargets.AllBuffered, squareID, newSquare.name);
                if (!GameObject.Find("GameManager").GetComponent<GameOver>().gameOver)
                {
                    if (doubleSquare)
                    {
                        GameObject.Find("EventText").GetComponent<Text>().text = "Double square! Place another line.";
                        GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Show(false);
                    }
                    else
                    {
                        GameObject.Find("EventText").GetComponent<Text>().text = "Well done! Place another line.";
                        GameObject.Find("EventPanel").GetComponent<DoozyUI.UIElement>().Show(false);
                    }
                }
                squareAnimFinished = true;
            }
            yield return new WaitForSeconds(0.0001f);
        }
    }
}

