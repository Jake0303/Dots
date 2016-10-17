// Copyright (c) 2015 - 2016 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if dUI_PlayMaker
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DoozyUI
{
    [AddComponentMenu("DoozyUI/Playmaker/Event Dispatcher", 10)]
    [RequireComponent(typeof(PlayMakerFSM))]
    public class PlaymakerEventDispatcher : MonoBehaviour
    {

        #region Context Menu
#if UNITY_EDITOR
        [MenuItem("DoozyUI/Playmaker/Event Dispatcher", false, 100)]
        [MenuItem("GameObject/DoozyUI/Playmaker/Event Dispatcher", false, 100)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            if (GameObject.Find("UIManager") == null)
            {
                Debug.LogError("[DoozyUI] The DoozyUI system was not found in the scene. Please add it before trying to create a DoozyUI Playmaker Event Dispatcher.");
                return;
            }
            GameObject go = new GameObject("New PlaymakerEventDispatcher");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            go.AddComponent<PlaymakerEventDispatcher>();
            Selection.activeObject = go;
        }
#endif
        #endregion

        #region Public Variables
        [HideInInspector]
        public bool showHelp = false;
        public bool debugThis = false;

        public bool overrideTargetFSM = false;
        public PlayMakerFSM targetFSM;

        public bool dispatchGameEvents = false;
        public bool dispatchButtonClicks = false;
        #endregion

        void Start()
        {
            if (targetFSM == null)
            {
                Debug.Log("[DoozyUI] The targetFSM for the Event Dispacher attached to [" + gameObject.name + "] gameObject is null. This dispatcher is disabled.");
            }
            UpdateTargetFSM();
        }

        void OnEnable()
        {
           DoozyUI.UIManager.RegisterPlaymakerEventDispatcher(this);
        }

        void OnDisable()
        {
           DoozyUI.UIManager.UnregisterPlaymakerEventDispatcher(this);
        }

        #region Update TargetFSM
        public void UpdateTargetFSM()
        {
            if (overrideTargetFSM == false)
            {
                targetFSM = gameObject.GetComponent<PlayMakerFSM>();
            }
        }
        #endregion

        #region Dispatch Event
        public void DispatchEvent(string eventValue,DoozyUI.UIManager.EventType eventType)
        {
            switch (eventType)
            {
                caseDoozyUI.UIManager.EventType.GameEvent:
                    if (dispatchGameEvents)
                    {
                        StartCoroutine(WaitAndSendEvent(eventValue));
                    }
                    break;

                caseDoozyUI.UIManager.EventType.ButtonClick:
                    if (dispatchButtonClicks)
                    {
                        StartCoroutine(WaitAndSendEvent(eventValue));
                    }
                    break;
            }
        }
        #endregion

        #region IEnumerator Wait And Send Event
        private IEnumerator WaitAndSendEvent(string eventValue)
        {
            yield return new WaitForEndOfFrame();

            if (targetFSM == null)
            {
                Debug.LogWarning("[DoozyUI] The targetFSM for the Event Dispacher attached to [" + gameObject.name + "] gameObject is null. This should not happem.");
            }
            else
            {
                targetFSM.SendEvent(eventValue);

                if (debugThis)
                    Debug.Log("[DoozyUI] - PlaymakerEventDispatcher - Sent Event: [" + eventValue + "] to the [" + targetFSM.FsmName + "] FSM that is attached to the [" + targetFSM.name + "] gameObject.");
            }
        }
        #endregion
    }
}
#endif
