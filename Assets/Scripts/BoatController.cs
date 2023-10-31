using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;



public class BoatController : MonoBehaviour
{

    public float rudderAngle = 0, sailAngle;

    // enum BoatPart { KEEL, RUDDER, FRONTSAIL, MAINSAIL };
    // BoatPart boatPart = BoatPart.KEEL;
    public enum KeelStatus { UP, HALFWAY, DOWN };
    public KeelStatus keelStatus = KeelStatus.UP;
    public Text sailSliderValue;
    public Text keelSliderValue;
    public Text steeringSliderValue;
    private CanvasScaler canvasScaler;
    private bool controlBothSails = true;

    [SerializeField] GameObject rudder, keel;
    SpriteRenderer rudderSpr, keelSpr;
    [SerializeField] Sprite[] keelSprites;
    [SerializeField] float boatForceFactor = 1f;
        

    WindController wc;
    Rigidbody2D rb;

    TMP_Text boatText;

    // CONSTANTS FOR BOAT
    //[Range(0.0f, 10.0f)]
    //float WATER_DENSITY = 50, BOAT_MASS = 1;
    [SerializeField] float DRAG_FACTOR = 0.3f;
    Dictionary<KeelStatus, float> LATERAL_DRAG_FACTOR = new Dictionary<KeelStatus, float>()
    { {KeelStatus.UP, 1f},
      {KeelStatus.HALFWAY, 2f},
      {KeelStatus.DOWN, 3f},
    };
    float WIND_BODY_FACTOR = .01f;
    const float ANGULAR_DRAG = 0.5f;

    SailController FrontSail, MainSail;

    public static BoatController instance;

    public static BoatController Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
    }

    float elapsedTime = 0;
    void Start()
    {
        MainSail = transform.GetChild(0).gameObject.GetComponent<SailController>();
        FrontSail = transform.GetChild(1).gameObject.GetComponent<SailController>(); 
        wc = GameObject.FindGameObjectWithTag("Wind").GetComponent<WindController>();
        Canvas canvas = GameObject.FindGameObjectWithTag("SailingControl").GetComponent<Canvas>();
        canvasScaler = canvas.GetComponent<CanvasScaler>();

        rb = GetComponent<Rigidbody2D>();
        rb.angularDrag = ANGULAR_DRAG;

        keelSpr = keel.GetComponent<SpriteRenderer>();
        boatText = GameObject.FindGameObjectWithTag("BoatInfo").GetComponent<TMP_Text>();
        sailAngle = MainSail.transform.localEulerAngles.z;
    }

    // Update is called once per frame
    float lastTime = 0;
    void Update()
    {
       /* bool boatMode = true; // Time.realtimeSinceStartup < 36f * 5f;
        elapsedTime += Time.deltaTime;
        if (elapsedTime - lastTime > 3) {
           if (boatMode) 
                transform.localRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 15);
           else
           {
                FrontSail.transform.localRotation = Quaternion.Euler(0, 0, FrontSail.transform.rotation.eulerAngles.z + 10);
                MainSail.transform.localRotation = Quaternion.Euler(0, 0, MainSail.transform.rotation.eulerAngles.z + 10);
           }
           lastTime = elapsedTime;
        }*/
        DoPhysics();
    }
      
    public void SetKeel(Slider sl)
    {
        keelStatus = (KeelStatus)sl.value;
        keelSliderValue.text = keelStatus.ToString();
        keelSpr.sprite = keelSprites[(int)keelStatus];
        // Debug.Log(keelStatus);
    }

    public void SetSailTension(Slider sl)
    {
        MainSail.SetTension(sl.value);
        if (controlBothSails)
            FrontSail.SetTension(sl.value);
        sailSliderValue.text = sl.value.ToString();
    }


    public void SetSteering(Slider sl)
    {
        rudderAngle = sl.value * 10; // value is -8 to +8 meaning -80 deg, to +80 deg.
        Debug.Log("rudderAngle: " + rudderAngle);
        steeringSliderValue.text = rudderAngle.ToString("F0");
        rudder.transform.localRotation = Quaternion.Euler(0, 0, rudderAngle);
    }

    public float windBoatAngleN = 0;


    public void ZoomInControls()
    {
        canvasScaler.scaleFactor += 0.1f;
    }
    public void ZoomOutControls()
    {
        // Debug.Log(canvasTransform.localScale);
        canvasScaler.scaleFactor -= 0.1f;
    }
    float rad(float deg) {return deg * Mathf.Deg2Rad; }
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
        boatText.text = "Boat:" + boatDirection.ToShortString() + ", wind:" + windDirection.ToShortString() + ", wind-boat:" + windBoatAngleN.ToShortString() + ", " + MainSail.ToShortString();
        Vector2 mainForces = MainSail.GetForcesVector();
        Vector2 frontForces = FrontSail.GetForcesVector();

        // our forces are only taking into account the wind here
        Vector2 forwardForceVector = boatForwardDirection * (mainForces[0] + frontForces[0]);
        Vector2 lateralForceVector = boatLateralDirection * (mainForces[0] + frontForces[0]);
        float forwardVelocity = Vector2.Dot(rb.velocity, boatForwardDirection);
        float lateralVelocity = Vector2.Dot(rb.velocity, boatLateralDirection);
        rb.AddForce((forwardForceVector * GetForwardDragFactor(boatForwardDirection) + lateralForceVector * GetLateralDragFactor(keelStatus, lateralVelocity)) * Time.deltaTime * boatForceFactor);
        float rudderAngleN = rudder.transform.localEulerAngles.z - 180;
        rb.AddTorque(-sign(rudderAngleN) * Mathf.Cos(rad(90 - Mathf.Abs(rudderAngleN))) * forwardVelocity * Mathf.Cos(rad(rudderAngleN)) * Time.deltaTime * 5f);
        rb.angularDrag = (Mathf.Cos(rad(rudderAngleN) + LATERAL_DRAG_FACTOR[keelStatus]/3f) * 5f);
        // Water Drag Force on the boat

        //ApplySteeringForce();
        // CalculateSailShape(mainForce, frontForce);
        // Boat rotation and Torque (steering)

        //boatText.text = rb.velocity.ToShortString() + " - " + rb.angularVelocity.ToShortString();
      
    }

    void ApplySteeringForce()
    {
        //Debug.Log((-1 * rudder.transform.up).ToShortString() + " <- forward, right -> " + rudder.transform.right.ToShortString());
        Vector2 rudderDirection = rudder.transform.up * -1;
 
    }

    float GetLateralDragFactor(KeelStatus keelStatus, float lateralVelocity)
    {
        return Mathf.Max(0f, 1 - LATERAL_DRAG_FACTOR[keelStatus] * DRAG_FACTOR * (1 + 
            Mathf.Cos(rad(rudder.transform.localEulerAngles.z)) / 3) * lateralVelocity);
    }

    float GetForwardDragFactor(Vector2 forwardDirection)
    {
        float forwardVelocity = Vector2.Dot(rb.velocity, forwardDirection);
        return Mathf.Max(0f, 1 - DRAG_FACTOR * ( 1 + 
            Mathf.Abs(Mathf.Sin(rad(rudder.transform.localEulerAngles.z)))/ 3) * forwardVelocity);
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
