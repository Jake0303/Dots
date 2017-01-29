using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBackgroundManager : MonoBehaviour
{
    /**
     * Change in game background color
     */
    private AudioSource bgMusic;
    private float[] spectrum = new float[512];
    private float[] freqBand = new float[8];
    private float[] bandBuffer = new float[8];
    private float[] bufferDecrease = new float[8];
    public bool left, top;
    private float bassPower = 0.3f;
    private float snarePower = 0.6f;
    private Vector3 initialSquareScale;

    void Start()
    {
        bgMusic = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        initialSquareScale = transform.localScale;
    }

    void Update()
    {
        GetSpectrumData();
        MakeFrequencyBands();
        BandBuffer();
        if (left && !top)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[0] * 1f) + 1f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[0] * 1f) + 1f);
            transform.localScale= new Vector3((bandBuffer[0] * bassPower) +initialSquareScale.x, 1f, (bandBuffer[0] * bassPower) + initialSquareScale.z);
        }
        else if (top && left)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[0] * 1f) + 1f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[0] * 1f) + 1f);
            transform.localScale = new Vector3((bandBuffer[0] * bassPower) + initialSquareScale.x, 1f, (bandBuffer[0] * bassPower) +initialSquareScale.z);
        }
        else if (!left && !top)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[4] * 2f) + 1f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[4] * 2f) + 1f);
            transform.localScale = new Vector3((bandBuffer[4] * snarePower) + initialSquareScale.x, 1f, (bandBuffer[4] * snarePower) +initialSquareScale.z);
        }
        else
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[4] * 2f) + 1f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[4] * 2f) + 1f);
            transform.localScale = new Vector3((bandBuffer[4] * snarePower) + initialSquareScale.x, 1f, (bandBuffer[4] * snarePower) + initialSquareScale.z);
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
