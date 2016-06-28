using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using Photon;


public class GameState : PunBehaviour
{
    public Coroutine routine;
    GameObject[] players;

    //State of the game
    public enum State
    {
        None,
        Waiting,
        BuildingGrid,
        InProgress,
        GameOver
    };
    public State gameState;
    void Start()
    {
        gameState = State.None;
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            
            // We own this player: send the others our data
            stream.SendNext(gameState);
            /*
            switch (gameState)
            {
                case State.Waiting:
                    DisplayServerMessage("Waiting for players", false);
                    foreach (var player in players)
                    {
                        player.GetComponent<UIManager>().DisplayPopupText("Waiting for players", false);
                    }
                    break;
                case State.InProgress:
                    foreach (var player in players)
                    {
                        if (player.GetComponent<UIManager>().routine != null)
                            //player.GetComponent<UIManager>().StopCoroutine(player.GetComponent<UIManager>().routine);
                            if (!player.GetComponent<PlayerID>().isPlayersTurn)
                            {
                                player.GetComponent<UIManager>().DisplayPopupText("Waiting for opponent to make a move", false);
                            }
                    }
                    break;
                case State.BuildingGrid:
                    DisplayServerMessage("Building grid", false);
                    foreach (var player in players)
                    {
                        player.GetComponent<UIManager>().DisplayPopupText("Building grid", false);
                    }
                    break;
                case State.GameOver:
                    foreach (var player in players)
                    {
                        if (player.GetComponent<UIManager>().routine != null)
                        {
                            //player.GetComponent<UIManager>().StopCoroutine(player.GetComponent<UIManager>().routine);

                        }
                    }
                    break;
            }*/
        }
        else
        {
            // Network player, receive data
            this.gameState = (State)stream.ReceiveNext();
        }
    }
    public override void OnConnectedToMaster()
    {
        switch (gameState)
        {
            case State.Waiting:
                DisplayServerMessage("Waiting for players", false);
                foreach (var player in players)
                {
                    player.GetComponent<UIManager>().DisplayPopupText("Waiting for players", false);
                }
                break;
            case State.BuildingGrid:
                DisplayServerMessage("Building grid", false);
                foreach (var player in players)
                {
                    player.GetComponent<UIManager>().DisplayPopupText("Building grid", false);
                }
                break;
        }
    }

    //Fade in text animation
    public IEnumerator FadeTextToFullAlpha(float t, Text i, bool fadeOut)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            if (i != null)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            }
            yield return null;
        }
        if (fadeOut)
        {
            StartCoroutine(FadeOutText(1f, i));
        }
        else
        {
            //StopCoroutine(routine);
        }
    }
    //Fade out text animation
    IEnumerator FadeOutText(float t, Text i)
    {
        yield return new WaitForSeconds(1f);
        i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a);
        while (i.color.a > 0f)
        {
            if (i != null)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            }
            yield return null;
        }
        //StopCoroutine(routine);
    }
    //Display popup text for the player
    public void DisplayServerMessage(string text, bool fadeOutMessage)
    {
        if (photonView.isMine)
        {
            GameObject.Find("PopupText").GetComponent<Text>().text = text;
            if (text != "")
            {
                routine = StartCoroutine(FadeTextToFullAlpha(1f, GameObject.Find("PopupText").GetComponent<Text>(), fadeOutMessage));
            }
        }
    }
}
