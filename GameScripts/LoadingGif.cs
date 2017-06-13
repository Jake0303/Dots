using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class LoadingGif : MonoBehaviour {

    public Sprite[] gifTextures = new Sprite[5];
    private float framesPerSecond = 12f;
    private float index;

    public IEnumerator playGif()
    {
        while (this != null && GetComponent<Image>() != null)
        {
            index = Time.time * framesPerSecond;
            index = index % gifTextures.Length;
            GetComponent<Image>().sprite = gifTextures[(int)Mathf.Floor(index)];
            yield return new WaitForSeconds(0.01f);
        }
    }
}
