using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class gestures : MonoBehaviour
{
    [SerializeField] Slider sails, steering, keel;
    public Button hitLeft, hitRight, adamBayam, plus, minus;
    public GameObject adamBayamBuoy;
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
        keelDownK = new KeyTracking(KeyCode.PageDown);
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
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

    }
}
