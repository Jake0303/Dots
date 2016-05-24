using UnityEngine;
using System.Collections;

public class GLOBALS : MonoBehaviour {
    //Global variable to distinguish if the network is the local player's host or Unity's Matchmaking Service
    public const bool ISNETWORKLOCAL = true;
    //Global variable to determine the delay before a game has started
    public const float GAMESTARTDELAY = 1f;
    //Global variable to determine the number of players required to start a game
    public const int NUMOFPLAYERSTOSTARTGAME = 1;
    //Global variable to determine the height of a line before being place
    public const int LINEHEIGHT = 0;
    //Global variable to determine the timer duration
    public const float MAXTURNTIME = 30.0f;
    //Volume of the game
    public static float Volume = 0;
    //Name of the game
    public static string GameName = "Dots";
    //Points to win game
    //(((GameStart.gridWidth - 1) * (GameStart.gridHeight - 1) / 2)+1)
    public const  int POINTSTOWIN = 2;
    //GridWidth
    public const int GRIDWIDTH = 5;
    //GridHeight
    public const int GRIDHEIGHT = 5;
    //The distance between each dot
    public const float DOTDISTANCE = 11.0f;
}
