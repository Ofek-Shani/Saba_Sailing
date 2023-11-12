using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// (c) 2023 copyright Uri Shani, Ofek Shani

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public GameObject toFollow;
    public float speed;
    public bool withOrientation = true;
    private void Awake() { Instance = this; }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, toFollow.transform.position, 0.1f) + Vector3.back;
        Quaternion tobe = toFollow.transform.rotation;
        if (withOrientation) {
            tobe = Quaternion.Euler(0f, 0f, tobe.eulerAngles.z - 90f);
        } else {
            tobe = Quaternion.Euler(0f, 0f, 0f); //.eulerAngles.z = 0; 
        } 
        if (transform.rotation.eulerAngles.z != tobe.eulerAngles.z) 
            transform.rotation = tobe;
    }
}
