using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBackgroundManager : MonoBehaviour
{
    /**
     * Change in game background color
     */
    //Good looking colors for menu background, purple and orange are the calculated colors
    private Color[] sexyColors = { Color.magenta, Color.cyan, Color.green, Color.yellow, new Color(0.4f, 0.2f, 0.6f), Color.red, new Color(1f, 0.55f, 0f) };
    private Color randomColor;
    private AudioSource bgMusic;
    private float[] spectrum = new float[512];
    private float[] freqBand = new float[8];
    private float[] bandBuffer = new float[8];
    private float[] bufferDecrease = new float[8];
    public bool left, top;



    void Start()
    {
        bgMusic = GameObject.Find("AudioManager").GetComponent<AudioSource>();
    }

    void Update()
    {
        GetSpectrumData();
        MakeFrequencyBands();
        BandBuffer();
        if (left && !top)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[0] * 1f) + 0.5f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[0] * 1f) + 1f);
        }
        else if (top && left)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[2] * 1f) + 0.5f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[2] * 1f) + 1f);
        }
        else if (!left && !top)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[4] * 1f) + 0.5f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[4] * 1f) + 1f);
        }
        else
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[6] * 1f) + 0.5f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[6] * 1f) + 1f);
        }

    }

    void GetSpectrumData()
    {
        bgMusic.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
    }
    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (freqBand[g] > bandBuffer[g])
            {
                bandBuffer[g] = freqBand[g];
                bufferDecrease[g] = 0.005f;
                /*
                if (randomColor == sexyColors[Mathf.CeilToInt(Random.Range(0, 7))])
                    randomColor = sexyColors[Mathf.CeilToInt(Random.Range(0, 7))];
                else
                    randomColor = sexyColors[Mathf.CeilToInt(Random.Range(0, 7))];
                GetComponentInChildren<Renderer>().material.SetColor("_MKGlowTexColor", randomColor);
                GetComponentInChildren<Renderer>().material.SetColor("_MKGlowColor", randomColor);
                GetComponentInChildren<Renderer>().material.SetColor("_RimColor", randomColor);*/
            }

            if (freqBand[g] < bandBuffer[g])
            {
                bandBuffer[g] -= bufferDecrease[g];
                bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7) sampleCount += 2;

            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrum[count] * (count + 1);
                count++;
            }
            average /= count;
            freqBand[i] = average * 10;
        }
    }
}
