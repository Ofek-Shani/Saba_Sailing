using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoy : MonoBehaviour
{
    [SerializeField] public float speed; // = 100f; // The speed at which the buoy swings from side to side
    [SerializeField] public float amplitude; // = 30f; // The distance of the swing
    [SerializeField] public bool both; // = false; if swinging in both x and y.
    [SerializeField] public GameObject theBoat;
    public bool throwable = false;
    public float throwForce = 0.25f;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (throwable)
        {
            gameObject.SetActive(false);
            transform.position = new Vector3(0f, 0f, transform.position.z);
        }
    }

    void Update()
    {
        Quaternion rotation = transform.rotation;
        if (!throwable) {
            float t = (Time.time * speed) * Mathf.Deg2Rad;
        // Debug.Log("t: " + t);
            float a = Mathf.Sin(t) * amplitude;

            // Debug.Log("Spin speed: " + spinSpeed);
            if (both) {
                rotation = Quaternion.Euler(a, a, rotation.eulerAngles.z);
            } else {
                rotation = Quaternion.Euler(a, rotation.eulerAngles.y, rotation.eulerAngles.z);
            }

            // Assign the new rotation to the object
            transform.rotation = rotation;
        }
    }

    public void ThrowBuoy() {
        Vector3 buoyDirection = theBoat.transform.rotation.eulerAngles;
        float var = Random.Range(-90, 90);
        buoyDirection.z += 180 + var; // inverse to boat direction with variations.
        float directionInRad = buoyDirection.z * Mathf.Deg2Rad;
        Vector3 throwFrom = theBoat.transform.position + new Vector3(3f * Mathf.Cos(directionInRad), 3f * Mathf.Sin(directionInRad), 0f); ;
        gameObject.SetActive(true);

        Vector3 dirV = Quaternion.Euler(buoyDirection) * Vector3.right;
        Vector2 dirV2 = new Vector2(dirV.x, dirV.y);
        transform.position = throwFrom;
        rb.AddTorque(1);  // rotations as it is throwm
        rb.AddForce(dirV2 * throwForce * Random.Range(0.8f,1.2f), ForceMode2D.Impulse);
        // Debug.Log("rotations:" + direction + ", var:" + var + ", dirV:" + dirV + ",dirV2:" + dirV2);
    }           
}

