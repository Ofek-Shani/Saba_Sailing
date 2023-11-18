using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
// (c) 2023 copyright Uri Shani, Ofek Shani


public class gestures : MonoBehaviour
{
    [SerializeField] Slider sails, steering, keel;
    public GameObject theWater;
    public Button hitLeft, hitRight, adamBayamB, plus, minus;
    public GameObject adamBayamBuoy, pausePlayB, fullScreenB, infoP, helpB, restartB, exitB;
    public GameObject confirmP; 
    // public Camera camera;
    bool pauseIsOn = true;
    Image pauseImg, playImg, expandScreenImg, shrinkScreenImg, forbidImg;
    GameObject canvas;
    public class KeyTracking
    {
        KeyCode keyCode;
        bool stateUp = true;
        bool trace = false;
        public KeyTracking(KeyCode keyCode_, bool trace_ = false)
        {
            keyCode = keyCode_;
            trace = trace_;
        }
        public bool clicked()
        {
            bool keyDown = Input.GetKeyDown(keyCode);
            if (trace && keyDown) {
                Debug.Log("Key [" + keyCode + "]: " + Input.GetKeyDown(keyCode) + ". state: " + stateUp);
            }
            if (keyDown)
            {
                if (stateUp)
                {
                    stateUp = false;
                    return true;
                }
                else return false;
            }
            else
            {
                stateUp = true;
                return false;
            }
        }

    }
    KeyTracking hitLeftK = new KeyTracking(KeyCode.LeftShift),
        hitRightK = new KeyTracking(KeyCode.RightShift),
        sailsUpK = new KeyTracking(KeyCode.UpArrow),
        sailsDownK = new KeyTracking(KeyCode.DownArrow),
        steeringLeft = new KeyTracking(KeyCode.LeftArrow),
        steeringRight = new KeyTracking(KeyCode.RightArrow),
        adamBayamK = new KeyTracking(KeyCode.Space),
        plusK = new KeyTracking(KeyCode.Equals),
        minusK = new KeyTracking(KeyCode.Minus),
        keelUpK = new KeyTracking(KeyCode.PageUp),
        keelDownK = new KeyTracking(KeyCode.PageDown),
        zoomK = new KeyTracking(KeyCode.Z),
        followK = new KeyTracking(KeyCode.X),
        f11K = new KeyTracking(KeyCode.F11),
        exitK = new KeyTracking(KeyCode.C),
        togglePausePlayK = new KeyTracking(KeyCode.P),
        restartK = new KeyTracking(KeyCode.R),
        showHideHelpK = new KeyTracking(KeyCode.Slash),
        hK = new KeyTracking(KeyCode.H),
        escK = new KeyTracking(KeyCode.Escape, false),
        enterK = new KeyTracking(KeyCode.Return, false),
        ancorK = new KeyTracking(KeyCode.A);
    Color normalHelpBColor, normalRestartBColor, normalQuitBColor, normalPausePlayBColor;
    // Start is called before the first frame update

    BoatController boat() { return BoatController.Instance; }
    AdamBayam adamBayam() { return AdamBayam.Instance; }
    void Start()
    {
        pauseImg = pausePlayB.transform.GetChild(0).GetComponent<Image>();
        playImg = pausePlayB.transform.GetChild(1).GetComponent<Image>();
        expandScreenImg = fullScreenB.transform.GetChild(0).GetComponent<Image>();
        shrinkScreenImg = fullScreenB.transform.GetChild(1).GetComponent<Image>();
        forbidImg = fullScreenB.transform.GetChild(2).GetComponent<Image>();
#if UNITY_EDITOR
       // fullScreenB.GetComponent<Button>().interactable = false;
        forbidImg.gameObject.SetActive(true); // show that it is inactive.
#endif
        canvas = infoP.transform.parent.gameObject;
        SetCanvasScale();
    }

    Vector2 screenSize;
    void SetCanvasScale() {
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize == screenSize)  return;
        screenSize = currentScreenSize;
        Debug.Log("screen size: " + screenSize);
        float ss = (Screen.width + Screen.height) /2f;
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        float cs = 1.3f * ss / 3000f;
        // if (ss > 3000) cs = 1.3f;
        // else if (ss > 2000) cs = 1f;
        // else if (ss > 1000) cs = 0.8f;
        // else cs = 0.5f;
        Debug.Log("Setting scale to: " + cs);
        canvasScaler.scaleFactor = cs;
    }
    void TestExitRequested()
    { // exits the game if CTRL-C is clicked.
        if (exitK.clicked() && (
            Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.RightControl)))
        {
            doExit();                   
        }
    }

    // Update is called once per frame
    GameObject inFocus;
    void Update()
    {   GameObject infocusNow = EventSystem.current.currentSelectedGameObject;
        if (inFocus != infocusNow) {
            Debug.Log("focus changed: " + inFocus + " -> " + infocusNow);
            inFocus = infocusNow; 
        }
        SetCanvasScale();
        // EventSystem.current.SetSelectedGameObject(gameObject);
        // Debug.Log("escape: " + Input.GetKey(KeyCode.Escape) + ", return: " + Input.GetKey(KeyCode.Return));
        if (confirm().active && !confirm().done) return;
        TestExitRequested();
        if (hitLeftK.clicked()) hitLeft.onClick.Invoke();
        if (hitRightK.clicked()) hitRight.onClick.Invoke();
        if (adamBayamK.clicked())
        {
            AdamBayam ab = adamBayam();
            if (ab.adamBayamIsOn)
            {
                ab.adamBayamIsOn = false;
                ab.OnMouseDown(); // Buoy.SendMessage("OnMouseDown");
            }
            else
            {
                adamBayamB.onClick.Invoke();
                ab.adamBayamIsOn = true;
            }
        }
        if (plusK.clicked()) plus.onClick.Invoke();
        if (minusK.clicked()) minus.onClick.Invoke();
        if (sailsUpK.clicked()) sails.value = Mathf.Max(sails.minValue, sails.value - 1);
        if (sailsDownK.clicked()) sails.value = Mathf.Min(sails.maxValue, sails.value + 1);
        if (steeringLeft.clicked()) steering.value = Mathf.Max(steering.minValue, steering.value - 1);
        if (steeringRight.clicked()) steering.value = Mathf.Min(steering.maxValue, steering.value + 1);
        if (keelUpK.clicked()) keel.value = Mathf.Max(keel.minValue, keel.value - 1);
        if (keelDownK.clicked()) keel.value = Mathf.Min(keel.maxValue, keel.value + 1);
        if (zoomK.clicked()) { Camera.main.orthographicSize = (Camera.main.orthographicSize == 10f) ? 30f : 10f; }
        if (followK.clicked()) { CameraController.Instance.withOrientation = !CameraController.Instance.withOrientation; }
        if (togglePausePlayK.clicked()) {doPausePlay();}
        if (restartK.clicked()) {doRestart();}
        if (showHideHelpK.clicked()) {doHelp();}
        if (escK.clicked()) {doEscape();}
        if (enterK.clicked()) {doEnter();}
        if (f11K.clicked()) { doFullScreen();}
        if (ancorK.clicked()) {boat().ToggleAncor();}
    }

    public class Confirm {
        public bool active = false, done = false, confirmed = false;
        GameObject confirmP;
        Action<bool> action;
        public Confirm(GameObject confirmP_) {
            confirmP = confirmP_;
        }
        public void Start(Action<bool> action_) { 
            active = true; confirmed = false; 
            confirmP.gameObject.SetActive(true); 
            action = action_;
        }
        public void Stop(bool confirmed_) {
            active = false;
            done = true; 
            confirmP.gameObject.SetActive(false);
            confirmed = confirmed_;
            action.Invoke(confirmed);
        }
    }

    private static Confirm confirm_;
    public Confirm confirm() {
        if (confirm_ == null) confirm_ = new Confirm(confirmP);
        return confirm_;
    }

    bool isConfirmOn() {return confirm().active;}
    bool isHelpOn() { return infoP.gameObject.activeSelf;}
    bool isFullScreen() {return Screen.fullScreen;}

    public void doHelp() {
        infoP.gameObject.SetActive(!isHelpOn());
    }

    public void doEscape() {
        Debug.Log("escape");
        if (isHelpOn()) doCloseHelp();
        else if (isConfirmOn()) doConfirmCancel();
    }
    public void doEnter() {
        Debug.Log("enter");
        if (isHelpOn()) doCloseHelp();
        else if (isConfirmOn()) doConfirmOK();
    }

    public void doRestart() {
        boat().Reset();
        AdamBayam ab = adamBayam();
        if (ab.adamBayamIsOn)
        {
            ab.adamBayamIsOn = false;
            ab.OnMouseDown(); // Buoy.SendMessage("OnMouseDown");
        }

    }
    void exitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    public void doExit() {
        if (confirm().active) return;
        confirm().Start((ok) => {
            if (ok) exitGame();
        });
    }
    public void doConfirmOK() {
        if (confirm().active) confirm().Stop(true);
    }
    public void doConfirmCancel() {
        if (confirm().active) confirm().Stop(false);
    }

    public void doCloseHelp(){
        infoP.gameObject.SetActive(false);
    }


    public void doShowHelp(){
        infoP.gameObject.SetActive(!isHelpOn()); // toggle its active state.
    }
    void PauseGame() {
        Time.timeScale = 0;
        Physics.autoSimulation = false;
    }
    void PlayGame() {
        Time.timeScale = 1;
        Physics.autoSimulation = true;
    }
    public void doPausePlay() {
        if (pauseIsOn) PauseGame();
        else PlayGame();
        pauseIsOn = !pauseIsOn;
        pauseImg.gameObject.SetActive(pauseIsOn);
        playImg.gameObject.SetActive(!pauseIsOn);
    }
    public void doFullScreen() {
        Debug.Log("doFullScreen");
        bool fs = isFullScreen();
        Screen.fullScreen = !Screen.fullScreen;
        if (fs == isFullScreen()) {
            // does not change status, so lit both image on the button
        };
        shrinkScreenImg.gameObject.SetActive(fs); //isFullScreen());
        expandScreenImg.gameObject.SetActive(!fs); //!isFullScreen());
    }

}
