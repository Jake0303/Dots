﻿using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;

public class FacebookManager : MonoBehaviour
{
    public string accessToken = "";
    // Singleton is nice here 
    // you can then simply use FacebookManager.instance.WhatEverMethodYouWant()
    private static FacebookManager _instance;
    public static FacebookManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(FacebookManager)) as FacebookManager;
                _instance.accessToken = "";

                if (!_instance)
                {
                    Debug.LogError("There needs to be one active FacebookManager script on a GameObject in your scene.");
                }
            }

            return _instance;
        }
    }


    void Start()
    {
        accessToken = "";
        // Initialized the singleton if no other component has accessed the singleton yet
        if (_instance == null)
        {
            instance.Init();
        }
    }

    void Init()
    {
        FB.Init(FBInit, FBOnHideUnity);
    }


    void FBInit()
    {
        FBLogin();
    }

    public void FBLogin()
    {
        if (FB.IsLoggedIn)
        {
            return;
        }

        if (!FB.IsInitialized)
        {
            FB.Init(FBInitCallback, FBOnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }

    }

    private void FBInitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
    public void FBButtonClick()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            FBGetPerms();
        }
    }

    void FBGetPerms()
    {
        List<string> perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, FBAuthCallback);
    }

    private void FBOnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void FBAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            accessToken = AccessToken.CurrentAccessToken.UserId;
            GameObject.Find("LoginMenu").GetComponent<DoozyUI.UIElement>().Hide(false);
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().JoinGame(true);
        }
    }
}
