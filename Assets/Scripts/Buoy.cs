using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoy : MonoBehaviour
{
    [SerializeField] public float speed; // = 100f; // The speed at which the buoy swings from side to side
    [SerializeField] public float amplitude; // = 30f; // The distance of the swing
    [SerializeField] public bool both; // = false; if swinging in both x and y.
    [SerializeField] public GameObject theBoat;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Start_();
        //sr.PointerClicked += HandlePointerClicked;
    }

    protected virtual void Start_() { }
    protected virtual void Update_() {
        Quaternion rotation = transform.rotation;
        float t = (Time.time * speed) * Mathf.Deg2Rad;
        // Debug.Log("t: " + t);
        float a = Mathf.Sin(t) * amplitude;

        // Debug.Log("Spin speed: " + spinSpeed);
        if (both)
        {
            rotation = Quaternion.Euler(a, a, rotation.eulerAngles.z);
        }
        else
        {
            rotation = Quaternion.Euler(a, rotation.eulerAngles.y, rotation.eulerAngles.z);
        }

        // Assign the new rotation to the object
        transform.rotation = rotation;
    }
    void Update()
    {
        Update_();
    }

}

