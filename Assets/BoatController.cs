using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{

    float windSpeed, windDirection, steeringAngle, sailAngle, keelPosition, boatAngle = 0;
    
    GameObject frontSail, mainSail;

    WindController wc;
    // Start is called before the first frame update
    void Start()
    {
        windSpeed = 10;
        windDirection = Mathf.PI / 2;
        mainSail = transform.GetChild(0).gameObject;
        frontSail = transform.GetChild(1).gameObject;

        wc = GameObject.FindGameObjectWithTag("Wind").GetComponent<WindController>();
    }

    // Update is called once per frame
    void Update()
    {
        ControlSailPositions();
        
    }

    void CalculateSpeed()
    {

    }

    void CalculateSailShape()
    {
        float frontSailZ = frontSail.transform.rotation.eulerAngles.z;
        float mainSailZ = mainSail.transform.rotation.eulerAngles.z;
        float[] GetForces(float sailDir)
        {
            float deltaAngle = (sailDir - wc.windDirection + 360) % 360;
            float forceTangent = Mathf.Cos(deltaAngle * Mathf.Deg2Rad);
            float forceNormal = -1 * Mathf.Sin(deltaAngle * Mathf.Deg2Rad);

            return new float[] { deltaAngle, forceTangent, forceNormal };
        }

        float[] mainForces = GetForces(mainSailZ);
        float[] frontForces = GetForces(frontSailZ);

        frontSail.transform.localScale = new Vector3(frontSail.transform.localScale.x, frontForces[2], 0);
        mainSail.transform.localScale = new Vector3(mainSail.transform.localScale.x, mainForces[2], 0);
    }


    void ControlSailPositions()
    {
        float horizInput = Input.GetAxis("Horizontal");
        sailAngle += horizInput * .1f;

        sailAngle = Mathf.Clamp(sailAngle, -150, -30);

        frontSail.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
        mainSail.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);

        CalculateSailShape();
    }
}
