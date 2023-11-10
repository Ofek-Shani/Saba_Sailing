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
        transform.position = Vector3.Lerp(transform.position, toFollow.transform.position, .01f) + Vector3.back;
        if (withOrientation) {
            Quaternion tobe = toFollow.transform.rotation;
//            tobe.z = (tobe.z + 90f) % 360;
            transform.rotation = Quaternion.Lerp(transform.rotation, tobe, 0.1f);
        }
    }
}
