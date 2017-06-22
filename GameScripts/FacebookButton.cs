 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FacebookButton : MonoBehaviour {

    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            var callback = new EventTrigger.TriggerEvent();
            callback.RemoveAllListeners();
            callback.AddListener(e => GameObject.Find("MenuManager").GetComponent<FacebookManager>().FBButtonClick());
            EventTrigger eventTrigger = GetComponent<EventTrigger>();
            eventTrigger.triggers.Add(new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerDown,
                callback = callback
            });
        }
    }
}
