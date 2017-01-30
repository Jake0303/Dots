using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
/**
 * InGameBackgroundManager
 * Control the squares and sync them the music
 */
public class InGameBackgroundManager : MonoBehaviour
{
    private AudioSource bgMusic;
    private float[] spectrum = new float[512];
    private float[] freqBand = new float[8];
    private float[] bandBuffer = new float[8];
    private float[] bufferDecrease = new float[8];
    public bool left, top;
    private float bassPower = 0.3f;
    private float snarePower = 0.6f;
    private Vector3 initialSquareScale;


    /**
    * Start
    */
    void Start()
    {
        bgMusic = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        initialSquareScale = transform.localScale;
    }

    /**
     * On update get the music frequencies and update the squares size and colour
     */
    void Update()
    {
        //Allow WebGL to GetSpectrumData
        GetSpectrumData();
        MakeFrequencyBands();
        BandBuffer();
        SyncSquaresWithMusic();
    }



    /**
     * Get the spectrum data of the current music
     */
    void GetSpectrumData()
    {
        bgMusic.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
    }


    /**
    * Utilize the current frequencies and make the squares smoothly rise or decrease when the spectrum has changed   
*/
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


    /**
    * Get the 8 different frequency bands
    * 1 - 20-60Hz
    * 2 - 60-250Hz
    * 3 - 250-500Hz
    * 4 - 500Hz-2kHz
    * 5 - 2-4kHz
    * 6 - 4-6kHz
    * 7 - 6-20kHz
*/
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
    /**
     * SyncSquaresWithMusic
     */
    void SyncSquaresWithMusic()
    {
        if (left && !top)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[0] * 1f) + 1f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[0] * 1f) + 1f);
            transform.localScale = new Vector3((bandBuffer[0] * bassPower) + initialSquareScale.x, 1f, (bandBuffer[0] * bassPower) + initialSquareScale.z);
        }
        else if (top && left)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[0] * 1f) + 1f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[0] * 1f) + 1f);
            transform.localScale = new Vector3((bandBuffer[0] * bassPower) + initialSquareScale.x, 1f, (bandBuffer[0] * bassPower) + initialSquareScale.z);
        }
        else if (!left && !top)
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[4] * 2f) + 1f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[4] * 2f) + 1f);
            transform.localScale = new Vector3((bandBuffer[4] * snarePower) + initialSquareScale.x, 1f, (bandBuffer[4] * snarePower) + initialSquareScale.z);
        }
        else
        {
            GetComponentInChildren<Renderer>().material.SetFloat("_MKGlowPower", (bandBuffer[4] * 2f) + 1f);
            GetComponentInChildren<Renderer>().material.SetFloat("_RimPower", (bandBuffer[4] * 2f) + 1f);
            transform.localScale = new Vector3((bandBuffer[4] * snarePower) + initialSquareScale.x, 1f, (bandBuffer[4] * snarePower) + initialSquareScale.z);
        }
    }
}
