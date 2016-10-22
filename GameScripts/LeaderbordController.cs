using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LeaderbordController : MonoBehaviour {
    public static bool leaderBoardError = false;
    private string secretKey = "mySecretKey"; // Edit this value and make sure it's the same as the one stored on the server
    public static string addScoreURL = "https://squarz.io/Scripts/AddToLeaderboard.php?"; //be sure to add a ? to your url
    public string highscoreURL = "https://squarz.io/Scripts/DisplayLeaderboard.php";

    void Start()
    {
        StartCoroutine(GetScores());
    }

    // remember to use StartCoroutine when calling this function!
    public static IEnumerator PostScores(string name, int wins, int losses)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        //string hash = MD5Test.Md5Sum(name + score + secretKey);

        string post_url = addScoreURL + "Username=" + WWW.EscapeURL(name) + "&Wins=" + wins + "&Losses=" + losses;

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }
    }

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    public IEnumerator GetScores()
    {
        gameObject.GetComponent<Text>().fontSize = 40;
        gameObject.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        gameObject.GetComponent<Text>().text = "Loading Scores...";
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;

        if (hs_get.error != null)
        {
            gameObject.GetComponent<Text>().fontSize = 35;
            gameObject.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
            gameObject.GetComponent<Text>().text = "There was an error getting leadboards, please try again later";
            leaderBoardError = true;
        }
        else
        {
            string data = hs_get.text;
            string[] scores = data.Split(","[0]);
            gameObject.GetComponent<Text>().text = scores[0]; // this is a GUIText that will display the scores in game.
            leaderBoardError = false;
        }
    }

}
