using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour
{
    public GameObject square;
    Color fade;
    float alpha = 0;
    IEnumerator ShowSquare()
    {
        while (true)
        {
            GameObject newSquare = Instantiate(square, new Vector3(Random.Range(265, 335), Random.Range(265, 335), Random.Range(20, 70)), square.transform.rotation) as GameObject;
            StartCoroutine(fadeIn(newSquare));
            newSquare.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            newSquare.layer = 5;//UI layer
            newSquare.GetComponentInChildren<Renderer>().enabled = true;// get the object's network ID
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0));
            newSquare.GetComponentInChildren<Renderer>().material.SetColor("_detailcolor", new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0));
            newSquare.GetComponent<Light>().intensity = 0;
            yield return new WaitForSeconds(Random.Range(1f, 1.2f));
        }
    }
    void Start()
    {
        StartCoroutine(ShowSquare());
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
                power += 0.05f;
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_power", power);

                newSquare.GetComponent<Light>().intensity++;
                newSquare.GetComponent<Light>().enabled = true;

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
                alpha -= 2f;
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_detailpower", alpha);
                power -= 0.02f;
                newSquare.GetComponentInChildren<Renderer>().material.SetFloat("_power", power);

                fade = newSquare.GetComponentInChildren<Renderer>().material.GetColor("_TintColor");
                fade.a -= 2f;
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", fade);
                newSquare.GetComponentInChildren<Renderer>().material.SetColor("_detailcolor", fade);
                newSquare.GetComponent<Light>().intensity--;

            }
            //kill when faded
            if (power <= 0)
            {
                newSquare.GetComponent<Light>().enabled = false;
                Destroy(newSquare);
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine(fadeIn(newSquare));

    }

}
