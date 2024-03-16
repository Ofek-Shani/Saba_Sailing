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
using UnityEngine.UIElements;
// (c) 2023 copyright Uri Shani, Ofek Shani


public class gestures : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Slider sails, steering, keel;
    public GameObject theWater;
    public UnityEngine.UI.Button hitLeft, hitRight, adamBayamB, plus, minus;
    public GameObject adamBayamBuoy, pausePlayB, zoomB, f11B, followB, infoP, helpB, restartB, exitB;
    public GameObject confirmP; 
    // public Camera camera;
    bool pauseIsOn = true;
    UnityEngine.UI.Image pauseImg, playImg, followBoatImg, followWorldImg; //, zoomOutImg, zoomInImg;
    private Canvas canvas;
    private CanvasScaler canvasScaler;

    public class KeyTracking
    {
        float clickTimeout = 0.15f, firstClickTimeout = 0.35f;
        float lastClick = 0;
        KeyCode keyCode;
        bool stateUp = true;
        bool trace = false, multi = false, multiActive = false;
        public KeyTracking(KeyCode keyCode_, bool multi_ = false, bool trace_ = false)
        {
            keyCode = keyCode_;
            trace = trace_;
            multi = multi_;
        }
        public bool Clicked()
        {
            bool keyDown = Input.GetKeyDown(keyCode);
            bool keyUp = Input.GetKeyUp(keyCode);
            if (trace ){//&& keyDown) {
                Debug.Log("Key [" + keyCode + "]: " + keyDown + "/" + keyUp);
            }
            if (!multi) return keyDown;
            // multi mode key type:
            if (keyUp) {
                stateUp = true;
                multiActive = false;
                lastClick = 0;
                return false;
            }
            if (keyDown) {
                stateUp = false;
                lastClick = Time.time; 
                return true;
            }
            // key not changing its stat. 
            // if it was not released yet (it is still down) work with the time:
            if (!stateUp) { // it is still pressed down
                float deltaTime = Time.time - lastClick;
                if (!multiActive && deltaTime > firstClickTimeout) {
                    multiActive = true;
                    deltaTime = Mathf.Clamp(deltaTime, 0, firstClickTimeout + clickTimeout - 0.05f);
                    lastClick = Time.time - (deltaTime - firstClickTimeout);
                    return true;
                } else if (multiActive && deltaTime > clickTimeout) {
                    deltaTime = Mathf.Clamp(deltaTime, 0, 2*clickTimeout - 0.05f);
                    lastClick = Time.time - (deltaTime - clickTimeout);
                    return true;
                } else
                    return false;
            }
            return false;
        }
    }
    KeyTracking hitLeftK = new KeyTracking(KeyCode.LeftShift),
        hitRightK = new KeyTracking(KeyCode.RightShift),
        sailsUpK = new KeyTracking(KeyCode.UpArrow, true),
        sailsDownK = new KeyTracking(KeyCode.DownArrow, true),
        steeringLeft = new KeyTracking(KeyCode.LeftArrow, true),
        steeringRight = new KeyTracking(KeyCode.RightArrow, true),
        adamBayamK = new KeyTracking(KeyCode.Space),
        plusK = new KeyTracking(KeyCode.Equals),
        minusK = new KeyTracking(KeyCode.Minus),
        keelUpK = new KeyTracking(KeyCode.PageUp),
        keelDownK = new KeyTracking(KeyCode.PageDown),
        zoomK = new KeyTracking(KeyCode.Z),
        followK = new KeyTracking(KeyCode.X),
        f11K = new KeyTracking(KeyCode.F11),
        exitK = new KeyTracking(KeyCode.C),
        toggleCanvasK = new KeyTracking(KeyCode.K),
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
        pauseImg = pausePlayB.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        playImg = pausePlayB.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        // zoomOutImg = zoomB.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        // zoomInImg = zoomB.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        followBoatImg = followB.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        followWorldImg = followB.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        canvas = GameObject.FindGameObjectWithTag("SailingControl").GetComponent<Canvas>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();

#if UNITY_EDITOR
       // hide the F11 button
       f11B.gameObject.SetActive(false);
#endif
        CanvasAutoScale();
    }
    bool CtrlIsDown() {
        return Input.GetKeyDown(KeyCode.LeftControl);
    }
    Vector2 screenSize;
    void CanvasAutoScale() {
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize == screenSize)  return;
        screenSize = currentScreenSize;
        Debug.Log("screen size: " + screenSize);
        float ss = (Screen.width + Screen.height) /2f;
        float cs = 1.3f * ss / 3000f;
        Debug.Log("Setting scale to: " + cs);
        canvasScaler.scaleFactor = cs;
    }
    void TestExitRequested()
    { // exits the game if CTRL-C is clicked.
        if (exitK.Clicked() && CtrlIsDown())
        {
            doExit();                   
        }
    }

    // Update is called once per frame
    GameObject inFocus;
    void Update()
    {   GameObject infocusNow = EventSystem.current.currentSelectedGameObject;
        if (inFocus != infocusNow) {
            // Debug.Log("focus changed: " + inFocus + " -> " + infocusNow);
            inFocus = infocusNow; 
        }
        EventSystem.current.SetSelectedGameObject(null);
        // CanvasAutoScale();
        // Debug.Log("escape: " + Input.GetKey(KeyCode.Escape) + ", return: " + Input.GetKey(KeyCode.Return));
        if (confirm().active && !confirm().done) return;
        TestExitRequested();
        if (hitLeftK.Clicked()) hitLeft.onClick.Invoke();
        if (hitRightK.Clicked()) hitRight.onClick.Invoke();
        if (adamBayamK.Clicked())
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
        if (plusK.Clicked()) doZoom(+10f);
        if (minusK.Clicked()) doZoom(-10f);
        if (sailsUpK.Clicked()) sails.value = Mathf.Max(sails.minValue, sails.value - 1);
        if (sailsDownK.Clicked()) sails.value = Mathf.Min(sails.maxValue, sails.value + 1);
        if (steeringLeft.Clicked()) steering.value = Mathf.Max(steering.minValue, steering.value - 1f/3);
        if (steeringRight.Clicked()) steering.value = Mathf.Min(steering.maxValue, steering.value + 1f/3);
        if (keelUpK.Clicked()) keel.value = Mathf.Max(keel.minValue, keel.value - 1);
        if (keelDownK.Clicked()) keel.value = Mathf.Min(keel.maxValue, keel.value + 1);
        // if (zoomK.clicked() && CtrlIsDown()) { doZoom(-10f);}
        // if (zoomK.clicked() && !CtrlIsDown()) { doZoom(10f);}
        if (followK.Clicked())  doFollow(); 
        if (togglePausePlayK.Clicked()) doPausePlay();
        if (restartK.Clicked()) doRestart();
        if (showHideHelpK.Clicked()) doHelp();
        if (escK.Clicked()) doEscape();
        if (enterK.Clicked()) doEnter();
        if (f11K.Clicked()) doF11();
        if (ancorK.Clicked()) boat().ToggleAnchor();
        if (toggleCanvasK.Clicked()) toggleCanvas();
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

    void toggleCanvas(bool forceOn = false) {
        bool on = forceOn ? true : !isCanvasOn();
        canvas.gameObject.SetActive(on);
        if (!on && isHelpOn()) doHelp(); // will not happen if invoked from doHelp().
    }
    private static Confirm confirm_;
    public Confirm confirm() {
        if (confirm_ == null) confirm_ = new Confirm(confirmP);
        return confirm_;
    }

    bool isConfirmOn() {return confirm().active;}
    bool isHelpOn() { return infoP.gameObject.activeSelf;}
    bool isCanvasOn() { return canvas.gameObject.activeSelf;}

    public void CanvasSizeControl(float factor) {
        canvasScaler.scaleFactor = factor;
    }
    // public void ZoomOut()
    // {
    //     // canvasScaler.scaleFactor += 0.1f;
    //     doZoom(10f);
    // }
    // public void ZoomIn()
    // {
    //     // Debug.Log(canvasTransform.localScale);
    //     // canvasScaler.scaleFactor -= 0.1f;
    //     doZoom(-10f);
    // }
    float rad(float deg) {return deg * Mathf.Deg2Rad; }

    public void doHelp() {
        infoP.GetComponent<HelpManager>().activate(!isHelpOn());
        toggleCanvas(true); // force it to be on
    }

    public void doFollow() {
        bool followBoat = !CameraController.Instance.withOrientation;
        CameraController.Instance.withOrientation = followBoat;
        followBoatImg.gameObject.SetActive(followBoat);
        followWorldImg.gameObject.SetActive(!followBoat);
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
    public void doF11() {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void doZoom(float change) {
        // bool close = Camera.main.orthographicSize == 10f;
        Debug.Log("camera size: " + Camera.main.orthographicSize);
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + change, 10f, 100f); //close ? 30f : 10f; 
        // close = !close;
        // zoomOutImg.gameObject.SetActive(close);
        // zoomInImg.gameObject.SetActive(!close);
    }
}
