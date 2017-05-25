﻿using UnityEditor;
using UnityEngine;
using UnityEngine.Advertisements;


public class ADManager : MonoBehaviour {

    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
    }
}
