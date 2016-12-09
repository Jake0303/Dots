using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackgroundManager : MonoBehaviour
{
    //Good looking colors for menu background
    private Color[] sexyColors = {Color.magenta, Color.cyan, Color.green, Color.yellow, new Color(0.4f, 0.2f, 0.6f), Color.red, new Color(0.2F, 0.3F, 0.4F)};
    private Color randomColor;
    public GameObject square;
    Color fade;
    float alpha = 0;

    public IEnumerator ShowSquare()
    {
        while (true)
        {
            GameObject newSquare = Instantiate(square, new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(40, 41)), square.transform.rotation) as GameObject;
            StartCoroutine(fadeIn(newSquare));
            newSquare.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            newSquare.layer = 5;//UI layer
            newSquare.GetComponentInChildren<Renderer>().enabled = true;
            if(randomColor == sexyColors[Mathf.CeilToInt(Random.Range(0, 7))])
                randomColor = sexyColors[Mathf.CeilToInt(Random.Range(0, 7))];
            else
                randomColor = sexyColors[Mathf.CeilToInt(Random.Range(0, 7))];
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", randomColor);
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_detailcolor", randomColor);
            yield return new WaitForSeconds(Random.Range(0.9f, 1.1f));
        }
    }
    void Start()
    {
        DontDestroyOnLoad(this.transform);
        StartCoroutine(ShowSquare());
        SceneManager.sceneLoaded += SceneLoaded;
    }

    IEnumerator fadeIn(GameObject newSquare)
    {
        float power = 0;
        while (power <= 1)
        {
            //Transparency.
            if (newSquare != null)
            {
                alpha = newSquare.GetComponentInChildren<Renderer>().material.GetFloat("_detailpower");
                alpha += 2f;
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_detailpower", alpha);
                power += 0.1f;
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_power", power);

                newSquare.GetComponent<Light>().intensity+= 3;
                newSquare.GetComponent<Light>().range+= 3;
                newSquare.GetComponent<Light>().enabled = true;
                newSquare.GetComponent<Light>().color = newSquare.GetComponentInChildren<Renderer>().material.GetColor("_TintColor");
                fade = newSquare.GetComponentInChildren<Renderer>().material.GetColor("_TintColor");
                fade.a += 2f;
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", fade);
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_detailcolor", fade);
            }
            yield return new WaitForSeconds(0.1f);
        }
        while (true)
        {
            //Transparency.
            if (newSquare != null)
            {
                alpha = newSquare.GetComponentInChildren<Renderer>().material.GetFloat("_detailpower");
                alpha -= 1f;
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_detailpower", alpha);
                power -= 0.04f;
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_power", power);

                fade = newSquare.GetComponentInChildren<Renderer>().material.GetColor("_TintColor");
                fade.a -= 4f;
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", fade);
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_detailcolor", fade);
                newSquare.GetComponent<Light>().intensity--;
                //newSquare.GetComponent<Light>().range--;
            }
            //kill when faded
            if (power < 0)
           {
                newSquare.GetComponent<Light>().enabled = false;
                Destroy(newSquare);
                break;
           }
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine(fadeIn(newSquare));

    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if(scene.buildIndex == 1)
        {
            StopAllCoroutines();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowSquare());
        }
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("FadedSquare"))
        {
            Destroy(square);
        }
    }
}
