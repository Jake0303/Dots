using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;
using System;

public class FacebookManager : MonoBehaviour
{
    #region Notes
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
        // Initialized the singleton if no other component has accessed the singleton yet
        
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
        FBUpdateLoginStatus(FB.IsLoggedIn);
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
            FBGetPerms();
        }
        Debug.Log(FB.AppId);
    }

    private void FBInitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            FBGetPerms();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
    public void FBButtonClick()
    {
        if (_instance == null)
        {
            instance.Init();
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
        // return focus to your game after facebook login completes
        // include this function on your unity index.html page
        /* <script type='text/javascript'>
                function recuperaFocus() {
                    this.focus();
                }
          </ script >
        */
        Application.ExternalCall("recuperaFocus");
        if (FB.IsLoggedIn)
        {
            AccessToken accessToken = AccessToken.CurrentAccessToken;
            Debug.Log("Get Short Lived Access token: " + accessToken.UserId + ", " + accessToken.ExpirationTime + ", " + accessToken.Permissions);
        }
        else
        {
            Debug.Log("User cancelled login");
        }

        FBUpdateLoginStatus(FB.IsLoggedIn);
    }

    void FBUpdateLoginStatus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerLocal>().JoinGame();
            GameObject.Find("LoginMenu").GetComponent<DoozyUI.UIElement>().Hide(false);
            GameObject.Find("ConnectingMenu").GetComponent<DoozyUI.UIElement>().Show(false);
        }
    }
    #endregion
}
