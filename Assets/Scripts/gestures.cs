using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// (c) 2023 copyright Uri Shani, Ofek Shani


public class gestures : MonoBehaviour
{
    [SerializeField] Slider sails, steering, keel;
    public Button hitLeft, hitRight, adamBayam, plus, minus;
    public GameObject adamBayamBuoy;
    // public Camera camera;
    bool adamBayamIsOn = false;
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
        exitK = new KeyTracking(KeyCode.C);

    bool isFullScreen = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    void TestExitRequested()
    { // exits the game if CTRL-C is clicked.
        if (exitK.clicked() && (
            Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.RightControl)))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            Debug.Log("Exit requested");
        }
    }

    void TestToggleScreenSizeRequested()
    {
        if (f11K.clicked())
        {
            isFullScreen = !isFullScreen;
            Debug.Log("isFullScreen: " + isFullScreen);
        }
        Screen.fullScreen = isFullScreen;
    }
    // Update is called once per frame
    void Update()
    {
        TestToggleScreenSizeRequested();
        TestExitRequested();
        if (hitLeftK.clicked()) hitLeft.onClick.Invoke();
        if (hitRightK.clicked()) hitRight.onClick.Invoke();
        if (adamBayamK.clicked())
        {
            if (adamBayamIsOn)
            {
                adamBayamIsOn = false;
                adamBayamBuoy.SendMessage("OnMouseDown");
            }
            else
            {
                adamBayam.onClick.Invoke();
                adamBayamIsOn = true;
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

    }
}
