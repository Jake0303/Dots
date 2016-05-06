using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;

public class GameStart : NetworkBehaviour
{

    [SerializeField]
    public GameObject dots, lineHor, lineVert;
    public GameObject hoverLineHor, hoverLineVert;
    public SyncListString playerNames = new SyncListString();
    //GridWidth
    public static int gridWidth = 6;
    //GridHeight
    public static int gridHeight = 6;
    //The distance between each dot
    public static float dotDistance = 11.0f;
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

    public override void OnStartServer()
    {
        base.OnStartServer();
        buildGrid = true;
    }
    void OnStartChanged(bool change)
    {
        startGame = change;
    }
    void OnListChanged(SyncListString.Operation operation, int index)
    {    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        playerNames.Callback = OnListChanged;
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
            StartCoroutine(StartGame());
            //Build the grid of dots
            //Hide temporary lines
            lineHor.GetComponent<Renderer>().enabled = false;
            lineVert.GetComponent<Renderer>().enabled = false;
            startGame = false;
        }
    }

    void AssignTurnsAndColors()
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

    //Tell the server that we spawned a line or dot
    [Command]
    void CmdSpawnObj(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }
    //Build the grid
    IEnumerator CreateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            yield return new WaitForSeconds(spawnSpeed);

            for (int z = 0; z < gridHeight; z++)
            {
                yield return new WaitForSeconds(spawnSpeed);
                //Spawn dot
                dots = Instantiate(dots, Vector3.zero, dots.transform.rotation) as GameObject;
                dots.transform.localPosition = new Vector3(x * dotDistance, 0, z * dotDistance);
                dots.transform.localScale = new Vector3(3, 3, 3);
                dots.name = "Dot " + x.ToString() + "," + z.ToString();
                dots.GetComponent<DotID>().dotID = dots.name;
                CmdSpawnObj(dots);
                //This if statement stops from building extra unnecessary lines
                if (z < gridHeight - 1)
                {
                    //Spawn line in between dots horizontally
                    lineHor = Instantiate(lineHor, Vector3.zero, lineHor.transform.rotation) as GameObject;
                    lineHor.transform.localPosition = new Vector3(x * dotDistance, 0, dots.transform.localPosition.z + (dotDistance / 2.0f));
                    lineHorScale = new Vector3(3, 3, dotDistance - dots.transform.localScale.z + 0.5f);
                    lineHorRot = Quaternion.Euler(0, 0, 0);
                    lineHor.name = "linesHorizontal " + x.ToString() + "," + z.ToString();
                    lineHor.GetComponent<LineID>().lineID = lineHor.name;
                    lineHor.transform.localScale = lineHorScale;
                    lineHor.transform.rotation = lineHorRot;
                    CmdSpawnObj(lineHor);
                }
                if (x < gridWidth - 1)
                {
                    //Spawn line in between dots vertically
                    lineVert = Instantiate(lineVert, Vector3.zero, lineVert.transform.rotation) as GameObject;
                    lineVert.transform.localPosition = new Vector3(dots.transform.localPosition.x + (dotDistance / 2.0f), 0, z * dotDistance);
                    lineVertScale = new Vector3(dotDistance - dots.transform.localScale.z + 0.5f, 3, 3);
                    lineVertRot = Quaternion.Euler(0,0,0);
                    lineVert.name = "linesVertical " + x.ToString() + "," + z.ToString();
                    lineVert.GetComponent<LineID>().lineID = lineVert.name;
                    lineVert.transform.localScale = lineVertScale;
                    lineVert.transform.rotation = lineVertRot;
                    CmdSpawnObj(lineVert);
                }
            }
        }
        //Start the timer after the grid has been built
        //gameObject.GetComponent<PlayerTurn>().enabled = true;
        gameObject.GetComponent<TurnTimer>().enabled = true;
        CmdEnableTimer();
        AssignTurnsAndColors();
        buildGrid = false;
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
    //Tell the server the timer has started
    [Command]
    void CmdEnableTimer()
    {
        gameObject.GetComponent<PlayerTurn>().enabled = true;
        gameObject.GetComponent<TurnTimer>().enabled = true;
        RpcEnableTimer();
    }
    //Replicate to all clients
    [ClientRpc]
    void RpcEnableTimer()
    {
        gameObject.GetComponent<PlayerTurn>().enabled = true;
        gameObject.GetComponent<TurnTimer>().enabled = true;
    }
}
