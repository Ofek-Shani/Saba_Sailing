using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoy : MonoBehaviour
{
    [SerializeField] public float speed; // = 100f; // The speed at which the boat moves up and down
    [SerializeField] public float amplitude; // = 30f; // The distance between the top and bottom of the water
    [SerializeField] public bool both; // = false;
    private Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        rotation = transform.rotation;
    }

    void Update()
    {
        // Calculate the current position of the boat based on time
        float t = (Time.time * speed) * Mathf.Deg2Rad;
        // Debug.Log("t: " + t);
        float a = Mathf.Sin(t) * amplitude;


        if (both) {
            rotation = Quaternion.Euler(a, a, rotation.eulerAngles.z);
        } else {
            rotation = Quaternion.Euler(a, rotation.eulerAngles.y, rotation.eulerAngles.z);
        }

        // Assign the new rotation to the object
        transform.rotation = rotation;
    }
}

