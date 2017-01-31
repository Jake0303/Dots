using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    //Good looking colors for menu background, purple and orange are the calculated colors
    private Color[] sexyColors = { Color.magenta, Color.cyan, Color.green, Color.yellow, new Color(0.4f, 0.2f, 0.6f), Color.red, new Color(1f, 0.55f, 0f) };
    private Color randomColor;
    public GameObject square;
    Color fade;
    int colorIndex = 0;
    private float glowRate = 1.2f;
    private float decreaseGlowRate = 1.1f;

    public IEnumerator ShowSquare()
    {
        while (true)
        {
            GameObject newSquare = null;
            if (SceneManager.GetActiveScene().buildIndex != 1)
            {
                newSquare = Instantiate(square, new Vector3(Random.Range(-30, 30), Random.Range(-20, 20), Random.Range(44, 44)), square.transform.rotation) as GameObject;
                newSquare.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            }
            else
            {
                newSquare = Instantiate(square, new Vector3(Random.Range(-25, 50), Random.Range(-11, -9), Random.Range(-25, 50)), square.transform.rotation) as GameObject;
            }
            newSquare.layer = 5;//UI layer
            newSquare.GetComponentInChildren<Renderer>().enabled = true;
            if (colorIndex <= 6)
                randomColor = sexyColors[colorIndex];
            else
                randomColor = sexyColors[0];
            randomColor.a = 0;
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", randomColor);
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", randomColor);
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_RimColor", randomColor);
            StartCoroutine(fadeIn(newSquare));
            if (colorIndex <= 6)
                colorIndex++;
            else
                colorIndex = 0;
            if (SceneManager.GetActiveScene().buildIndex != 1)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            }
        }
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            glowRate = 2f;
            decreaseGlowRate = 1f;
        }
        else
        {
            glowRate = 1.2f;
            decreaseGlowRate = 1.1f;
        }
        colorIndex = 0;
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
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", fade.a * glowRate);
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", fade.a * glowRate);
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
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", fade.a * decreaseGlowRate);
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", fade.a * decreaseGlowRate);
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
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    StopAllCoroutines();
                }
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
