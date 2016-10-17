using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingGif : MonoBehaviour {

    public Sprite[] gifTextures = new Sprite[5];
    private float framesPerSecond = 12f;

	// Play the gif
	void Update () {
        float index = Time.time * framesPerSecond;
        index = index % gifTextures.Length;
        GetComponent<Image>().sprite = gifTextures[(int)Mathf.Floor(index)];
    }
}
