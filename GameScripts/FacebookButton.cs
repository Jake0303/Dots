using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FacebookButton : MonoBehaviour {

    void Start()
    {
        var callback = new EventTrigger.TriggerEvent();
        callback.AddListener(e => GameObject.Find("MenuManager").GetComponent<FacebookManager>().FBButtonClick());
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        eventTrigger.triggers.Add(new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerDown,
            callback = callback
        });
    }
}
