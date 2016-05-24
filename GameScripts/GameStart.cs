using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameStart : NetworkBehaviour
{

    [SerializeField]
    public GameObject dots, lineHor, lineVert,centerSquare;
    public GameObject hoverLineHor, hoverLineVert;
    public SyncListString playerNames = new SyncListString();
    //The speed at which each dot in the grid spawns
    [SyncVar]
    public float spawnSpeed = 0.1f;
    [SyncVar]
    public bool buildGrid = false;
    [SyncVar(hook = "OnStartChanged")]
    public bool startGame = false;
    //Sync the rotation and scale,Network.Spawn only sync position
    [SyncVar(hook = "OnVertRotChanged")]
    public Quaternion lineVertRot;
    [SyncVar(hook = "OnHorRotChanged")]
    public Quaternion lineHorRot;
    [SyncVar(hook = "OnVertScaleChanged")]
    public Vector3 lineVertScale;
    [SyncVar(hook = "OnHorScaleChanged")]
    public Vector3 lineHorScale;
    [SyncVar(hook = "OnSquareScaleChanged")]
    public Vector3 squareScale;
    public List<GameObject> objectsToDelete = new List<GameObject>();
    public override void OnStartServer()
    {
        base.OnStartServer();
        buildGrid = true;
    }
    void OnStartChanged(bool change)
    {
        startGame = change;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
    }
    void OnSquareScaleChanged(Vector3 scale)
    {
        squareScale = scale;
        centerSquare.transform.localScale = squareScale;
    }
    void OnVertScaleChanged(Vector3 scale)
    {
        lineVertScale = scale;
        lineVert.transform.localScale = lineVertScale;
    }
    void OnHorScaleChanged(Vector3 scale)
    {
        lineHorScale = scale;
        lineHor.transform.localScale = lineHorScale;
    }

    void OnVertRotChanged(Quaternion rot)
    {
        lineVertRot = rot;
        lineVert.transform.localRotation = lineVertRot;
    }
    void OnHorRotChanged(Quaternion rot)
    {
        lineHorRot = rot;
        lineHor.transform.localRotation = lineHorRot;
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(GLOBALS.GAMESTARTDELAY);
        StartCoroutine(CreateGrid());
    }
    void Update()
    {
        if (startGame)
        {
            GetComponent<GameState>().gameState = GameState.State.BuildingGrid;
            StartCoroutine(StartGame());
            //Build the grid of dots
            //Hide temporary lines
            lineHor.GetComponent<Renderer>().enabled = false;
            lineVert.GetComponent<Renderer>().enabled = false;
            //centerSquare.GetComponent<Renderer>().enabled = false;
            startGame = false;

        }
    }

    void AssignTurnsAndColors()
    {
        if (isServer)
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
                    CmdSetFirstTurn(player.GetComponent<NetworkIdentity>());
                }
                else
                {
                    player.GetComponent<PlayerID>().isPlayersTurn = false;
                    CmdDisableTurn(player.GetComponent<NetworkIdentity>());
                }
                //Only 4 people per game
                if (i != GameObject.Find("GameManager").GetComponent<GameStart>().playerNames.Count)
                    i++;
            }
        }
    }

    //Tell the server that we spawned a line or dot
    void SpawnObj(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }
    //Build the grid
    IEnumerator CreateGrid()
    {
        if (isServer)
        {
            for (int x = 0; x < GLOBALS.GRIDWIDTH; x++)
            {
                yield return new WaitForSeconds(spawnSpeed);

                for (int z = 0; z < GLOBALS.GRIDHEIGHT; z++)
                {
                    yield return new WaitForSeconds(spawnSpeed);
                    //Spawn dot

                    GameObject dot = Instantiate(dots, Vector3.zero, Quaternion.Euler(90,0,0)) as GameObject;
                    dot.transform.localPosition = new Vector3(x * GLOBALS.DOTDISTANCE, 0, z * GLOBALS.DOTDISTANCE);
                    dot.transform.localScale = new Vector3(3, 3, 3);
                    dot.name = "Dot " + x.ToString() + "," + z.ToString();
                    dot.GetComponent<DotID>().dotID = dot.name;
                    objectsToDelete.Add(dot);
                    SpawnObj(dot);
                   
                    //This if statement stops from building extra unnecessary lines
                    if (z < GLOBALS.GRIDHEIGHT - 1)
                    {
                        //Spawn line in between dots horizontally
                        GameObject lineHorizontal = Instantiate(lineHor, Vector3.zero, lineHor.transform.rotation) as GameObject;
                        lineHorizontal.transform.localPosition = new Vector3(x * GLOBALS.DOTDISTANCE, 0, dot.transform.localPosition.z + (GLOBALS.DOTDISTANCE / 2.0f));
                        lineHorScale = new Vector3(3, 3, GLOBALS.DOTDISTANCE - dot.transform.localScale.z + 0.5f);
                        lineHorRot = Quaternion.Euler(0, 0, 0);
                        lineHorizontal.name = "linesHorizontal " + x.ToString() + "," + z.ToString();
                        lineHorizontal.GetComponent<LineID>().lineID = lineHorizontal.name;
                        lineHorizontal.transform.localScale = lineHorScale;
                        lineHorizontal.transform.rotation = lineHorRot;
                        objectsToDelete.Add(lineHorizontal);
                        SpawnObj(lineHorizontal);
                    }
                    if (x < GLOBALS.GRIDWIDTH - 1)
                    {
                        //Spawn line in between dots vertically
                        GameObject lineVertical = Instantiate(lineVert, Vector3.zero, lineVert.transform.rotation) as GameObject;
                        lineVertical.transform.localPosition = new Vector3(dot.transform.localPosition.x + (GLOBALS.DOTDISTANCE / 2.0f), 0, z * GLOBALS.DOTDISTANCE);
                        lineVertScale = new Vector3(GLOBALS.DOTDISTANCE - dot.transform.localScale.z + 0.5f, 3, 3);
                        lineVertRot = Quaternion.Euler(0, 0, 0);
                        lineVertical.name = "linesVertical " + x.ToString() + "," + z.ToString();
                        lineVertical.GetComponent<LineID>().lineID = lineVertical.name;
                        lineVertical.transform.localScale = lineVertScale;
                        lineVertical.transform.rotation = lineVertRot;
                        objectsToDelete.Add(lineVertical);
                        SpawnObj(lineVertical);
                    }
                    //Spawn the center of a square
                    if (x < GLOBALS.GRIDWIDTH - 1 && z < GLOBALS.GRIDHEIGHT - 1)
                    {
                        GameObject centerSq = Instantiate(centerSquare, Vector3.zero, centerSquare.transform.rotation) as GameObject;
                        centerSq.transform.localPosition = new Vector3(dot.transform.localPosition.x + (GLOBALS.DOTDISTANCE / 2.0f), 0, dot.transform.localPosition.z + (GLOBALS.DOTDISTANCE / 2.0f));
                        squareScale = new Vector3(8.5f, 2f, 8.5f);
                        centerSq.transform.localScale = squareScale;
                        centerSq.name = "Centre " + x.ToString() + "," + z.ToString();
                        centerSq.GetComponent<SquareID>().squareID = centerSq.name;
                        objectsToDelete.Add(centerSq);
                        SpawnObj(centerSq);
                        centerSq.GetComponent<Renderer>().enabled = false;
                    }
                }
            }
            //Start the timer after the grid has been built
            gameObject.GetComponent<TurnTimer>().enabled = true;
            RpcEnableTimer();
            AssignTurnsAndColors();
            GetComponent<GameState>().gameState = GameState.State.InProgress;
            buildGrid = false;
        }
    }

    //Destroy the grid
    public void DestroyGrid()
    {
        foreach (var obj in objectsToDelete)
        {
            GameObject.Destroy(obj);
        }
        objectsToDelete.Clear();
    }

    //Tell the server that this player turn is disabled
    [Command]
    void CmdDisableTurn(NetworkIdentity playerID)
    {
        playerID.GetComponent<PlayerID>().isPlayersTurn = false;
        RpcDisableTurn(playerID);
    }
    //Tell all clients who turn it is not
    [ClientRpc]
    void RpcDisableTurn(NetworkIdentity playerID)
    {
        playerID.GetComponent<PlayerID>().isPlayersTurn = false;
    }
    //Tell the server that the first player turn is up
    [Command]
    void CmdSetFirstTurn(NetworkIdentity playerID)
    {
        playerID.GetComponent<PlayerID>().isPlayersTurn = true;
        RpcSetFirstTurn(playerID);
    }
    //Tell all clients who turn it is
    [ClientRpc]
    void RpcSetFirstTurn(NetworkIdentity playerID)
    {
        playerID.GetComponent<PlayerID>().isPlayersTurn = true;
    }
    [ClientRpc]
    //Tell the server the timer has started
    void RpcEnableTimer()
    {
        gameObject.GetComponent<PlayerTurn>().enabled = true;
        gameObject.GetComponent<TurnTimer>().enabled = true;
    }
}
