using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Text;

public class LeaderbordController : MonoBehaviour
{
    public static bool leaderBoardError = false;
    public static JSONObject data = new JSONObject();
    private static string secretKey = "89oN04ydon854CBm9XTG4Tt6YcAKEAqA"; // Edit this value and make sure it's the same as the one stored on the server
    public static string addScoreURL = "https://squarz.io/Scripts/AddToLeaderboard.php?"; //be sure to add a ? to your url
    public string highscoreURL = "https://squarz.io/Scripts/DisplayLeaderboard.php?";
    private float defaultScrollPos = 0.1f;
    private float howFarCanWeScroll; // default
    private const float scrollBarVerticalOffset = 2.5f, scrollBarVerticalLimit = 4.5f, mobileScrollBarVerticalLimit = 3f, mobileScrollBarVerticalOffset = 1.5f;



    // remember to use StartCoroutine when calling this function!
    public static IEnumerator PostScores(string name, int wins, int losses, string fbID, bool forfeit)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = sha256(name + wins.ToString() + losses.ToString() + secretKey);
        string post_url = addScoreURL + "Username=" + WWW.EscapeURL(name) + "&Wins=" + wins + "&Losses=" + losses + "&fbID=" + fbID + "&hash=" + hash;
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        if (forfeit && hs_post.isDone)
        {
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ExitGames.Client.Photon.ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(2, null, true, options);
            PhotonNetwork.Disconnect();
        }
        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }


    }


    // remember to use StartCoroutine when calling this function!
    public static IEnumerator PostScoresBeforeStart(string name, int wins, int losses, string fbID)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = sha256(name + wins.ToString() + losses.ToString() + secretKey);
        string post_url = addScoreURL + "Username=" + WWW.EscapeURL(name) + "&Wins=" + wins + "&Losses=" + losses + "&fbID=" + fbID + "&hash=" + hash;
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        if (hs_post.isDone)
        {
            //Hide and show menus
            GameObject.Find("EnterNickMenu").GetComponent<DoozyUI.UIElement>().Hide(false);
            GameObject.Find("ConnectingMenu").GetComponent<DoozyUI.UIElement>().Show(false);
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().AutoConnect = true;
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().ConnectInUpdate = true;
        }
        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }


    }

    static string sha256(string password)
    {
        System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
        System.Text.StringBuilder hash = new System.Text.StringBuilder();
        byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
        foreach (byte theByte in crypto)
        {
            hash.Append(theByte.ToString("x2"));
        }
        return hash.ToString();
    }


    // remember to use StartCoroutine when calling this function!
    public static IEnumerator PostScores(string guestID, string name, int wins, int losses, bool forfeit)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = sha256(name + wins.ToString() + losses.ToString() + secretKey);
        string post_url = addScoreURL + "GuestID=" + guestID + "&Username=" + WWW.EscapeURL(name) + "&Wins=" + wins + "&Losses=" + losses + "&hash=" + hash;
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
        if (forfeit && hs_post.isDone)
        {
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ExitGames.Client.Photon.ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(2, null, true, options);
            PhotonNetwork.Disconnect();
        }
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
        if (GameObject.Find("GettingLeaderboardDataText"))
            GameObject.Find("GettingLeaderboardDataText").GetComponent<Text>().text = "Getting leaderboard data...";
        string hash = sha256(secretKey);
        string post_url = highscoreURL + "?hash=" + hash;
        WWW hs_get = new WWW(post_url);
        yield return hs_get;

        if (hs_get.error != null)
        {
            GameObject.Find("GettingLeaderboardDataText").GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
            GameObject.Find("GettingLeaderboardDataText").GetComponent<Text>().text = "There was an error getting leaderboards, please try again later.";
            leaderBoardError = true;
            yield break;
        }
        else
        {
            if (GameObject.Find("GettingLeaderboardDataText"))
                GameObject.Find("GettingLeaderboardDataText").GetComponent<Text>().text = "";
            data = new JSONObject(hs_get.text);
            int position = 1;
            howFarCanWeScroll = defaultScrollPos;
            GameObject.Find("position").GetComponent<Text>().text = "\n";
            GameObject.Find("username").GetComponent<Text>().text = "\n";
            GameObject.Find("wins").GetComponent<Text>().text = "\n";
            GameObject.Find("losses").GetComponent<Text>().text = "\n";
            foreach (var aData in data.list)
            {
                if (GameObject.Find("wins"))
                {
                    GameObject.Find("position").GetComponent<Text>().text += position.ToString() + ".\n ";
                    GameObject.Find("username").GetComponent<Text>().text += " " + aData["Username"].str + "\n";
                    GameObject.Find("wins").GetComponent<Text>().text += " " + aData["Wins"].str + "\n";
                    GameObject.Find("losses").GetComponent<Text>().text += aData["Losses"].str + "\n";
                    position++;
                    howFarCanWeScroll += 0.3f;
                }
            }
            leaderBoardError = false;
            yield break;
        }
    }
    //Restrict the scroll to the min and max
    public void OnScrollChanged(Vector2 value)
    {
        if (GameObject.Find("LeaderboardPanel").transform.position.y >= howFarCanWeScroll)
        {
            GameObject.Find("LeaderboardPanel").transform.position = new Vector3(GameObject.Find("LeaderboardPanel").transform.position.x, howFarCanWeScroll, GameObject.Find("LeaderboardPanel").transform.position.z);
        }
        else if (GameObject.Find("LeaderboardPanel").transform.position.y < defaultScrollPos)
        {
            GameObject.Find("LeaderboardPanel").transform.position = new Vector3(GameObject.Find("LeaderboardPanel").transform.position.x, defaultScrollPos, GameObject.Find("LeaderboardPanel").transform.position.z);
        }
        if (Screen.orientation == ScreenOrientation.Landscape)
            GameObject.Find("ScrollImg").transform.position = new Vector3(GameObject.Find("ScrollImg").transform.position.x, (-GameObject.Find("LeaderboardPanel").transform.position.y / (howFarCanWeScroll / scrollBarVerticalLimit)) + scrollBarVerticalOffset, GameObject.Find("ScrollImg").transform.position.z);
        else
            GameObject.Find("ScrollImg").transform.position = new Vector3(GameObject.Find("ScrollImg").transform.position.x, (-GameObject.Find("LeaderboardPanel").transform.position.y / (howFarCanWeScroll / mobileScrollBarVerticalLimit)) + mobileScrollBarVerticalOffset, GameObject.Find("ScrollImg").transform.position.z);
    }
}
