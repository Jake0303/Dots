using UnityEngine;
using System.Collections;

/**
 * Globals
 * Manages global variables for Squarz
 */ 
public class GLOBALS : MonoBehaviour
{
    //Global variable to determine the delay before a game has started
    public const float GAMESTARTDELAY = 1.0f;
    //Global variable to determine the number of players required to start a game
    public const int NUMOFPLAYERSTOSTARTGAME = 2;
    //Global variable to determine the height of a line before being placed
    public const int LINEHEIGHT = 37;
    //Global variable to determine the timer duration
    public const float MAXTURNTIME = 20.0f;
    //Volume of the game
    public static float Volume = 25;
    //Name of the game
    public static string GameName = "Squarz";
    //Points to win game
    //(((GRIDWIDTH - 1) * (GRIDHEIGHT - 1) / 2) + 1)
    public const int POINTSTOWIN = 1;
    //GridWidth
    public const int GRIDWIDTH = 4;
    //GridHeight
    public const int GRIDHEIGHT = 4;
    //The distance between each dot
    public const float DOTDISTANCE = 11.0f;
    //Version of the game
    public const byte Version = 1;
    //Is Colorblind Assist Enabled
    public static bool ColorBlindAssist = false;
    //DarkGreen
    public static Color DarkGreen = new Color(0, 0.7f, 0, 1);
    //DarkRed
    public static Color DarkRed = new Color(0.7f, 0, 0, 1);
    //DarkBlue
    public static Color DarkBlue= new Color(0, 0, 0.7f, 1);
    //DarkYellow
    public static Color DarkYellow = new Color(0.7f, 0.62f, 0.008f, 1);
}