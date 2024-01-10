using System;
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
    public Color flagColor = Color.black;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    Vector3 origLocalScale;
    static System.Random random = new System.Random(999);
    Transform ft;
    SpriteRenderer fr;
    float fs, fsc;
    float buoySize;
    float randomDelay;
    // Start is called before the first frame update
    void Start()
    {
        ft = transform.Find("Circle"); // flag transform
        // Debug.Log(name + ": circle: " + ft);
        Vector3 size = GetComponent<Renderer>().bounds.size;
        buoySize = (size.x + size.y) / 2;
        if (ft != null) {
            fr = ft.GetComponent<SpriteRenderer>(); // flag renderer
            fs = fr.bounds.size.y/2f; // flad radius in y
            fsc = ft.localScale.y;    // flag orig scale in y
        }
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        origLocalScale = transform.localScale;
        randomDelay = (float)random.NextDouble() * Mathf.PI;
        Start_();
        //sr.PointerClicked += HandlePointerClicked;
    }

    protected virtual void Start_() { }
    protected virtual void Update_() {
        if (fr != null) fr.color = flagColor;
        Quaternion rotation = transform.rotation;
        float t = (Time.time * speed) * Mathf.Deg2Rad + randomDelay;
        Vector3 localScale = origLocalScale;
        // Debug.Log("t: " + t);
        float f = Mathf.Sin(t), 
            a = f * amplitude,
            b = Mathf.Cos(a * Mathf.Deg2Rad);
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
        if (ft != null) {
            // Debug.Log("color of [" + transform.name + "]: " + flagColor.ToString());
            if (flagColor.r != 0 || flagColor.g != 0 || flagColor.b != 0) {
                ft.gameObject.SetActive(true);
                float d = f * buoySize/6f;
                ft.localPosition = new Vector3(0, d, 0);
                //ft.localScale = new Vector3(1f,Math.Max(fsc, Math.Abs(d)/fs/2),1f);
            } else {
                ft.gameObject.SetActive(false);
            }
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

