using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{

    float windSpeed, windDirection, steeringAngle, sailAngle, keelPosition, boatAngle = 0;

    GameObject frontSail, mainSail;

    // Start is called before the first frame update
    void Start()
    {
        windSpeed = 10;
        windDirection = Mathf.PI / 2;
        mainSail = transform.GetChild(0).gameObject;
        frontSail = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        float horizInput = Input.GetAxis("Horizontal");
        sailAngle += horizInput * .1f;

        sailAngle = Mathf.Clamp(sailAngle, -150, -30);

        frontSail.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
        mainSail.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);

        Debug.Log(frontSail.transform.localRotation.z + " " + (frontSail.transform.localRotation.z < -Mathf.PI / 4));
        frontSail.GetComponent<SpriteRenderer>().flipY = frontSail.transform.localRotation.z < -Mathf.PI/4;
        mainSail.GetComponent<SpriteRenderer>().flipY = frontSail.transform.localRotation.z < -90;
    }
}
