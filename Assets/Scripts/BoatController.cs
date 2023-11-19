using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
// (c) 2023 copyright Uri Shani, Ofek Shani


public class BoatController : MonoBehaviour
{

    public float rudderAngle = 0, sailAngle;

    // enum BoatPart { KEEL, RUDDER, FRONTSAIL, MAINSAIL };
    // BoatPart boatPart = BoatPart.KEEL;
    public enum KeelStatus { UP, HALFWAY, DOWN };
    public KeelStatus keelStatus = KeelStatus.UP;
    public Text sailSliderValue;
    public TMP_Text sailSliderTitle;
    public Slider sailSlider;
    public Slider keelSlider;
    public Text keelSliderValue;
    public Text steeringSliderValue;
    public Slider steeringSlider;
    public Button frotSailButton;
    public Button mainSailButton;
    public GameObject frontSailPanel;
    public GameObject mainSailPanel;
    private Image mainSailPanelImage, frontSailPanelImage;
    private Color mainSailPanelRestColor, frontSailPanelRestColor;
    private SailController.Animation steeringAnimation = new SailController.Animation(0.1f, 1f);

    [SerializeField] GameObject rudder, keel;
    SpriteRenderer rudderSpr, keelSpr;
    [SerializeField] Sprite[] keelSprites;
    [SerializeField] float boatForceFactor = 3f;
        

    WindController wc;
    Rigidbody2D rb;

    TMP_Text angDvalueText, linDvalueText, fwdFvalueText, latFvalueText;
    Toggle ancor;

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

    public void ToggleAncor() {
        if (ancor != null) ancor.isOn = !ancor.isOn;
    }
    public void Reset() {
        sailSlider.value = state.sails;
        steeringSlider.value = state.steering;
        keelSlider.value = state.keel;
        transform.position = state.boat.position;
        transform.rotation = state.boat.rotation;
        transform.localScale = state.boat.localScale;
        if (useMain) mainSailFlipped();
        if (useFront) frontSailFlipped();
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
        SaveStatus();
        MainSail = transform.GetChild(0).gameObject.GetComponent<SailController>();
        FrontSail = transform.GetChild(1).gameObject.GetComponent<SailController>(); 
        wc = GameObject.FindGameObjectWithTag("Wind").GetComponent<WindController>();
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
        ancor = GameObject.FindGameObjectWithTag("ancor").GetComponent<Toggle>();
        sailAngle = MainSail.transform.localEulerAngles.z;
        mainSailPanelImage = mainSailPanel.GetComponent<Image>();
        frontSailPanelImage = frontSailPanel.GetComponent<Image>();
        mainSailPanelRestColor = mainSailPanelImage.color;
        frontSailPanelRestColor = frontSailPanelImage.color;

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
    public void mainSailClicked() { mainSailFlipped(); }
    void mainSailFlipped(bool flipOther = true)
    {
        useMain = !useMain;
        mainSailPanelImage.color = useMain ? Color.green : mainSailPanelRestColor; //  new Color(1f, 0.5f, 0.5f, 1f); // pink
        if (useFront && flipOther) frontSailFlipped(false); // turn it off;
    }
    public void frontSailClicked() { frontSailFlipped(); }
    void frontSailFlipped (bool flipOther = true) {
        useFront = !useFront;
        frontSailPanelImage.color = useFront ? Color.green : frontSailPanelRestColor; // new Color(1f, 0.5f, 0.5f, 1f); // pink
        if (useMain && flipOther) mainSailFlipped(false); // turn it off;
    }
    bool simulateTension = false;
    KeyControl useMainK = new KeyControl(KeyCode.S), useFrontK = new KeyControl(KeyCode.W);
    void Update()
    {
        if (useMainK.clicked()) mainSailFlipped();
        if (useFrontK.clicked()) frontSailFlipped();
        bool setAsMain = useMain, setAsFront = useFront;
        if (!setAsMain && !setAsFront) setAsMain = setAsFront = true;
        simulateTension = true; // if sailSlider changes below, ignore the event calls to SetSailTension below.
        if (setAsMain && setAsFront) { sailSliderTitle.text = "SAILS"; sailSlider.value = Mathf.Clamp((MainSail.SailTension + FrontSail.SailTension) / 2f, 0f, 10f); }
        else if (setAsMain) { sailSliderTitle.text = "MAIN"; sailSlider.value = MainSail.SailTension; }
        else if (setAsFront) { sailSliderTitle.text = "FRONT"; sailSlider.value = FrontSail.SailTension; }
        simulateTension = false; // stop ignoring.
        if (!steeringAnimation.done)
        {
            SetSteering(steeringAnimation.Value);
        }
        DoPhysics();
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
        steeringAnimation.setBoth(-8f, 0);
        float torque = Mathf.Abs(-8f - steeringState) * 1.5f;
        rb.AddTorque(torque);
    }
    public void HitSteeringRight()
    {
        float steeringState = steeringSlider.value;
        SetSteering(8f);
        steeringAnimation.setBoth(8f, 0);
        float torque = -Mathf.Abs(8f - steeringState) * 1.5f;
        rb.AddTorque(torque);
    }

    void SetSteering(float value)
    {
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
        rudderSpr.color = forwardVelocity > 0 ? Color.green : Color.red;
        // apply force and torque:
        //Vector2 forceVector = (forwardForceVector * GetForwardDragFactor(boatForwardDirection) + lateralForceVector * GetLateralDragFactor(keelStatus, lateralVelocity)) * Time.deltaTime * boatForceFactor;
        float ldf = GetLateralDragFactor(keelStatus, lateralVelocity);
        // Debug.Log("ldf:" + ldf);
        Vector2 forceVector = (forwardForceVector + lateralForceVector * ldf) * Time.deltaTime * boatForceFactor;
        if ((ancor != null && !ancor.isOn) || ancor == null) 
            rb.AddForce(forceVector);
        float rudderAngleN = rudder.transform.localEulerAngles.z;
        if (rudderAngleN > 180) rudderAngleN -= 360;
        float torque = 0;
        if (Mathf.Abs(rudderAngleN) > 5f) 
            torque = - sign(rudderAngleN) * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(rad(rudderAngleN)))) * forwardVelocity * Mathf.Cos(rad(rudderAngleN)) * Time.deltaTime * 10f;
        rb.AddTorque(torque);
        float angularDrag = (Mathf.Clamp(Mathf.Cos(rad(rudderAngleN)*2),0,1f)*3f + LATERAL_DRAG_FACTOR[keelStatus]/3f) * 0.15f;
        rb.angularDrag = angularDrag;
        float drag = (1f + LATERAL_DRAG_FACTOR[keelStatus] / 10f + Mathf.Abs(Mathf.Sin(rad(rudderAngleN))) * 0.5f) * 0.15f ;
        rb.drag = drag;
        // Water Drag Force on the boat

        //ApplySteeringForce();
        // CalculateSailShape(mainForce, frontForce);
        // Boat rotation and Torque (steering)

        /*
         * boatText.text = "F: " + forwardForceVector.magnitude.ToShortString() + 
            "/" + lateralForceVector.magnitude.ToShortString() + 
            ", V: " + forwardVelocity.ToShortString() + 
            "/" + lateralVelocity.ToShortString() +
            ", tQ: " + torque.ToShortString() + ", aDrag: " + angularDrag.ToShortString() + ", drag: " + drag.ToShortString(); //" / " + forwardForceVector.ToShortString() + " - " + lateralForceVector.ToShortString() + ". Velocity: " + rb.velocity.ToShortString() + " - " + rb.angularVelocity.ToShortString();
        */
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
    public const float KB = 0.5f; // factorfor wind effect on the boat body
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
