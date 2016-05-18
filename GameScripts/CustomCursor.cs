using UnityEngine;
using System.Collections;

public class CustomCursor : MonoBehaviour
{
    //The texture that's going to replace the current cursor 
    public Texture2D cursorTexture;

    //This variable flags whether the custom cursor is active or not 
    public bool ccEnabled = true;

    void Start()
    {
        //Call the 'SetCustomCursor' (see below) with a delay of 2 seconds.  
        Invoke("SetCustomCursor", 2.0f);
    }

    void OnDisable()
    {
        //Resets the cursor to the default 
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        //Set the _ccEnabled variable to false 
        this.ccEnabled = false;
    }

    private void SetCustomCursor()
    {
        //Replace the 'cursorTexture' with the cursor   
        Cursor.SetCursor(this.cursorTexture, Vector2.zero, CursorMode.Auto);
        //Set the ccEnabled variable to true 
        this.ccEnabled = true;
    }
}