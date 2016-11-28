using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon;
using ExitGames.Client.Photon;
using System;
using System.Text;


public class GameStart : PunBehaviour
{
    [SerializeField]
    public GameObject dots, lineHor, lineVert, centerSquare;
    public GameObject[] listOfDots = new GameObject[GLOBALS.GRIDHEIGHT * GLOBALS.GRIDWIDTH];
    public GameObject hoverLineHor, hoverLineVert;
    public List<string> playerNames = new List<string>();
    //The speed at which each dot in the grid spawns

    public float spawnSpeed = 0.1f;

    public bool buildGrid = false;
    public bool startGame = false;
    //Sync the rotation and scale,PhotonNetwork.Spawn only sync position
    public Quaternion lineVertRot;
    public Quaternion lineHorRot;
    public Vector3 lineVertScale;
    public Vector3 lineHorScale;
    public Vector3 squareScale;
    public List<GameObject> objectsToDelete = new List<GameObject>();
    private new PhotonView photonView;
    private int viewID;
    private Color greyedPanel = new Color(0.5f, 0.5f, 0.5f, 0.6f);

    void Start()
    {
        photonView = this.GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(startGame);
            stream.SendNext(squareScale);
            stream.SendNext(lineVertScale);
            stream.SendNext(lineHorScale);
            stream.SendNext(lineVertRot);
            stream.SendNext(lineHorRot);
        }
        else
        {
            // Network player, receive data
            this.startGame = (bool)stream.ReceiveNext();
            this.squareScale = (Vector3)stream.ReceiveNext();
            this.lineVertScale = (Vector3)stream.ReceiveNext();
            this.lineHorScale = (Vector3)stream.ReceiveNext();
            this.lineVertRot = (Quaternion)stream.ReceiveNext();
            this.lineHorRot = (Quaternion)stream.ReceiveNext();
        }
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(GLOBALS.GAMESTARTDELAY);
        StartCoroutine(CreateGrid());
    }
    void Update()
    {
        //if (startGame && PhotonNetwork.isMasterClient)
        if (startGame)
        {
            photonView.RPC("BuildGridText", PhotonTargets.AllBuffered);
            GetComponent<GameState>().gameState = GameState.State.BuildingGrid;
            StartCoroutine(StartGame());
            //Build the grid of dots
            //Hide temporary lines
            lineHor.GetComponentInChildren<Renderer>().enabled = false;
            lineVert.GetComponentInChildren<Renderer>().enabled = false;
            //centerSquareuare.GetComponent<Renderer>().enabled = false;
            startGame = false;
        }
    }

    void AssignTurnsAndColors()
    {
        if (PhotonNetwork.isMasterClient)
        {
            //Assign each player a random turn order
            var players = GameObject.FindGameObjectsWithTag("Player");
            var rnd = new System.Random();
            var randomNumbers = Enumerable.Range(1, GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count).OrderBy(x => rnd.Next()).Take(GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count).ToArray();
            int i = 0;
            GetComponent<PlayerTurn>().AssignTurns();
            foreach (var player in players)
            {
                player.GetComponent<PlayerID>().playerTurnOrder = GetComponent<PlayerTurn>().assortPlayerTurns[randomNumbers.ElementAt(i)];
                player.GetComponent<PlayerColor>().playerColor = player.GetComponent<PlayerColor>().colors[randomNumbers.ElementAt(i)];
                player.GetComponent<PlayerColor>().CmdTellServerMyColor(player.GetComponent<PlayerColor>().playerColor);
                //Set the first players turn
                if (player.GetComponent<PlayerID>().playerTurnOrder == 1)
                {
                    player.GetComponent<PlayerID>().isPlayersTurn = true;
                    photonView.RPC("RpcSetFirstTurn", PhotonTargets.AllBuffered, player.name);
                }
                else
                {
                    player.GetComponent<PlayerID>().isPlayersTurn = false;
                    photonView.RPC("RpcDisableTurn", PhotonTargets.AllBuffered, player.name);
                }
                //Only 4 people per game
                if (i != GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count)
                    i++;
            }
        }
    }
    //Tell the server that we spawned a line or dot
    void SpawnOnNetwork(string objName, Vector3 pos, Quaternion rot, string name)
    {
        GameObject newObj = null;
        switch (objName)
        {
            case "dot":
                newObj = PhotonNetwork.Instantiate("Prefabs/Dots", pos, rot, 0);
                newObj.GetComponent<DotID>().CmdSetName(name);
                photonView.RPC("EnableDotLight", PhotonTargets.AllBuffered, newObj.name);
                break;
            case "lineHor":
                newObj = PhotonNetwork.Instantiate("Prefabs/LineHor", pos, rot, 0);
                newObj.GetComponent<LineID>().CmdSetName(name);
                break;
            case "lineVert":
                newObj = PhotonNetwork.Instantiate("Prefabs/LineVert", pos, rot, 0);
                newObj.GetComponent<LineID>().CmdSetName(name);
                break;
            case "centerSquare":
                newObj = PhotonNetwork.Instantiate("Prefabs/CenterSquare", pos, rot, 0);
                newObj.GetComponent<SquareID>().CmdSetName(name);
                break;
        }
        // Set objects PhotonView
        if (newObj != null)
        {
            objectsToDelete.Add(newObj);
        }
    }
    [PunRPC]
    void EnableDotLight(string dot)
    {
        GameObject.Find(dot).GetComponent<Light>().enabled = true;
    }
    //Build the grid
    IEnumerator CreateGrid()
    {
        int index = 0;
        if (PhotonNetwork.isMasterClient)
        {
            for (int x = 0; x < GLOBALS.GRIDWIDTH; x++)
            {
                yield return new WaitForSeconds(spawnSpeed);

                for (int z = 0; z < GLOBALS.GRIDHEIGHT; z++)
                {
                    yield return new WaitForSeconds(spawnSpeed);
                    //Spawn dot
                    if (photonView.isMine)
                    {
                        dots.transform.localPosition = new Vector3(x * GLOBALS.DOTDISTANCE, 0, z * GLOBALS.DOTDISTANCE);
                        dots.transform.localScale = new Vector3(3, 3, 3);
                        dots.name = "Dot " + x.ToString() + "," + z.ToString();
                        dots.GetComponentInChildren<DotID>().dotID = dots.name;
                        listOfDots[index] = dots;
                        index++;
                        SpawnOnNetwork("dot", dots.transform.localPosition, Quaternion.Euler(90, 0, 0), dots.name);
                    }
                    //This if statement stops from building extra unnecessary lines
                    if (z < GLOBALS.GRIDHEIGHT - 1)
                    {
                        //Spawn line in between dots horizontally
                        lineHor.transform.localPosition = new Vector3(x * GLOBALS.DOTDISTANCE, 0, dots.transform.localPosition.z + (GLOBALS.DOTDISTANCE / 2.0f));
                        lineHorScale = new Vector3(3, 3, GLOBALS.DOTDISTANCE - dots.transform.localScale.z + 0.5f);
                        lineHorRot = Quaternion.Euler(0, 0, 0);
                        lineHor.name = "linesHorizontal " + x.ToString() + "," + z.ToString();
                        lineHor.GetComponent<LineID>().lineID = lineHor.name;
                        lineHor.transform.localScale = lineHorScale;
                        lineHor.transform.rotation = lineHorRot;
                        SpawnOnNetwork("lineHor", lineHor.transform.localPosition, Quaternion.Euler(0, 0, 0), lineHor.name);
                    }
                    if (x < GLOBALS.GRIDWIDTH - 1)
                    {
                        //Spawn line in between dots vertically
                        lineVert.transform.localPosition = new Vector3(dots.transform.localPosition.x + (GLOBALS.DOTDISTANCE / 2.0f), 0, z * GLOBALS.DOTDISTANCE);
                        lineVertScale = new Vector3(GLOBALS.DOTDISTANCE - dots.transform.localScale.z + 0.5f, 3, 3);
                        lineVertRot = Quaternion.Euler(0, 0, 0);
                        lineVert.name = "linesVertical " + x.ToString() + "," + z.ToString();
                        lineVert.GetComponent<LineID>().lineID = lineVert.name;
                        lineVert.transform.localScale = lineVertScale;
                        lineVert.transform.rotation = lineVertRot;
                        SpawnOnNetwork("lineVert", lineVert.transform.localPosition, Quaternion.Euler(0, 0, 0), lineVert.name);
                    }
                    //Spawn the center of a square
                    if (x < GLOBALS.GRIDWIDTH - 1 && z < GLOBALS.GRIDHEIGHT - 1)
                    {
                        centerSquare.transform.localPosition = new Vector3(dots.transform.localPosition.x + (GLOBALS.DOTDISTANCE / 2.0f), 0, dots.transform.localPosition.z + (GLOBALS.DOTDISTANCE / 2.0f));
                        squareScale = new Vector3(1f, 1f, 1f);
                        centerSquare.transform.localScale = squareScale;
                        centerSquare.name = "CentreSquare " + x.ToString() + "," + z.ToString();
                        centerSquare.GetComponent<SquareID>().squareID = centerSquare.name;
                        centerSquare.GetComponentInChildren<Renderer>().enabled = false;
                        SpawnOnNetwork("centerSquare", centerSquare.transform.localPosition, centerSquare.transform.localRotation, centerSquare.name);
                    }
                }
            }

            GameObject.Find("Camera").transform.position = new Vector3(
                (listOfDots[listOfDots.Length - 1].transform.position.x) / 2,
        GameObject.Find("Camera").transform.position.y,
        GameObject.Find("Camera").transform.position.z);
            //Start the timer after the grid has been built
            gameObject.GetComponent<TurnTimer>().enabled = true;
            photonView.RPC("RpcEnableTimer", PhotonTargets.AllBuffered);
            AssignTurnsAndColors();
            GetComponent<GameState>().gameState = GameState.State.InProgress;
            photonView.RPC("GridCompleted", PhotonTargets.AllBuffered);
        }
    }

    //Destroy the grid
    public void DestroyGrid()
    {
        foreach (var obj in objectsToDelete)
        {
            if (obj.GetComponent<PhotonView>() != null)
                PhotonNetwork.Destroy(obj);
            else
                Destroy(obj);
        }
        objectsToDelete.Clear();
    }


    //Tell all clients who turn it is not
    [PunRPC]
    void RpcDisableTurn(string playerID)
    {
        GameObject.Find(playerID).GetComponent<PlayerID>().isPlayersTurn = false;
        GameObject.Find(GameObject.Find(playerID).GetComponent<PlayerID>().playersPanel).GetComponent<Image>().color = greyedPanel;
        GameObject.Find(playerID).GetComponent<UIManager>().DisplayPopupText("Waiting for opponent to make a move", false);
    }

    //Tell all clients who turn it is
    [PunRPC]
    void RpcSetFirstTurn(string playerID)
    {
        GameObject.Find(playerID).GetComponent<PlayerID>().isPlayersTurn = true;
        GameObject.Find(GameObject.Find(playerID).GetComponent<PlayerID>().playersPanel)
            .GetComponent<Image>().color = GameObject.Find(playerID).GetComponent<PlayerColor>().playerColor;
        GameObject.Find(playerID).GetComponent<UIManager>().DisplayPopupText("It's your turn, place a line!", true);
        GameObject.Find(playerID).GetComponent<UIManager>().DisplayPopupBox("It's your turn first, place a line!");
    }
    [PunRPC]
    //Tell the server the timer has started
    void RpcEnableTimer()
    {
        PhotonNetwork.RaiseEvent(0, null, true, null);
        gameObject.GetComponent<PlayerTurn>().enabled = true;
        gameObject.GetComponent<TurnTimer>().enabled = true;
        buildGrid = false;
    }
    [PunRPC]
    //Tell the server the grid is of dots is being built
    void BuildGridText()
    {
        PhotonNetwork.RaiseEvent(1, null, true, null);
        buildGrid = true;
    }
    [PunRPC]
    //Tell the server the grid is done
    void GridCompleted()
    {
        PhotonNetwork.RaiseEvent(10, null, true, null);
        buildGrid = false;
    }
}
