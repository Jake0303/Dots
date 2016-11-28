using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LeaderbordController : MonoBehaviour
{
    public static bool leaderBoardError = false;
    public static JSONObject data = new JSONObject();
    private static string secretKey = "89oN04ydon854CBm9XTG4Tt6YcAKEAqA"; // Edit this value and make sure it's the same as the one stored on the server
    public static string addScoreURL = "https://squarz.io/Scripts/AddToLeaderboard.php?"; //be sure to add a ? to your url
    public string highscoreURL = "https://squarz.io/Scripts/DisplayLeaderboard.php";
    private float defaultScrollPos = 3.2f;
    private float howFarCanWeScroll; // default


    void Start()
    {
        StartCoroutine(GetScores());
    }

    public static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    // remember to use StartCoroutine when calling this function!
    public static IEnumerator PostScores(string name, int wins, int losses, string fbID)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = Md5Sum(name + wins.ToString() + losses.ToString() + secretKey);
        string post_url = addScoreURL + "Username=" + WWW.EscapeURL(name) + "&Wins=" + wins + "&Losses=" + losses + "&fbID="+ fbID + "&hash=" + hash;
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }


    }


    // remember to use StartCoroutine when calling this function!
    public static IEnumerator PostScores(string name, int wins, int losses)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = Md5Sum(name + wins.ToString() + losses.ToString() + secretKey);
        string post_url = addScoreURL + "Username=" + WWW.EscapeURL(name) + "&Wins=" + wins + "&Losses=" + losses  + "&hash=" + hash;
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
        if (gameObject.GetComponent<Text>())
        {
            gameObject.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        }
        //gameObject.GetComponent<Text>().text = "Loading Scores...";
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;

        if (hs_get.error != null)
        {
            if (gameObject.GetComponent<Text>())
            {
                gameObject.GetComponent<Text>().fontSize = 35;
                gameObject.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
                gameObject.GetComponent<Text>().text = "There was an error getting leadboards, please try again later";
            }
            leaderBoardError = true;
        }
        else
        {
            if (hs_get.isDone)
            {
                data = new JSONObject(hs_get.text);
                int position = 1;
                howFarCanWeScroll = defaultScrollPos;
                foreach (var aData in data.list)
                {
                    if (GameObject.Find("wins"))
                    {
                        GameObject.Find("position").GetComponent<Text>().text += position.ToString() + ".  \n";
                        GetComponent<Text>().text += aData["Username"].str + "\n";
                        GameObject.Find("wins").GetComponent<Text>().text += aData["Wins"].str + "W \n";
                        GameObject.Find("losses").GetComponent<Text>().text += aData["Losses"].str + "L \n";
                        position++;
                        howFarCanWeScroll += 0.3f;
                    }
                }
                leaderBoardError = false;
            }
        }
    }
    //Restrict the scroll to the min and max
    public void OnScrollChanged(Vector2 value)
    {
        if (GameObject.Find("Content").transform.position.y >= howFarCanWeScroll)
        {
            //The -4.5 Centers it in the middle
            GameObject.Find("Content").transform.position = new Vector3(-4.5f, howFarCanWeScroll, 1);
        }
        else if (GameObject.Find("Content").transform.position.y <= defaultScrollPos)
        {
            GameObject.Find("Content").transform.position = new Vector3(-4.5f, defaultScrollPos, 1);
        }
    }
}
