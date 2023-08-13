using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, Input.GetAxis("Vertical")));
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
        Vector2 boatForwardDirection = (Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Vector2.up).normalized;
        Vector2 boatSidewaysDirection = new(-boatForwardDirection.y, boatForwardDirection.x);

        // Wind Force on the Sails
        float mainSailZ = mainSail.transform.rotation.eulerAngles.z;

        float deltaAngle = (mainSailZ - wc.windDirection) % 360;
        Debug.Log("deltaAngle " + deltaAngle);
        float forceNormal = -1 * Mathf.Sin(deltaAngle * Mathf.Deg2Rad);

        Vector2 forceNormalVector = new Vector2(Mathf.Cos((mainSailZ + 90) * Mathf.Deg2Rad), Mathf.Sin((mainSailZ + 90) * Mathf.Deg2Rad)) * forceNormal;

        rb.AddForce(forceNormalVector *  Time.deltaTime * wc.windStrength);

        // our forces are only taking into account the wind here -- water drag is not accounted for yet!
        Vector2 forwardForceVector = Vector2.Dot(forceNormalVector, boatForwardDirection) * boatForwardDirection;
        Vector2 sidewaysForceVector = Vector2.Dot(forceNormalVector, boatSidewaysDirection) * boatSidewaysDirection;

        float forwardSpeed = forwardForceVector.magnitude;
        float sidewaysSpeed = sidewaysForceVector.magnitude;

        // Water Drag Force on the boat

        Vector2 boatDirectionVeclocity = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.back) * rb.velocity;
        
        float inverseSquareVelocityX = rb.velocity.x * rb.velocity.x * Mathf.Sign(rb.velocity.x) * -1;
        float inverseSquareVelocityY = rb.velocity.y * rb.velocity.y * Mathf.Sign(rb.velocity.y) * -1;
        Vector2 inverseSquareVelocity = new(inverseSquareVelocityX, inverseSquareVelocityY);

        rb.AddForce(inverseSquareVelocity);

        boatText.text = boatForwardDirection.ToString() + "\n" + boatForwardDirection.magnitude;
        
        // Water Force on Keel



    }



}
