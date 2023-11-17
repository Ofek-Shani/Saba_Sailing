using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
// (c) 2023 copyright Uri Shani, Ofek Shani


public class gestures : MonoBehaviour
{
    [SerializeField] Slider sails, steering, keel;
    public Button hitLeft, hitRight, adamBayamB, plus, minus;
    public GameObject adamBayamBuoy, pausePlayB, infoP, helpB, restartB, exitB;
    public GameObject confirmP; 
    // public Camera camera;
    bool pauseIsOn = true;
    Image pauseImg, playImg;
    class KeyTracking
    {
        KeyCode keyCode;
        bool stateUp = true;
        public KeyTracking(KeyCode keyCode_)
        {
            keyCode = keyCode_;
        }
        public bool clicked()
        {
            if (Input.GetKeyDown(keyCode))
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
        hK = new KeyTracking(KeyCode.H);

    bool isFullScreen = false;
    bool isInfoShown = false;

    Color normalHelpBColor, normalRestartBColor, normalQuitBColor, normalPausePlayBColor;
    // Start is called before the first frame update

    BoatController boat() { return BoatController.Instance; }
    AdamBayam adamBayam() { return AdamBayam.Instance; }
    void Start()
    {
        pauseImg = pausePlayB.transform.GetChild(0).GetComponent<Image>();
        playImg = pausePlayB.transform.GetChild(1).GetComponent<Image>();
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

    void TestToggleScreenSizeRequested()
    {
        if (f11K.clicked())
        {
            isFullScreen = !isFullScreen;
        }
        Screen.fullScreen = isFullScreen;
    }
    // Update is called once per frame
    void Update()
    {
        if (confirm().active && !confirm().done) return;
        TestToggleScreenSizeRequested();
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

    bool helpIsOn = false;
    public void doHelp() {
        helpIsOn = !helpIsOn;
        infoP.gameObject.SetActive(helpIsOn);
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
        confirm().Stop(true);
    }
    public void doConfirmCancel() {
        confirm().Stop(false);
    }

    public void doCloseHelp(){
        infoP.gameObject.SetActive(false);
    }
    public void doShowHelp(){
        infoP.gameObject.SetActive(!infoP.gameObject.activeSelf); // toggle its active state.
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
}
