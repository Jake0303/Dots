// Copyright (c) 2015 - 2016 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DoozyUI
{
    [AddComponentMenu("DoozyUI/UI Element", 1)]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(UIAnimationManager))]
    [DisallowMultipleComponent]
    public class UIElement : MonoBehaviour
    {

        #region Context Menu Methods

#if UNITY_EDITOR
        [MenuItem("DoozyUI/Components/UI Element", false, 1)]
        [MenuItem("GameObject/DoozyUI/UI Element", false, 1)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            if (GameObject.Find("UIManager") == null)
            {
                Debug.LogError("[DoozyUI] The DoozyUI system was not found in the scene. Please add it before trying to create a UI Element.");
                return;
            }
            GameObject go = new GameObject("New UIElement");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            if (go.GetComponent<Transform>() != null)
            {
                go.AddComponent<RectTransform>();
            }
            if (go.transform.parent == null)
            {
                go.transform.SetParent(UIManager.GetUiContainer);
            }
            go.GetComponent<RectTransform>().localScale = Vector3.one;
            go.AddComponent<UIElement>();
            Selection.activeObject = go;
        }
#endif
        #endregion

        #region Internal Classes --> ElementName, TriggerEvent
        [Serializable]
        public class ElementName
        {
            public string elementName = UIManager.DEFAULT_ELEMENT_NAME;
        }

        [Serializable]
        public class TriggerEvent : UnityEvent { }
        #endregion

        #region BACKUP VARIABLES
        public string elementName = UIManager.DEFAULT_ELEMENT_NAME;

        public string moveInSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string moveInSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string rotationInSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string rotationInSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string scaleInSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string scaleInSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string fadeInSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string fadeInSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;

        public string moveLoopSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string moveLoopSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string rotationLoopSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string rotationLoopSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string scaleLoopSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string scaleLoopSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string fadeLoopSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string fadeLoopSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;

        public string moveOutSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string moveOutSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string rotationOutSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string rotationOutSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string scaleOutSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string scaleOutSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        public string fadeOutSoundAtStart = UIManager.DEFAULT_SOUND_NAME;
        public string fadeOutSoundAtFinish = UIManager.DEFAULT_SOUND_NAME;
        #endregion

        #region Public Variables
        public bool showHelp = false;

        public bool LANDSCAPE = true;
        public bool PORTRAIT = true;

        public bool linkedToNotification = false; //if we link a notification to this UIElement, then we will let the UINotification auto-generate the elementName; thus we need to disable user's ability to tamper with it

        public bool useCustomStartAnchoredPosition = false;
        public Vector3 customStartAnchoredPosition = Vector3.zero;

        public ElementName elementNameReference = new ElementName();
        public bool startHidden = false;
        public bool animateAtStart = false;
        public bool disableWhenHidden = false;
        public GameObject selectedButton = null;  //this is the button that gets selected when this UIElement gets shown; if null then no button will get auto selected

        public bool autoRegister = true;    //if this element is handled by a notification, the we let the notification handle the registration process with an auto generated name
        public bool isVisible = true;

        //
        public bool useInAnimations = false;
        public bool useLoopAnimations = false;
        public bool useOutAnimations = false;

        //
        public bool useInAnimationsStartEvents = false;
        public bool useInAnimationsFinishEvents = false;
        public bool useOutAnimationsStartEvents = false;
        public bool useOutAnimationsFinishEvents = false;

        //In Animations
        public int activeInAnimationsPresetIndex = 0;
        public string[] inAnimationsPresetNames;
        public string inAnimationsPresetName = UIAnimationManager.DEFAULT_PRESET_NAME;
        public UIAnimator.MoveIn moveIn = new UIAnimator.MoveIn();
        public UIAnimator.RotationIn rotationIn = new UIAnimator.RotationIn();
        public UIAnimator.ScaleIn scaleIn = new UIAnimator.ScaleIn();
        public UIAnimator.FadeIn fadeIn = new UIAnimator.FadeIn();

        //Loop Animations
        public int activeLoopAnimationsPresetIndex = 0;
        public string[] loopAnimationsPresetNames;
        public string loopAnimationsPresetName = UIAnimationManager.DEFAULT_PRESET_NAME;
        public UIAnimator.MoveLoop moveLoop = new UIAnimator.MoveLoop();
        public UIAnimator.RotationLoop rotationLoop = new UIAnimator.RotationLoop();
        public UIAnimator.ScaleLoop scaleLoop = new UIAnimator.ScaleLoop();
        public UIAnimator.FadeLoop fadeLoop = new UIAnimator.FadeLoop();

        //Out Animations
        public int activeOutAnimationsPresetIndex = 0;
        public string[] outAnimationsPresetNames;
        public string outAnimationsPresetName = UIAnimationManager.DEFAULT_PRESET_NAME;
        public UIAnimator.MoveOut moveOut = new UIAnimator.MoveOut();
        public UIAnimator.RotationOut rotationOut = new UIAnimator.RotationOut();
        public UIAnimator.ScaleOut scaleOut = new UIAnimator.ScaleOut();
        public UIAnimator.FadeOut fadeOut = new UIAnimator.FadeOut();

        public TriggerEvent onInAnimationsStart = new TriggerEvent();
        public TriggerEvent onInAnimationsFinish = new TriggerEvent();
        public TriggerEvent onOutAnimationsStart = new TriggerEvent();
        public TriggerEvent onOutAnimationsFinish = new TriggerEvent();
        #endregion

        #region Private Variables
        [SerializeField]
        private UIAnimationManager animationManager;
        private Canvas Canvas;
        private GraphicRaycaster GraphicRaycaster;

        private RectTransform rectTransform;

        private Vector3 startAnchoredPosition3D;
        private Vector3 startRotation;
        private Vector3 startScale;


        private float disableTimeBuffer = 0.1f;
        private WaitForSeconds outAnimationsDisableBuffer; //this is the default buffer (even for instant actions)
        private WaitForSeconds outAnimationsDDisableDelay; //this will be the max time+delay for animations delay before the disable
        private Coroutine inAnimationsCoroutine;
        private Coroutine outAnimationsCoroutine;
        #endregion

        #region Properties
        public UIAnimationManager GetAnimationManager
        {
            get
            {
                if (animationManager == null)
                {
                    animationManager = GetComponent<UIAnimationManager>();
                    if (animationManager == null)
                        animationManager = gameObject.AddComponent<UIAnimationManager>();
                }
                return animationManager;
            }
        }

        public string[] GetInAnimationsPresetNames
        {
            get
            {
                GetAnimationManager.LoadPresetList(UIAnimationManager.AnimationType.IN);
                return inAnimationsPresetNames;
            }
        }

        public string[] GetLoopAnimationsPresetNames
        {
            get
            {
                GetAnimationManager.LoadPresetList(UIAnimationManager.AnimationType.LOOP);
                return loopAnimationsPresetNames;
            }
        }

        public string[] GetOutAnimationsPresetNames
        {
            get
            {
                GetAnimationManager.LoadPresetList(UIAnimationManager.AnimationType.OUT);
                return outAnimationsPresetNames;
            }
        }

        public UIAnimationManager.InAnimations GetInAnimations
        {
            get
            {
                UIAnimationManager.InAnimations inAnimations = new UIAnimationManager.InAnimations();
                inAnimations.inAnimationsPresetName = inAnimationsPresetName;
                inAnimations.moveIn = moveIn;
                inAnimations.rotationIn = rotationIn;
                inAnimations.scaleIn = scaleIn;
                inAnimations.fadeIn = fadeIn;
                return inAnimations;
            }
        }

        public UIAnimationManager.InAnimations SetInAnimations
        {
            set
            {
                inAnimationsPresetName = value.inAnimationsPresetName;
                moveIn = value.moveIn;
                rotationIn = value.rotationIn;
                scaleIn = value.scaleIn;
                fadeIn = value.fadeIn;
            }
        }

        public UIAnimationManager.LoopAnimations GetLoopAnimations
        {
            get
            {
                UIAnimationManager.LoopAnimations loopAnimations = new UIAnimationManager.LoopAnimations();
                loopAnimations.loopAnimationsPresetName = loopAnimationsPresetName;
                loopAnimations.moveLoop = moveLoop;
                loopAnimations.rotationLoop = rotationLoop;
                loopAnimations.scaleLoop = scaleLoop;
                loopAnimations.fadeLoop = fadeLoop;
                return loopAnimations;
            }
        }

        public UIAnimationManager.LoopAnimations SetLoopAnimations
        {
            set
            {
                loopAnimationsPresetName = value.loopAnimationsPresetName;
                moveLoop = value.moveLoop;
                rotationLoop = value.rotationLoop;
                scaleLoop = value.scaleLoop;
                fadeLoop = value.fadeLoop;
            }
        }

        public UIAnimationManager.OutAnimations GetOutAnimations
        {
            get
            {
                UIAnimationManager.OutAnimations outAnimations = new UIAnimationManager.OutAnimations();
                outAnimations.outAnimationsPresetName = outAnimationsPresetName;
                outAnimations.moveOut = moveOut;
                outAnimations.rotationOut = rotationOut;
                outAnimations.scaleOut = scaleOut;
                outAnimations.fadeOut = fadeOut;
                return outAnimations;
            }
        }

        public UIAnimationManager.OutAnimations SetOutAnimations
        {
            set
            {
                outAnimationsPresetName = value.outAnimationsPresetName;
                moveOut = value.moveOut;
                rotationOut = value.rotationOut;
                scaleOut = value.scaleOut;
                fadeOut = value.fadeOut;
            }
        }

        public UIAnimator.InitialData GetInitialData
        {
            get
            {
                UIAnimator.InitialData initialData = new UIAnimator.InitialData();
                initialData.startAnchoredPosition3D = startAnchoredPosition3D;
                initialData.startRotation = startRotation;
                initialData.startScale = startScale;
                initialData.startFadeAlpha = 1f;
                initialData.soundOn = UIManager.isSoundOn;

                return initialData;
            }
        }

        public RectTransform GetRectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        /// <summary>
        /// Retruns TRUE if at least one IN Animation is enabled. Otherwise it returns FALSE
        /// </summary>
        public bool AreInAnimationsEnabled
        {
            get
            {
                if (moveIn.enabled) return true;
                else if (rotationIn.enabled) return true;
                else if (scaleIn.enabled) return true;
                else if (fadeIn.enabled) return true;
                else return false;
            }
        }

        /// <summary>
        /// Retruns TRUE if at least one LOOP Animation is enabled. Otherwise it returns FALSE
        /// </summary>
        public bool AreLoopAnimationsEnabled
        {
            get
            {
                if (moveLoop.enabled) return true;
                else if (rotationLoop.enabled) return true;
                else if (scaleLoop.enabled) return true;
                else if (fadeLoop.enabled) return true;
                else return false;
            }
        }

        /// <summary>
        /// Retruns TRUE if at least one OUT Animation is enabled. Otherwise it returns FALSE
        /// </summary>
        public bool AreOutAnimationsEnabled
        {
            get
            {
                if (moveOut.enabled) return true;
                else if (rotationOut.enabled) return true;
                else if (scaleOut.enabled) return true;
                else if (fadeOut.enabled) return true;
                else return false;
            }
        }
        #endregion

        void Awake()
        {
            outAnimationsDisableBuffer = new WaitForSeconds(disableTimeBuffer);
            Canvas = GetComponent<Canvas>();
            if (Canvas == null)
            {
                Canvas = gameObject.AddComponent<Canvas>();
                Debug.Log("[DoozyUI] Adding the missing component <Canvas> to [" + name + "]. The UIElements needs to have a <Canvas> component attached and it might not be visible because of this.");
            }
            GraphicRaycaster = GetComponent<GraphicRaycaster>();
            if (GraphicRaycaster == null)
            {
                GraphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log("[DoozyUI] Adding the missing component <GraphicRaycaster> to [" + name + "]. The UIElements needs to have a <GraphicRaycaster> component attached and it might not receive clicks because of this.");
            }

            if (autoRegister)
                UIManager.RegisterUiElement(this);

            if (useCustomStartAnchoredPosition)
            {
                GetRectTransform.anchoredPosition3D = customStartAnchoredPosition;
            }

            startAnchoredPosition3D = GetRectTransform.anchoredPosition3D;
            startRotation = GetRectTransform.localRotation.eulerAngles;
            startScale = GetRectTransform.localScale;
        }

        void OnEnable()
        {
            //if (autoRegister)
            //    UIManager.RegisterUiElement(this);
        }

        void Start()
        {
            SetupElement();
            InitLoopAnimations();
        }

        void OnDestroy()
        {
            UIManager.UnregisterUiElement(this);
        }

        #region Setup Element Methods
        /// <summary>
        /// Setups the element.
        /// </summary>
        void SetupElement()
        {
            if (animateAtStart)
            {
                if (linkedToNotification)
                {
                    Hide(true, false);
                    Show(false);
                }
                else
                {
                    if (UIManager.useOrientationManager)
                    {
                        if (UIManager.currentOrientation == UIManager.Orientation.Unknown)
                        {
                            StartCoroutine(GetOrientation());
                        }
                        else
                        {
                            if (LANDSCAPE && UIManager.currentOrientation == UIManager.Orientation.Landscape)
                            {
                                UIManager.HideUiElement(elementName, true, disableWhenHidden);
                                UIManager.ShowUiElement(elementName, false);
                            }
                            else if (PORTRAIT && UIManager.currentOrientation == UIManager.Orientation.Portrait)
                            {
                                UIManager.HideUiElement(elementName, true, disableWhenHidden);
                                UIManager.ShowUiElement(elementName, false);
                            }
                        }
                    }
                    else
                    {
                        UIManager.HideUiElement(elementName, true, disableWhenHidden);
                        UIManager.ShowUiElement(elementName, false);
                    }
                }
            }
            else if (startHidden)
            {
                UIManager.HideUiElement(elementName, true, disableWhenHidden);
            }
        }
        #endregion

        #region Show Methods (IN Animations)

        /// <summary>
        /// Shows the element.
        /// </summary>
        /// <param name="instantAction">If set to <c>true</c> it will execute the animations in 0 seconds and with 0 delay</param>
        public void Show(bool instantAction)
        {
            if (outAnimationsCoroutine != null)
            {
                isVisible = false;
                StopCoroutine(outAnimationsCoroutine);
                outAnimationsCoroutine = null;
            }

            if (AreInAnimationsEnabled)
            {
                if (isVisible == false)
                {
                    TriggerInAnimationsEvents();
                    UIAnimator.StopOutAnimations(GetRectTransform, GetInitialData);
                    inAnimationsCoroutine = StartCoroutine(InAnimationsEnumerator(instantAction));
                    isVisible = true;
                }
            }
            else if (AreInAnimationsEnabled == false)
            {
                Debug.LogWarning("[DoozyUI] [" + name + "] You are trying to SHOW the " + elementName + " UIElement, but you didn't enable any IN animations. To fix this warning you should enable at least one IN animation.");
            }
        }

        IEnumerator SetSelectedGameObject()
        {
            int infiniteLoopCount = 0;

            while (UIManager.GetEventSystem == null)
            {
                yield return new WaitForEndOfFrame();

                infiniteLoopCount++;
                if (infiniteLoopCount > 1000)
                    break;
            }

            UIManager.GetEventSystem.SetSelectedGameObject(selectedButton);
        }

        IEnumerator InAnimationsEnumerator(bool instantAction)
        {
            //if (UIManager.firstPass)
            //{
            //    //We need this WaitForEndOfFrame so that the UIManager gets on the first frame the UIScreenRect size and position
            //    yield return new WaitForEndOfFrame();
            //}

            yield return new WaitForEndOfFrame();

            UIAnimator.StopLoopAnimations(GetRectTransform, GetInitialData);

            ToggleCanvasAndGraphicRaycaster(true);

            UIAnimator.DoMoveIn(moveIn, GetRectTransform, GetInitialData, instantAction);
            UIAnimator.DoRotationIn(rotationIn, GetRectTransform, GetInitialData, instantAction);
            UIAnimator.DoScaleIn(scaleIn, GetRectTransform, GetInitialData, instantAction);
            UIAnimator.DoFadeIn(fadeIn, GetRectTransform, GetInitialData, instantAction);

            StartCoroutine("SetSelectedGameObject");

            inAnimationsCoroutine = null;
            yield return null;
        }

        #endregion

        #region Loop Methods (LOOP Animations)

        /// <summary>
        /// Initiates (if enabled) and plays (if set to autoStart) the idle animations.
        /// </summary>
        public void InitLoopAnimations()
        {
            if (AreLoopAnimationsEnabled)
                StartCoroutine(LoopAnimationsEnumerator());
        }

        IEnumerator LoopAnimationsEnumerator()
        {
            //We need this WaitForEndOfFrame so that the UIManager gets on the first frame the UIScreenRect size and position
            yield return new WaitForEndOfFrame();
            UIAnimator.DoMoveLoop(moveLoop, GetRectTransform, GetInitialData);
            UIAnimator.DoRotationLoop(rotationLoop, GetRectTransform, GetInitialData);
            UIAnimator.DoScaleLoop(scaleLoop, GetRectTransform, GetInitialData);
            UIAnimator.DoFadeLoop(fadeLoop, GetRectTransform, GetInitialData);
        }

        #endregion

        #region Hide Methods (OUT Animations)

        public void Hide(bool instantAction)
        {
            Hide(instantAction, true);
        }

        /// <summary>
        /// Hides the element.
        /// </summary>
        /// <param name="instantAction">If set to <c>true</c> it will execute the animations in 0 seconds and with 0 delay</param>
        public void Hide(bool instantAction, bool shouldDisable)
        {
            if (inAnimationsCoroutine != null)
            {
                isVisible = true;
                StopCoroutine(inAnimationsCoroutine);
                inAnimationsCoroutine = null;
            }

            if (AreOutAnimationsEnabled)
            {
                if (isVisible)
                {
                    if (instantAction == false) //we do this check so that the events are not triggered onEnable when we have startHidden set as true
                    {
                        TriggerOutAnimationsEvents();
                    }
                    UIAnimator.StopInAnimations(GetRectTransform, GetInitialData);
                    outAnimationsCoroutine = StartCoroutine(OutAnimationsEnumerator(instantAction, shouldDisable));
                    isVisible = false;
                }
            }
            else if (AreOutAnimationsEnabled == false)
            {
                Debug.LogWarning("[DoozyUI] [" + name + "] You are trying to HIDE the " + elementName + " UIElement, but you didn't enable any OUT animations. To fix this warning you should enable at least one OUT animation.");
            }
        }

        IEnumerator OutAnimationsEnumerator(bool instantAction, bool shouldDisable = true)
        {
            //if (UIManager.firstPass)
            //{
            //    //We need this WaitForEndOfFrame so that the UIManager gets after the first frame the UIScreenRect size and position
            //    yield return new WaitForEndOfFrame();
            //}

            UIAnimator.StopLoopAnimations(GetRectTransform, GetInitialData);

            UIAnimator.DoMoveOut(moveOut, GetRectTransform, GetInitialData, instantAction);
            UIAnimator.DoRotationOut(rotationOut, GetRectTransform, GetInitialData, instantAction);
            UIAnimator.DoScaleOut(scaleOut, GetRectTransform, GetInitialData, instantAction);
            UIAnimator.DoFadeOut(fadeOut, GetRectTransform, GetInitialData, instantAction);

            if (disableWhenHidden) //do we have disableWhenHidden available for this UIElement?
            {
                if (shouldDisable) //should this ui element get disabled?
                {
                    yield return outAnimationsDisableBuffer; //default wait time before the disable
                    if (instantAction == false) //is this an instant animation
                    {
                        outAnimationsDDisableDelay = new WaitForSeconds(GetOutAnimationsFinishTime()); //we get the max wait time
                        yield return outAnimationsDDisableDelay; //we wait
                    }

                    ToggleCanvasAndGraphicRaycaster(false);
                    gameObject.SetActive(false); //we disable the UIElement gameObject
                }
            }
            else
            {
                if (instantAction == false) //is this an instant animation
                {
                    outAnimationsDDisableDelay = new WaitForSeconds(GetOutAnimationsFinishTime()); //we get the max wait time
                    yield return outAnimationsDDisableDelay; //we wait
                }

                ToggleCanvasAndGraphicRaycaster(false);
            }

            outAnimationsCoroutine = null;
            yield return null;
        }
        #endregion

        #region Animation Start and Finish Times
        /// <summary>
        /// This returns the start time of the IN Animations, taking into account all the delays. It retruns the minimum animationStartDelay.
        /// How long does the animation take to start?
        /// It will return -1 if no IN Animations are enabled
        /// </summary>
        /// <returns></returns>
        private float GetInAnimationsStartTime()
        {
            if (moveIn.enabled || rotationIn.enabled || scaleIn.enabled || fadeIn.enabled)
            {
                float[] startTimes = new float[4] { 10000, 10000, 10000, 10000 };
                if (moveIn.enabled) startTimes[0] = moveIn.delay;
                if (rotationIn.enabled) startTimes[1] = rotationIn.delay;
                if (scaleIn.enabled) startTimes[2] = scaleIn.delay;
                if (fadeIn.enabled) startTimes[3] = fadeIn.delay;
                return Mathf.Min(startTimes);
            }
            return -1f;
        }

        /// <summary>
        /// This returns the finish time of the IN Animations, taking into account all the delays. It retruns the maximum animationStatDelay + the maximum animationTime.
        /// How long does the animation take to finish?
        /// It will return -1 if no IN Animations are enabled
        /// </summary>
        /// <returns></returns>
        private float GetInAnimationsFinishTime()
        {
            if (moveIn.enabled || rotationIn.enabled || scaleIn.enabled || fadeIn.enabled)
            {
                float[] finishTimes = new float[4] { 0, 0, 0, 0 };
                if (moveIn.enabled) finishTimes[0] = moveIn.time + moveIn.delay;
                if (rotationIn.enabled) finishTimes[1] = rotationIn.time + rotationIn.delay;
                if (scaleIn.enabled) finishTimes[2] = scaleIn.time + scaleIn.delay;
                if (fadeIn.enabled) finishTimes[3] = fadeIn.time + fadeIn.delay;
                return Mathf.Max(finishTimes);
            }
            return -1f;
        }

        /// <summary>
        /// This returns the start time of the OUT Animations, taking into account all the delays. It retruns the minimum animationStartDelay.
        /// How long does the animation take to start?
        /// It will return -1 if no OUT Animations are enabled
        /// </summary>
        /// <returns></returns>
        private float GetOutAnimationsStartTime()
        {
            if (moveOut.enabled || rotationOut.enabled || scaleOut.enabled || fadeOut.enabled)
            {
                float[] startTimes = new float[4] { 10000, 10000, 10000, 10000 };
                if (moveOut.enabled) startTimes[0] = moveOut.delay;
                if (rotationOut.enabled) startTimes[1] = rotationOut.delay;
                if (scaleOut.enabled) startTimes[2] = scaleOut.delay;
                if (fadeOut.enabled) startTimes[3] = fadeOut.delay;
                return Mathf.Min(startTimes);
            }
            return -1f;
        }

        /// <summary>
        /// This returns the finish time of the OUT Animations, taking into account all the delays. It retruns the maximum animationStatDelay + the maximum animationTime.
        /// How long does the animation take to finish?
        /// It will return -1 if no OUT Animations are enabled
        /// </summary>
        /// <returns></returns>
        private float GetOutAnimationsFinishTime()
        {
            if (moveOut.enabled || rotationOut.enabled || scaleOut.enabled || fadeOut.enabled)
            {
                float[] finishTimes = new float[4] { 0, 0, 0, 0 };
                if (moveOut.enabled) finishTimes[0] = moveOut.time + moveOut.delay;
                if (rotationOut.enabled) finishTimes[1] = rotationOut.time + rotationOut.delay;
                if (scaleOut.enabled) finishTimes[2] = scaleOut.time + scaleOut.delay;
                if (fadeOut.enabled) finishTimes[3] = fadeOut.time + fadeOut.delay;
                return Mathf.Max(finishTimes);
            }
            return -1f;
        }
        #endregion

        #region Events

        #region IN Animations
        /// <summary>
        /// Triggers the IN Animations Events, if enabled.
        /// </summary>
        private void TriggerInAnimationsEvents()
        {
            if (useInAnimationsStartEvents)
            {
                if (GetInAnimationsStartTime() == -1)
                {
                    Debug.Log("[DoozyUI] You have activated IN Animations Start Events for the " + elementNameReference.elementName + " UIElement on " + gameObject.name + " gameObject, but you did not enable any IN animations. Nothing happened!");
                }
                else
                {
                    StartCoroutine(TriggerInAnimaionsStartEvents(GetInAnimationsStartTime()));
                }
            }

            if (useInAnimationsFinishEvents)
            {
                if (GetInAnimationsFinishTime() == -1)
                {
                    Debug.Log("[DoozyUI] You have activated IN Animations Finish Events for the " + elementNameReference.elementName + " UIElement on " + gameObject.name + " gameObject, but you did not enable any IN animations. Nothing happened!");
                }
                else
                {
                    StartCoroutine(TriggerInAnimaionsFinishEvents(GetInAnimationsFinishTime()));
                }
            }
        }

        IEnumerator TriggerInAnimaionsStartEvents(float delay)
        {
            yield return new WaitForSeconds(delay);
            onInAnimationsStart.Invoke();
        }

        IEnumerator TriggerInAnimaionsFinishEvents(float delay)
        {
            yield return new WaitForSeconds(delay);
            onInAnimationsFinish.Invoke();
        }
        #endregion

        #region OUT Animations
        /// <summary>
        /// Triggers the OUT Animations Events, if enabled.
        /// </summary>
        private void TriggerOutAnimationsEvents()
        {
            if (useOutAnimationsStartEvents)
            {
                if (GetOutAnimationsStartTime() == -1)
                {
                    Debug.Log("[DoozyUI] You have activated OUT Animations Start Events for the " + elementNameReference.elementName + " UIElement on " + gameObject.name + " gameObject, but you did not enable any OUT animations. Nothing happened!");
                }
                else
                {
                    StartCoroutine(TriggerOutAnimaionsStartEvents(GetOutAnimationsStartTime()));
                }
            }

            if (useOutAnimationsFinishEvents)
            {
                if (GetOutAnimationsFinishTime() == -1)
                {
                    Debug.Log("[DoozyUI] You have activated OUT Animations Finish Events for the " + elementNameReference.elementName + " UIElement on " + gameObject.name + " gameObject, but you did not enable any OUT animations. Nothing happened!");
                }
                else
                {
                    StartCoroutine(TriggerOutAnimaionsFinishEvents(GetOutAnimationsFinishTime()));
                }
            }
        }

        IEnumerator TriggerOutAnimaionsStartEvents(float delay)
        {
            yield return new WaitForSeconds(delay);
            onOutAnimationsStart.Invoke();
        }

        IEnumerator TriggerOutAnimaionsFinishEvents(float delay)
        {
            yield return new WaitForSeconds(delay);
            onOutAnimationsFinish.Invoke();
        }
        #endregion

        #endregion

        void ToggleCanvasAndGraphicRaycaster(bool isEnabled)
        {
            Canvas.enabled = isEnabled;
            GraphicRaycaster.enabled = isEnabled;
        }

        IEnumerator GetOrientation()
        {
            while (UIManager.currentOrientation == UIManager.Orientation.Unknown)
            {
                UIManager.CheckDeviceOrientation();
                if (UIManager.currentOrientation != UIManager.Orientation.Unknown)
                    break;

                yield return new WaitForEndOfFrame();
            }

            if (LANDSCAPE && UIManager.currentOrientation == UIManager.Orientation.Landscape)
            {
                UIManager.HideUiElement(elementName, true, disableWhenHidden);
                UIManager.ShowUiElement(elementNameReference.elementName, false);
            }
            else if (PORTRAIT && UIManager.currentOrientation == UIManager.Orientation.Portrait)
            {
                UIManager.HideUiElement(elementName, true, disableWhenHidden);
                UIManager.ShowUiElement(elementNameReference.elementName, false);
            }

        }
    }
}

