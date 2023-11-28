using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// (c) 2023 copyright Uri Shani, Ofek Shani

public class Buoy : MonoBehaviour
{
    [SerializeField] public float speed; // = 100f; // The speed at which the buoy swings from side to side
    [SerializeField] public float amplitude; // = 30f; // The distance of the swing
    [SerializeField] public bool both; // = false; if swinging in both x and y.
    [SerializeField] public GameObject theBoat;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    Vector3 origLocalScale;
    static System.Random random = new System.Random(999);
    float randomDelay;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        origLocalScale = transform.localScale;
        randomDelay = (float)random.NextDouble() * Mathf.PI;
        Start_();
        //sr.PointerClicked += HandlePointerClicked;
    }

    protected virtual void Start_() { }
    protected virtual void Update_() {
        Quaternion rotation = transform.rotation;
        float t = (Time.time * speed) * Mathf.Deg2Rad + randomDelay;
        Vector3 localScale = origLocalScale;
        // Debug.Log("t: " + t);
        float a = Mathf.Sin(t) * amplitude;
        float b = Mathf.Cos(a * Mathf.Deg2Rad);
        // Debug.Log("Spin speed: " + spinSpeed);
        if (both)
        {
            rotation = Quaternion.Euler(a, a, rotation.eulerAngles.z); 
            localScale.x *= b;
            localScale.y *= b; 
        } else {
            rotation = Quaternion.Euler(a, rotation.eulerAngles.y, rotation.eulerAngles.z); 
            localScale.y *= b;
            //localScale.y *= b;
        }

        // Assign the new rotation to the object
        transform.rotation = rotation;
        transform.localScale = localScale;
    }
    void Update()
    {
        Update_();
    }

}

