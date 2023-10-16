using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoy : MonoBehaviour
{
    [SerializeField] public float speed; // = 100f; // The speed at which the boat moves up and down
    [SerializeField] public float amplitude; // = 30f; // The distance between the top and bottom of the water
    [SerializeField] public bool both; // = false;
    [SerializeField] public GameObject theBoat;
    public float spinSpeed = 1;
    public bool throwable = false;
    private bool startSpeen = false;
    public float throwForce = 1000f;
    public int throwDistance = 40;
    public int throwVariant = 10;
    private Quaternion rotation;
    private Vector3 targetThrow, baseThrow;
    float travel, startTime;
    public float flyTime = 2f;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rotation = transform.rotation;
        if (throwable)
        {
            gameObject.SetActive(false);
            transform.position = new Vector3(0f, 0f, transform.position.z);
        }
    }

    void Update()
    {
        // Calculate the current position of the boat based on time
        float t = (Time.time * speed) * Mathf.Deg2Rad;
        // Debug.Log("t: " + t);
        float a = Mathf.Sin(t) * amplitude;
        float b = rotation.eulerAngles.z;

        // handle flight
        if (startTime != 0 && travel < 1)
        {
            travel = Mathf.Min(1f, (Time.time - startTime) / flyTime);
            float portion = 1 - (1 - travel) * (1 - travel); // start changing fast, then slow.
            Vector3 pos = baseThrow * (1-portion) + targetThrow * portion;
            transform.position = pos;
            if (travel >= 1f) { startTime = 0; travel = 0; startSpeen = false; }
            b += Time.deltaTime * spinSpeed * (0.5f - portion * 0.5f);
        } else if (!throwable) {
            // Debug.Log("Spin speed: " + spinSpeed);
            if (both) {
                rotation = Quaternion.Euler(a, a, b);
            } else {
                rotation = Quaternion.Euler(a, rotation.eulerAngles.y, b);
            }

            // Assign the new rotation to the object
            transform.rotation = rotation;
        }
    }

    public void ThrowBuoy() {
        startSpeen = true;
        float distance = throwDistance - throwVariant / 2 + Random.value * throwVariant;
        Vector3 direction = theBoat.transform.rotation.eulerAngles;
        float var = Random.Range(-90, 90);
        direction.z += 180 + var;
        float directionInRad = direction.z * Mathf.Deg2Rad;
        baseThrow = theBoat.transform.position + new Vector3(3f * Mathf.Cos(directionInRad), 3f * Mathf.Sin(directionInRad), 0f); ;
        travel = 0;
        // startTime = Time.time;
        targetThrow = theBoat.transform.position + new Vector3(distance * Mathf.Cos(directionInRad), distance * Mathf.Sin(directionInRad), 0f);
        gameObject.SetActive(true);

        Vector3 dirV = Quaternion.Euler(direction) * Vector3.right;
        Vector2 dirV2 = new Vector2(dirV.x, dirV.y);
        transform.position = baseThrow;
        rb.AddTorque(1);
        rb.AddForce(dirV2 * throwForce * Random.Range(0.8f,1.2f), ForceMode2D.Impulse);
        Debug.Log("rotations:" + direction + ", var:" + var + ", dirV:" + dirV + ",dirV2:" + dirV2);
    }           
}

