using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BoatController : MonoBehaviour
{

    public float rudderAngle, sailAngle, keelPosition;

    enum BoatPart { KEEL, RUDDER, FRONTSAIL, MAINSAIL };
    BoatPart boatPart = BoatPart.KEEL;
    public enum KeelStatus { UP, HALFWAY, DOWN };
    public KeelStatus keelStatus = KeelStatus.UP;

    bool controlBothSails = true;

    [SerializeField] GameObject frontSail, mainSail, rudder, keel;
    SpriteRenderer frontSailSpr, mainSailSpr, rudderSpr, keelSpr;
    [SerializeField] Sprite[] keelSprites;
        

    WindController wc;
    Rigidbody2D rb;

    TMP_Text boatText;

    // CONSTANTS FOR BOAT
    [Range(0.0f, 10.0f)]
    float WATER_DENSITY = 50, BOAT_MASS = 1;
    float FORWARD_DRAG_FACTOR = 3;
    Dictionary<KeelStatus, float> LATERAL_DRAG_FACTOR = new Dictionary<KeelStatus, float>()
    { {KeelStatus.UP, 10},
      {KeelStatus.HALFWAY, 50},
      {KeelStatus.DOWN, 150},};
    float WIND_BODY_FACTOR = .01f;

    public static BoatController instance;

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
        //mainSail = transform.GetChild(0).gameObject;
        //frontSail = transform.GetChild(1).gameObject;

        wc = GameObject.FindGameObjectWithTag("Wind").GetComponent<WindController>();

        rb = GetComponent<Rigidbody2D>();

        keelSpr = keel.GetComponent<SpriteRenderer>();
        boatText = GameObject.FindGameObjectWithTag("BoatInfo").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        GetControls();
        CalculateSailShape();
        DoPhysics();
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, Input.GetAxis("Vertical")));
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if(e.type.Equals(EventType.KeyUp))
        {
            switch(e.keyCode)
            {
                case KeyCode.Alpha1: boatPart = BoatPart.KEEL; Debug.Log("Keel Control"); break;
                case KeyCode.Alpha2: boatPart = BoatPart.RUDDER; Debug.Log("Rudder Control"); break;
                case KeyCode.Alpha3: boatPart = BoatPart.FRONTSAIL; Debug.Log("Front Sail Control"); break;
                case KeyCode.Alpha4: boatPart = BoatPart.MAINSAIL; Debug.Log("Main Sail Control"); break;
                default: break;
            }
        }
    }

    void GetControls()
    {
        switch(boatPart)
        {
            case BoatPart.KEEL: ControlKeel(); break;
            case BoatPart.RUDDER: ControlRudder(); break;
            case BoatPart.FRONTSAIL: ControlSails(); break;
            case BoatPart.MAINSAIL: ControlSails(); break;
            default: break;
        }
    }

    void CalculateSpeed()
    {
        
    }

    void ControlKeel()
    {
        // KeelStatus 0 is fully up and not in water, 2 is down and fully in water
        //Debug.Log("Controlling Keel...");
        if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W)) // move keel up
        {
            if(keelStatus != KeelStatus.UP ) keelStatus--;
            keelSpr.sprite = keelSprites[(int)keelStatus];
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)) // move keel down
        {
            if (keelStatus != KeelStatus.DOWN) keelStatus++;
            keelSpr.sprite = keelSprites[(int)keelStatus];
        }
    }

    void ControlRudder()
    {
        //Debug.Log("Controlling Rudder...");
        float horizInput = Input.GetAxis("Horizontal");
        rudderAngle += horizInput * .1f;

        rudderAngle = Mathf.Clamp(rudderAngle, -80, 80);

        rudder.transform.localRotation = Quaternion.Euler(0, 0, rudderAngle);
    }

    void ControlSails()
    {
        //Debug.Log("Controlling Sails...");
        float horizInput = Input.GetAxis("Horizontal");
        sailAngle += horizInput * .1f;

        sailAngle = Mathf.Clamp(sailAngle, -150, -30);

        frontSail.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
        mainSail.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);

        CalculateSailShape();
    }

    float[] GetForceMagnitudes(float sailDir)
    {
        float deltaAngle = (sailDir - wc.windDirection + 360) % 360;
        float forceTangent = Mathf.Cos(deltaAngle * Mathf.Deg2Rad);
        float forceNormal = -1 * Mathf.Sin(deltaAngle * Mathf.Deg2Rad);

        return new float[] { deltaAngle, forceTangent, forceNormal };
    }

    Vector2 GetForceVectors(float forceMag, float angle, bool isTangent)
    {
        Vector2 temp = new Vector2(forceMag * Mathf.Cos(angle), forceMag * Mathf.Sin(angle));
        return temp;
    }

    void CalculateSailShape()
    {
        float frontSailZ = frontSail.transform.rotation.eulerAngles.z;
        float mainSailZ = mainSail.transform.rotation.eulerAngles.z;
        
        float[] mainForces = GetForceMagnitudes(mainSailZ);
        float[] frontForces = GetForceMagnitudes(frontSailZ);

        frontSail.transform.localScale = new Vector3(frontSail.transform.localScale.x, frontForces[2], 0);
        mainSail.transform.localScale = new Vector3(mainSail.transform.localScale.x, mainForces[2], 0);
    }

    float SailTangentCondition(float v, float min, float max)
    {
        return v >= min && v <= max ? 1 : 0;
    }

    void DoPhysics()
    {
        // Wind Force on the Sails
        float mainSailZ = mainSail.transform.rotation.eulerAngles.z;
        float frontSailZ = frontSail.transform.rotation.eulerAngles.z;

        Vector2 boatForwardDirection = (Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Vector2.up).normalized;
        Vector2 boatLateralDirection = new(-boatForwardDirection.y, boatForwardDirection.x);

        Vector2 combinedSailForces = GetCombinedSailForces(mainSailZ, frontSailZ, 10, 5);

        // our forces are only taking into account the wind here -- water drag is not accounted for yet!
        Vector2 forwardForceVector = Vector2.Dot(combinedSailForces, boatForwardDirection) * boatForwardDirection;
        Vector2 lateralForceVector = Vector2.Dot(combinedSailForces, boatLateralDirection) * boatLateralDirection;

        rb.AddForce((forwardForceVector + lateralForceVector) * Time.deltaTime * 10);

        // Water Drag Force on the boat

        float forwardVelocity = Vector2.Dot(rb.velocity, boatForwardDirection);
        float lateralVelocity = Vector2.Dot(rb.velocity, boatLateralDirection);

        Vector2 boatDirectionVeclocity = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.back) * rb.velocity;
        
        float inverseSquareVelocityForward = Mathf.Pow(forwardVelocity, 2) * Mathf.Sign(forwardVelocity) * -1 * FORWARD_DRAG_FACTOR;
        float inverseSquareVelocityLateral = Mathf.Pow(lateralVelocity, 2) * Mathf.Sign(lateralVelocity) * -1 * LATERAL_DRAG_FACTOR[keelStatus];
        //Vector2 inverseSquareVelocity = new(inverseSquareVelocityForward, inverseSquareVelocityLateral);

        Vector2 forwardDragForce = boatForwardDirection * inverseSquareVelocityForward;
        Vector2 lateralDragForce = boatLateralDirection * inverseSquareVelocityLateral;

        rb.AddForce((forwardDragForce + lateralDragForce) * Time.deltaTime * 10);

        boatText.text = rb.velocity.ToShortString();
        
        // Water Force on Keel
    }

    /// <summary>
    /// Calculates forces acting on a sail of a given angle (assumes tangent force is negligable even though it might not be)
    /// </summary>
    /// <param name="sailZ"></param>
    /// <param name="sailFactor"></param>
    /// <returns></returns>
    Vector2 GetSailForces(float sailZ, float sailFactor)
    {
        float deltaAngle = (sailZ - wc.windDirection) % 360;
        Debug.Log("deltaAngle " + deltaAngle);
        float forceNormal = -1 * Mathf.Sin(deltaAngle * Mathf.Deg2Rad);

        Vector2 forceNormalVector = new Vector2(Mathf.Cos((sailZ + 90) * Mathf.Deg2Rad), Mathf.Sin((sailZ + 90) * Mathf.Deg2Rad)) * forceNormal;

        return forceNormalVector * sailFactor;
        // rb.AddForce(forceNormalVector * Time.deltaTime * wc.windStrength);
    }

    Vector2 GetCombinedSailForces(float mainSailZ, float frontSailZ, float mainSailFactor, float frontSailFactor)
    {
        Vector2 sailForces = GetSailForces(mainSailZ, mainSailFactor) + GetSailForces(frontSailZ, frontSailFactor);
        Vector2 boatForce = Quaternion.Euler(0, 0, wc.windDirection) * Vector2.right * wc.windStrength * WIND_BODY_FACTOR;
        return sailForces + boatForce;
    }



}
