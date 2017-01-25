using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    //Good looking colors for menu background, purple and orange are the calculated colors
    private Color[] sexyColors = { Color.magenta, Color.cyan, Color.green, Color.yellow, new Color(0.4f, 0.2f, 0.6f), Color.red, new Color(1f, 0.55f, 0f)};
    private Color randomColor;
    public GameObject square;
    Color fade;

    public IEnumerator ShowSquare()
    {
        while (true)
        {
            GameObject newSquare = Instantiate(square, new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(45, 45)), square.transform.rotation) as GameObject;
            newSquare.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            newSquare.layer = 5;//UI layer
            newSquare.GetComponentInChildren<Renderer>().enabled = true;
            if (randomColor == sexyColors[Mathf.CeilToInt(Random.Range(0, 7))])
                randomColor = sexyColors[Mathf.CeilToInt(Random.Range(0, 7))];
            else
                randomColor = sexyColors[Mathf.CeilToInt(Random.Range(0, 7))];
            randomColor.a = 0;
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", randomColor);
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", randomColor);
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", randomColor);
            StartCoroutine(fadeIn(newSquare));
            yield return new WaitForSeconds(Random.Range(0.7f, 0.9f));
        }
    }
    void Start()
    {
        StartCoroutine(ShowSquare());
        SceneManager.sceneLoaded += SceneLoaded;
    }

    IEnumerator fadeIn(GameObject newSquare)
    {
        float power = 0;
        while (power <= 2)
        {
            //Transparency.
            if (newSquare != null)
            {
                power += 0.1f;
                fade = newSquare.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowTexColor");
                fade.a += 0.1f;
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", fade);
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", fade);
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", fade.a*1.2f);
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", fade.a*1.2f);
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", fade);
            }
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(fadeOut(newSquare));
    }

    IEnumerator fadeOut(GameObject newSquare)
    {
        float power = 2;
        while (power >= 0)
        {
            //Transparency.
            if (newSquare != null)
            {
                power -= 0.1f;
                fade = newSquare.GetComponentInChildren<Renderer>().material.GetColor("_MKGlowTexColor");
                fade.a -= 0.1f;
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", fade);
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", fade);
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", fade.a*1.1f);
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", fade.a*1.1f);
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", fade);
            }
            yield return new WaitForSeconds(0.2f);
        }
        //kill when faded
        if (power <= 0)
        {
            Destroy(newSquare);
        }
    }

        void SceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.buildIndex == 1)
        {
            if (this != null)
            {
                StopAllCoroutines();
            }
        }
        else
        {
            if (this != null)
            {
                StopAllCoroutines();
                StartCoroutine(ShowSquare());
            }
        }
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("FadedSquare"))
        {
            Destroy(square);
        }
    }
}
