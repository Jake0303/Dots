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
    public const int LINEHEIGHT = 50;
}
