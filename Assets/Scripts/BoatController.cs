using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Data;
using UnityEditor.PackageManager.Requests;
using UnityEngine.Rendering;
using UnityEditor.Experimental.GraphView;
// (c) 2023 copyright Uri Shani, Ofek Shani


public class BoatController : MonoBehaviour
{

    public float rudderAngle = 0, sailAngle, frontSailTorque = 0, mainSailTorque= 0;

    // enum BoatPart { KEEL, RUDDER, FRONTSAIL, MAINSAIL };
    // BoatPart boatPart = BoatPart.KEEL;
    public enum KeelStatus { UP, HALFWAY, DOWN };
    public KeelStatus keelStatus = KeelStatus.DOWN;
    public Text sailSliderValue;
    public TMP_Text sailSliderTitle;
    public Slider sailSlider;
    public Slider keelSlider;
    public Slider steeringSlider;
    public Text keelSliderValue;
    public Text steeringSliderValue;
    public Button frotSailButton;
    public Button mainSailButton;
    public GameObject frontSailPanel;
    public GameObject mainSailPanel;
    public float demoVelocityMagnitude = 3f, demoSailForceMagnitude = 3f;
    private Image mainSailPanelImage, frontSailPanelImage;
    private Color mainSailPanelRestColor, frontSailPanelRestColor;
    private SailController.Animation hitSteeringAnimation = new SailController.Animation(0.1f, 1f);


    // Demo simulation state saving variables:
    private bool demoMode = false;
    private float demoVelocity = 0, saveWindDirection;

    [SerializeField] GameObject rudder, keel;
    SpriteRenderer rudderSpr, keelSpr;
    [SerializeField] Sprite[] keelSprites;
    [SerializeField] float boatForceFactor = 3f;
        

    WindController wc;
    Rigidbody2D rb;

    TMP_Text angDvalueText, linDvalueText, fwdFvalueText, latFvalueText;
    Toggle anchor;

    public enum DemoKind {STEERING=0, SAILS=1, KEEL=2}; 
    Slider[] sliderMap;
    // CONSTANTS FOR BOAT
    //[Range(0.0f, 10.0f)]
    //float WATER_DENSITY = 50, BOAT_MASS = 1;
    [SerializeField] float DRAG_FACTOR = 1.0f;
    float ANGULAR_DRAG = 0.5f;
    Dictionary<KeelStatus, float> LATERAL_DRAG_FACTOR = new Dictionary<KeelStatus, float>()
    { {KeelStatus.UP, 1f},
      {KeelStatus.HALFWAY, 2f},
      {KeelStatus.DOWN, 3f},
    };
    float WIND_BODY_FACTOR = .01f;

    SailController FrontSail, MainSail;

    static BoatController instance;

    struct State{
        public float sails, steering, keel;
        public Transform boat;
    };
    State state;

    public void setDemoMode(bool demo = false, bool forward = true) { 
        Renderer renderer = GetComponent<Renderer>(), 
            mainSailRenderer = MainSail.gameObject.GetComponent<Renderer>(),
            frontSailRenderer = FrontSail.gameObject.GetComponent<Renderer>();
        Color objectColor = renderer.material.color,
            mainSailColor = mainSailRenderer.material.color,
            frontSailColor = frontSailRenderer.material.color;
        float a = demo ? 0.5f : 1f; // Set the alpha value to 0.5 (50% transparency)
        objectColor.a = mainSailColor.a = frontSailColor.a = a;
        renderer.material.color = objectColor;
        mainSailRenderer.material.color = mainSailColor;
        frontSailRenderer.material.color = frontSailColor;
        demoMode = demo;
        demoVelocity = (forward ? 1 : -1) * demoVelocityMagnitude;
        if (demo) Reset();
        ToggleAnchor(demo ? 1 : -1);
    }
    public void ToggleAnchor(int force = 0) { // force can be -1 (for off), 0 (for toggle) or 1 (for on).
        if (anchor == null) return;
        if (force == 0) {
             anchor.isOn = !anchor.isOn;
        } else 
            anchor.isOn = force > 0;
    }
    public void Reset() {
        sailSlider.value = state.sails;
        steeringSlider.value = state.steering;
        keelSlider.value = state.keel;
        transform.position = state.boat.position;
        transform.rotation = state.boat.rotation;
        transform.localScale = state.boat.localScale;
        rb.velocity = Vector3.zero; // Set the velocity to zero
        rb.angularVelocity = 0; // Set the angular velocity 
        if (useMain) mainSailButtonToggle();
        if (useFront) frontSailButtonToggle();
    }
    void SaveStatus() {
        state.sails = sailSlider.value;
        state.steering = steeringSlider.value;
        state.keel = keelSlider.value;
        state.boat = new GameObject().transform;
        state.boat.position = transform.position;
        state.boat.rotation = transform.rotation;
        state.boat.localScale = transform.localScale;
    }
    public static BoatController Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        sliderMap  = new Slider[] {steeringSlider, sailSlider, keelSlider};
        MainSail = transform.GetChild(1).gameObject.GetComponent<SailController>();
        FrontSail = transform.GetChild(0).gameObject.GetComponent<SailController>(); 
        wc = GameObject.Find("Wind").GetComponent<WindController>();
        rb = GetComponent<Rigidbody2D>();
        rb.angularDrag = ANGULAR_DRAG;
        rb.drag = DRAG_FACTOR;
        rudder = transform.GetChild(3).gameObject;
        keel = transform.GetChild(2).gameObject;
        rudderSpr = rudder.GetComponent<SpriteRenderer>();

        keelSpr = keel.GetComponent<SpriteRenderer>();
        GameObject[] boatTexts = GameObject.FindGameObjectsWithTag("BoatInfo"); //.GetComponent<TMP_Text>();
        angDvalueText = boatTexts[0].GetComponent<TMP_Text>();
        linDvalueText = boatTexts[1].GetComponent<TMP_Text>();
        fwdFvalueText = boatTexts[2].GetComponent<TMP_Text>();
        latFvalueText = boatTexts[3].GetComponent<TMP_Text>();
        anchor = GameObject.FindGameObjectWithTag("anchor").GetComponent<Toggle>();
        sailAngle = MainSail.transform.localEulerAngles.z;
        mainSailPanelImage = mainSailPanel.GetComponent<Image>();
        frontSailPanelImage = frontSailPanel.GetComponent<Image>();
        mainSailPanelRestColor = mainSailPanelImage.color;
        frontSailPanelRestColor = frontSailPanelImage.color;
        SetKeel(keelSlider);
        SaveStatus();
}

// Update is called once per frame
class KeyControl
    {
        private KeyCode kc;
        private bool down = false;
        public KeyControl(KeyCode kc_) { kc = kc_; }
        public bool clicked() { 
            if (Input.GetKey(kc))
            {
                if (down) return false;
                down = true;
                return true;
            } else
            {
                down = false;
                return false;
            }
        }
    }

    bool useMain = false, useFront = false;
    public void mainSailClicked() { mainSailButtonToggle(); }
    void mainSailButtonToggle(bool toggleOther = true)
    {
        useMain = !useMain;
        mainSailPanelImage.color = useMain ? Color.green : mainSailPanelRestColor; //  new Color(1f, 0.5f, 0.5f, 1f); // pink
        if (useFront && toggleOther) frontSailButtonToggle(false); // turn it off;
    }
    public void frontSailClicked() { frontSailButtonToggle(); }
    void frontSailButtonToggle (bool toggleOther = true) {
        useFront = !useFront;
        frontSailPanelImage.color = useFront ? Color.green : frontSailPanelRestColor; // new Color(1f, 0.5f, 0.5f, 1f); // pink
        if (useMain && toggleOther) mainSailButtonToggle(false); // turn it off;
    }
    bool simulateTension = false;
    SliderDemo sliderDemo = null;

    KeyControl useMainK = new KeyControl(KeyCode.S), useFrontK = new KeyControl(KeyCode.W);
    void Update()
    {
        if (useMainK.clicked()) mainSailButtonToggle();
        if (useFrontK.clicked()) frontSailButtonToggle();
        bool setAsMain = useMain, setAsFront = useFront;
        if (!setAsMain && !setAsFront) setAsMain = setAsFront = true;
        simulateTension = true; // if sailSlider changes below, ignore the event calls to SetSailTension below.
        if (setAsMain && setAsFront) { 
            sailSliderTitle.text = "SAILS"; 
            sailSlider.value = Mathf.Clamp((MainSail.SailTension + FrontSail.SailTension) / 2f, 
                sailSlider.minValue, sailSlider.maxValue); 
        }
        else if (setAsMain) { sailSliderTitle.text = "MAIN"; sailSlider.value = MainSail.SailTension; }
        else if (setAsFront) { sailSliderTitle.text = "FRONT"; sailSlider.value = FrontSail.SailTension; }
        simulateTension = false; // stop ignoring.
        if (sliderDemo == null || !sliderDemo.Update()) {
            if (!hitSteeringAnimation.done)
                SetSteering(hitSteeringAnimation.Value);
        } 
        DoPhysics();
    }

    float sailTorque(SailController.sailPhysics sailPhysics) {
        float torqueSize = sign(sailPhysics.sailAngle) *
            (Mathf.Abs(sailPhysics.sailPos * 
                Mathf.Sin((270 + sailPhysics.sailAngle) * Mathf.Deg2Rad)) - sailPhysics.sailWidth / 2f); 
            //abs(f + r * Math.sin(theta)) / Math.sqrt(cotTheta ** 2 + 1);
        return -sailPhysics.sailForce * torqueSize * BoatPhysics.ST;        
    }
    public void SetFrontSail() {
        if (!useFront) frontSailButtonToggle();
    }
    public void SetMainSail() {
        if (!useMain) mainSailButtonToggle();
    }
    public void SetBothSails() {
        if (useFront) frontSailButtonToggle();
        if (useMain) mainSailButtonToggle();
   }
    // private Slider sliderMap(DemoKind kind) 
    // {
    //     switch(kind) { 
    //         case DemoKind.STEERING: return steeringSlider;
    //         case DemoKind.SAILS: return sailSlider;
    //         case DemoKind.KEEL: return keelSlider;
    //     }
    //     return null;    
    // }
    public void startSliderDemo(DemoKind kind, int cycles = 3, 
        float _factor = 1f, float _velocity = 3f) {
        Slider slider = sliderMap[(int)kind];
        float startPos = 0, endPos= 0, factor= _factor;
        demoVelocity = _velocity;
        switch (kind) { 
            case DemoKind.SAILS: endPos=10f; break; 
            case DemoKind.KEEL: endPos=2f; break; }
//            case DemoKind.STEERING: factor= 0.5f; break;}
        sliderDemo = new SliderDemo(this, slider, startPos, endPos, cycles, factor);
    }
    public void stopSliderDemo() {
        sliderDemo = null;    
        Reset();
    }
    class SliderDemo 
    {    
        SailController.Animation animator;
        Slider slider;
        int cycles = 0, cycle = 0;
        float startPos, endPos, factor;
        BoatController boat;
        public SliderDemo(BoatController _boat, Slider _slider, float _startPos=0, float _endPos=0, int _cycles=3, float _factor=1f) {
            animator = new SailController.Animation(0.05f, 1.5f);
            cycles = _cycles;
            cycle = 0;
            boat = _boat;
            slider = _slider;
            startPos = _startPos;
            endPos = _endPos;
            factor = _factor;
        } 
        float rest = 1f;
        bool resting = false;
        float restTime = 0; 
        public void Reset() {
            slider.value = startPos;
            animator.stop();
        }
        bool starting = true, finish = true;
        public bool Update() {
            if (animator == null) return false;
            if (resting) {
                restTime += Time.deltaTime;
                if (restTime > rest) resting = false;
                return true;
            }
            if (animator.done) {
                cycle++;
                if (cycle > 1 && cycle <= cycles) {
                    resting = true;
                    restTime = 0;
                }
                if (cycle > cycles && !finish) {
                    animator = null;
                    return false;
                } else {
                    float from = factor * slider.minValue, to = factor * slider.maxValue;
                    if (cycle > cycles && finish) {
                        finish = false;
                        from = endPos * factor;
                    } else if(starting) {
                        starting = false;
                        from = startPos * factor;
                    } 
                    if (cycle % 2 == 0) {
                        (to, from) = (from, to);
                    }
                    animator.setBoth(from, to);
                }
            }
            slider.value = animator.Value;
            // boat.SetSteering(animator.Value);
            return true;
        }
    }
    public void SetKeel(Slider sl)
    {
        keelStatus = (KeelStatus)sl.value;
        keelSliderValue.text = keelStatus.ToString();
        keelSpr.sprite = keelSprites[(int)keelStatus];
        // Debug.Log(keelStatus);
    }

    float sliderValue = 0;
    public void SetSailTension(Slider sl)
    {
        if (simulateTension) { sliderValue = sl.value; return; }
        if (sliderValue == sl.value) return;
        bool main = useMain; // shift - only main
        bool front = useFront; // control - only front
        if (!main && !front) main = front = true;
        float diff = sl.value - sliderValue;
        if (main) MainSail.SailTension = Mathf.Clamp(MainSail.SailTension + diff, 0f, 10f);
        if (front) FrontSail.SailTension = Mathf.Clamp(FrontSail.SailTension + diff, 0f, 10f);
        if (main && front) sliderValue = (MainSail.SailTension + FrontSail.SailTension) / 2f;
        else sliderValue = sl.value;
        sl.value = sliderValue;
        sailSliderValue.text = sliderValue.ToString();
    }


    public void HitSteeringLeft()
    {
        float steeringState = steeringSlider.value;
        SetSteering(-8f);
        hitSteeringAnimation.setBoth(-8f, 0);
        float torque = Mathf.Abs(-8f - steeringState) * 1.5f;
        rb.AddTorque(torque);
    }
    public void HitSteeringRight()
    {
        float steeringState = steeringSlider.value;
        SetSteering(8f);
        hitSteeringAnimation.setBoth(8f, 0);
        float torque = -Mathf.Abs(8f - steeringState) * 1.5f;
        rb.AddTorque(torque);
    }

    void SetSteering(float value)
    {   value = Mathf.Round(value * 3f)/3f; // 24 steps in each direction
        rudderAngle = value * 10; // value is -8 to +8 meaning -80 deg, to +80 deg.
        // Debug.Log("rudderAngle: " + rudderAngle);
        steeringSliderValue.text = rudderAngle.ToString("F0");
        rudder.transform.localRotation = Quaternion.Euler(0, 0, rudderAngle);
        steeringSlider.value = value;
    }
    public void SetSteering(Slider sl)
    {
        SetSteering(sl.value);
    }

    private float windBoatAngleN = 0;
    public float WindBoatAngleN
    {
        get { return windBoatAngleN; }
    }

    float normalized(float deg) { return BoatPhysics.normalized360(deg); }
    float bNormalized(float deg) {
        return BoatPhysics.normalized360(deg) - 180; //> 180 ? 180 - deg : deg; 
    }

    // relative direction where the wind blows, coming from the inverse side, with 0 along the boat keel to the rudder, positive on the right, negative on the left.
    public float getWindBoatAngleN()
    {
        return bNormalized(wc.windDirection - transform.rotation.eulerAngles.z);
    }
    public float sign(float v) { return v >= 0 ? 1f : -1f; }
    void DoPhysics()
    {
        float boatDirection = transform.rotation.eulerAngles.z;
        float windDirection = wc.windDirection;
        if (demoMode) wc.windDirection = boatDirection * (demoVelocity > 0 ? 1 : -1);
        Vector2 boatForwardDirection = new Vector2(Mathf.Cos(rad(boatDirection)), Mathf.Sin(rad(boatDirection))) ; //(Quaternion.AngleAxis(boatDirection, Vector3.forward) * Vector2.up).normalized;
        Vector2 boatLateralDirection = new Vector2(boatForwardDirection.y, -boatForwardDirection.x);
        windBoatAngleN = getWindBoatAngleN();  //- boatDirection); normalized so wind blows to the right is > 0, and wind to the left is < 0
        // boatText.text = "Boat:" + boatDirection.ToShortString() + ", wind:" + windDirection.ToShortString() + ", wind-boat:" + windBoatAngleN.ToShortString() + ", " + MainSail.ToShortString();
        Vector2 mainForces = MainSail.GetForcesVector();
        Vector2 frontForces = FrontSail.GetForcesVector();

        // our forces are only taking into account the wind here
        Vector2 forwardForceVector = boatForwardDirection * (mainForces[0] + frontForces[0]);
        Vector2 lateralForceVector = boatLateralDirection * (mainForces[1] + frontForces[1]);
        float forwardVelocity = Vector2.Dot(rb.velocity, boatForwardDirection);
        float lateralVelocity = Vector2.Dot(rb.velocity, boatLateralDirection);
        if (demoMode) {
            forwardVelocity = demoVelocity;
            lateralVelocity = 0f;
        }
        rudderSpr.color = forwardVelocity > 0 ? Color.green : Color.red;
        // apply force and torque:
        //Vector2 forceVector = (forwardForceVector * GetForwardDragFactor(boatForwardDirection) + lateralForceVector * GetLateralDragFactor(keelStatus, lateralVelocity)) * Time.deltaTime * boatForceFactor;
        float ldf = GetLateralDragFactor(keelStatus, lateralVelocity);
        // Debug.Log("ldf:" + ldf);
        Vector2 forceVector = (forwardForceVector + lateralForceVector * ldf) * Time.deltaTime * boatForceFactor;
        if ((anchor != null && !anchor.isOn) || anchor == null) 
            rb.AddForce(forceVector);
        float rudderAngleN = rudder.transform.localEulerAngles.z;
        if (rudderAngleN > 180) rudderAngleN -= 360;
        float torque = 0;
        if (Mathf.Abs(rudderAngleN) > 5f) 
            torque = - sign(rudderAngleN) * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(rad(rudderAngleN)))) * forwardVelocity * Mathf.Cos(rad(rudderAngleN)) * Time.deltaTime * BoatPhysics.RT;
        frontSailTorque = sailTorque(FrontSail.GetPhysics()) * Time.deltaTime * BoatPhysics.ST;
        mainSailTorque = sailTorque(MainSail.GetPhysics()) * Time.deltaTime * BoatPhysics.ST;
        rb.AddTorque(torque + frontSailTorque + mainSailTorque);
        float angularDrag = (Mathf.Clamp(Mathf.Cos(rad(rudderAngleN)*2),0,1f)*3f + LATERAL_DRAG_FACTOR[keelStatus]/3f) * 0.15f;
        rb.angularDrag = angularDrag;
        float drag = (1f + LATERAL_DRAG_FACTOR[keelStatus] / 10f + Mathf.Abs(Mathf.Sin(rad(rudderAngleN))) * 0.5f) * 0.15f ;
        rb.drag = drag;
        if (angDvalueText != null)
        {
            angDvalueText.text = angularDrag.ToShortString();
            linDvalueText.text = drag.ToShortString();
            fwdFvalueText.text = forwardForceVector.magnitude.ToShortString();
            latFvalueText.text = lateralForceVector.magnitude.ToShortString();
        }

    }

    float rad(float angle) { return angle * Mathf.Deg2Rad;}

    void ApplySteeringForce()
    {
        //Debug.Log((-1 * rudder.transform.up).ToShortString() + " <- forward, right -> " + rudder.transform.right.ToShortString());
        Vector2 rudderDirection = rudder.transform.up * -1;
 
    }

    float GetLateralDragFactor(KeelStatus keelStatus, float lateralVelocity)
    {
        return Mathf.Max(0f, (1 + LATERAL_DRAG_FACTOR[KeelStatus.DOWN] - LATERAL_DRAG_FACTOR[keelStatus]) / LATERAL_DRAG_FACTOR[KeelStatus.DOWN]) * 0.25f; 
            // (1 + Mathf.Cos(rad(rudder.transform.localEulerAngles.z)) / 3) * lateralVelocity * lateralVelocity);
    }

    float GetForwardDragFactor(Vector2 forwardDirection)
    {
        float forwardVelocity = Vector2.Dot(rb.velocity, forwardDirection);
        return Mathf.Max(1f); // 0f, 1 - DRAG_FACTOR * ( 1 + 
            // Mathf.Abs(Mathf.Sin(rad(rudder.transform.localEulerAngles.z)))/ 3) * forwardVelocity * forwardVelocity);
    }

    Vector2 GetCombinedSailForces()
    {
        Vector2 sailForces = MainSail.GetForcesVector() + FrontSail.GetForcesVector();
        Vector2 boatForce = Quaternion.Euler(0, 0, wc.windDirection) * Vector2.right * wc.windStrength * WIND_BODY_FACTOR;
        return sailForces + boatForce;
    }
}

public class BoatPhysics
{
    // Constants
    public const float KA = 0.5f;// acceleration factor
    public const float KN = 10; // Constant for normal sail power
    public const float KT = 20; // Constant for tangential sail power
    public const float KW = 1; // Constant for angular change rate calculation
    public const float M = 10; // Boat mass in kg
    public const float L = 5; // Boat length in meters
    public const float KB = 0.5f; // factor for wind effect on the boat body
    public const float ST = 1.0f; // factor for sail torque effect on the boat.
    public const float RT = 7.5f; // factor for rudder torque effect on the boat.
    public readonly float[] R = { -1.0f, 0, 0.5f }; // Water resistance constant [backward, still, forward]
    public const float AD = 0.5f; // Angular drag to slow rotational torque.

    float boatSpeed, boatDrift;
    // utils: 
    public static float normalized360(float deg) { return ((deg % 360) + 360) % 360; } // ensure it is between 0 and 360.

    public static float Condition(float v, float min, float max)
    {
        return v >= min && v <= max ? 1 : 0;
    }
}
