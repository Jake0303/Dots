using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
public class PlayerName : NetworkBehaviour {
    [SyncVar (hook="nameChanged")]
    public string enteredName;

    public void setName()
    {
        if(isLocalPlayer)
            enteredName = GameObject.Find("EnterNameInputField").GetComponent<InputField>().text;
    }

	void nameChanged(string name)
    {
        if (isLocalPlayer)
            enteredName = name;
    }
	
}
