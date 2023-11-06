using UnityEngine;
using Unity.VisualScripting;

public class SailController : MonoBehaviour
{
    [SerializeField] float area;
    public float sailAngle, sailAngleN, force;
    private float sailTension = 0, windBoatAngleN = 0;
    SpriteRenderer sailSpr;
    WindController wc;
    BoatController bc;
    Animation SailShape;


    void Start()
    {
        sailAngle = transform.localRotation.eulerAngles.z;
        sailAngleN = normalize(sailAngle);
        sailSpr = GetComponent<SpriteRenderer>();
        wc = GameObject.FindGameObjectWithTag("Wind").GetComponent<WindController>();
        bc = GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatController>();
        SailShape = new Animation(0.1f, 1.25f); // one second period, minimum change = 0.1
    }

    public string ToShortString()
    {
        return "sailAngle:" + sailAngle.ToShortString() + ", sailAngleN:" + sailAngleN.ToShortString() + ", forceN:" + forceN.ToShortString();
    }
    public SailController()
    {
    }
    public float normalize(float sailAngle) { return sailAngle - 180; }// angle is > 0 on right, and < 0 on left, like windN angle relative to boat.
    public float denormalize(float sailAngleN) { return BoatPhysics.normalized360(sailAngleN + 180); }// angle is > 0 on right, and < 0 on left, like windN angle relative to boat.
    public float SailTension
    {
        set { sailTension = value; }
        get { return sailTension; }
    }
    float changeFactor() { return Time.deltaTime * 10f; }
    // Update is called once per frame
    float elapsed = 0;
   void Update() 
    {
        elapsed += Time.deltaTime;
        // if (elapsed < 1f) return;
        float cf = elapsed * 10f; //  changeFactor() ;
        elapsed = 0;
        windBoatAngleN = bc.WindBoatAngleN;
        sailAngle = transform.localRotation.eulerAngles.z; // if changed from inspector - consider this.
        sailAngleN = normalize(sailAngle);
        float windSailDiff = Mathf.Abs(windBoatAngleN) - Mathf.Abs(sailAngleN); // positive when wind pushes sail, negative when it blows it backwards

        float changeO = Mathf.Min(10, 1 * Mathf.Clamp(wc.windStrength, 3f, 15f) * cf); // change rate when working with the wind
        float changeC = Mathf.Min(10, 1 * Mathf.Clamp(15 - wc.windStrength, 3f, 15f) * cf); // change rate when working against the wind
        float change = 0;
        // consider sail tension here:
        float pos = 70.0f * (sailTension / 10.0f);
        float targetSailAngleN = (80f - pos) * sign(sailAngleN);
        string trace = "";
        if (sign(windBoatAngleN) != sign(sailAngleN) && Mathf.Abs(windBoatAngleN) < 160) 
        { // MAHAPACH
            trace += "M";
            change = -changeO;
            force = -1f;
        }
        else
        {
            trace += "N";
            float maxChange = Mathf.Abs(targetSailAngleN - sailAngleN);
            // float windBoatInverseAngleN = windBoatAngleN; // -sign(windBoatAngleN) * (180 - Mathf.Abs(windBoatAngleN));
            void regular()
            {
                force = Mathf.Sin(windSailDiff * Mathf.Deg2Rad);
                change = sign(Mathf.Abs(targetSailAngleN) - Mathf.Abs(sailAngleN)) * Mathf.Min(maxChange, Mathf.Abs(targetSailAngleN) > Mathf.Abs(sailAngleN) ? changeO : changeC);
            }
            if (sign(windBoatAngleN) == sign(sailAngleN)) // wind blows to sail side (from different sides)
            {
                if (Mathf.Abs(windSailDiff) <= 10) // sail is loose
                {
                    trace += "A";
                    force = sign(windSailDiff) * 0.1f;
                    change = sign(windSailDiff) * Mathf.Min(Mathf.Abs(windSailDiff) + 10, changeO); // positive means decrease abs of angle.
                }
                else if (windSailDiff < -10) // wind blows above the sail - pushong it inward.
                {
                    trace += "B";
                    force = Mathf.Sin(windSailDiff * Mathf.Deg2Rad);
                    change = -Mathf.Min(maxChange + 10, changeO);
                }
                else  // wind pushes the sail by blowing it up. windSailDiff > 10.
                {
                    trace += "C";
                    regular();
                }
            } else // wind and sail on same side.
            {
                trace += "D";
                regular();
            }

        }
        sailAngleN += sign(sailAngleN) * change;
        sailAngleN = minAbs(sailAngleN, targetSailAngleN);
        forceN = force * wc.windStrength / 15f;
        // Debug.LogFormat("trace:{5}, change:{0}, sailAngleN:{1}, changeO:{2}, changeC:{3}, cf:{4}, winSailDiff:{6}", change, sailAngleN, changeO, changeC, cf, trace, windSailDiff) ;
        sailAngle = denormalize(sailAngleN); // convert it back to game relative coordinates
        transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
        // ApplyWind();
        CalculateSailShape();
    }

    public static float sign(float v) { return v > 0 ? 1 : -1; }

    public float minAbs(float a, float b) {
        if (sign(a) != sign(b)) return a;
        return sign(a) * Mathf.Min(Mathf.Abs(a), Mathf.Abs(b)); 
    }
    private float forceN = 0;
    /* public void ApplyWind()
    {
        float windSailAngleN = Mathf.Abs(windBoatAngleN - sailAngleN);
        float force = wc.windStrength * area / 100f;
        bool isGood = windSailAngleN > 30 && windSailAngleN < 150;
        forceN = -(isGood ? Mathf.Sin(windSailAngleN) : -Mathf.Abs(Mathf.Sin(windSailAngleN))) * force;
    }
    */
    public class Animation
    {
        float from, to;
        float current;
        public bool done = true;
        float totalTime = 0;
        float threshold;
        float period = 0.5f;
        public Animation(float threshold_, float period_) { threshold = threshold_; period = period_; }
        public void setBoth(float from_, float to_) { from = from_; to = to_;  done = false; totalTime = 0; current = from; }
        public void set(float to_)
        {
            if (!done) return;
            from = to;
            to = to_;
            current = from;
            if (Mathf.Abs(to - from) < threshold)
            { // nothing to animate.
                current = to;
                done = true;
            }
            else
            {
                done = false;
                current = from;
                totalTime = 0;
            }
        }
        public float Value {  
            get {
                if (!done)
                {
                    totalTime += Time.deltaTime;
                    done = totalTime >= period ;
                }
                if (!done) current = from + totalTime * (to - from) / period;
                if (Mathf.Abs(current) < threshold) current = SailController.sign(current) * threshold;
                // Debug.Log("from:" + from + ", to:" + to + ", current:" + current.ToShortString() + ", done:" + done + ", time:" + totalTime.ToShortString());
                return current;
            }
        }
    }


    public void CalculateSailShape()
    {
        const float minScale = 0.45f;
        float scaleY = sign(sailAngleN) * sign(forceN) * Mathf.Min(1f, Mathf.Max(minScale, Mathf.Abs(forceN)));
        SailShape.set(scaleY);
        scaleY = SailShape.Value;
        transform.localScale = new Vector3(transform.localScale.x, scaleY, 1);
        sailSpr.color = Mathf.Abs(forceN) < 0.25 ? Color.white : (forceN < 0 ? Color.red : Color.green);
    }

    /*
    public Vector2 GetSailForces()
    {
        float deltaAngle = normalize(sailAngleN - windBoatAngleN);
        //Debug.Log("deltaAngle " + deltaAngle);
        float forceNormal = -1 * Mathf.Sin(deltaAngle * Mathf.Deg2Rad);

        Vector2 forceNormalVector = new Vector2(Mathf.Cos((sailAngleN + 90) * Mathf.Deg2Rad), Mathf.Sin((sailAngleN + 90) * Mathf.Deg2Rad)) * forceNormal;

        return forceNormalVector * area;
        // rb.AddForce(forceNormalVector * Time.deltaTime * wc.windStrength);
    }
    */


    public Vector2 GetForcesVector()
    {
        float angle = (90 - Mathf.Abs(sailAngleN)) * Mathf.Deg2Rad;
        Vector2 temp = new Vector2(force * Mathf.Cos(angle), force * Mathf.Sin(angle));
        return temp;
    }

}
