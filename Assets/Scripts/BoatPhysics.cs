using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoatPhysics : MonoBehaviour
{
    // Constants
    const float KA = 0.5f;// acceleration factor
    const float KN = 10; // Constant for normal sail power
    const float KT = 20; // Constant for tangential sail power
    const float KW = 1; // Constant for angular change rate calculation
    const float M = 10; // Boat mass in kg
    const float L = 5; // Boat length in meters
    const float KB = 0.5f; // factorfor wind effect on the boat body
    readonly float[] R = { -1.0f, 0, 0.5f }; // Water resistance constant [backward, still, forward]

    float boatSpeed, boatDrift;

    BoatController bc;

    private void Start()
    {
        bc = BoatController.instance;
    }

    float Condition(float v, float min, float max)
    {
        return v >= min && v <= max ? 1 : 0;
    }
    void UpdateBoat()
    {
        float DT = Time.deltaTime;
        // boatDirection = (boatDirection + boatIncrement)% (2*Math.PI);
        // Compute angle between wind and boat direction
        float windAngle = bc.windDirection - transform.rotation.z;
        if (windAngle > Mathf.PI) windAngle = windAngle - 2 * Mathf.PI; // wind to boat angle is positive on the right sie, negative on the left side.
        float windSailAngle = Mathf.Sign(bc.sailAngle) * (bc.sailAngle - windAngle); // positive on the left sideof sail, negative on the right (back) side
        if (windSailAngle > Mathf.PI) windSailAngle = windSailAngle - 2 * Mathf.PI;
        // Compute sail and rudder forces
        float sailForce = Mathf.Sign(windSailAngle) * bc.windSpeed * (
            KT * Mathf.Abs(Mathf.Cos(windSailAngle)) * Condition(windSailAngle, 120, 160) +
            KN * Mathf.Abs(Mathf.Sin(windSailAngle))
        );
        float fwdSailForce = sailForce * Mathf.Abs(Mathf.Sin(bc.sailAngle));
        float driftSailForce = sailForce * Mathf.Abs(Mathf.Cos(bc.sailAngle));
        float fwdForce = fwdSailForce + bc.windSpeed * Mathf.Cos(windAngle) * KB;
        float driftForce = (driftSailForce + Mathf.Sign(-windAngle) * bc.windSpeed * Mathf.Abs(Mathf.Sin(windAngle)) * KB) * 
            new float[]  { 0.1f, 0.05f, 0.01f} [(int)bc.keelPosition];
        float waterResistance = -R[(int)Mathf.Sign(boatSpeed) + 1] * boatSpeed * boatSpeed;

        float steeringStep = (Mathf.PI / 4 / 5); // 5 steps in steering from straight to 45 degrees. 
                                                // none linear steering factor. Slows down if above 45 degrees.
        float steeringFactor = new float[]  { 0, 0.1f, 0.3f, 0.7f, 1.25f, 0.8f, 0.5f, 0.1f, 0 }[(int)Mathf.Min(8, Mathf.Round(Mathf.Abs(bc.rudderAngle) / steeringStep))];
        float rudderForce = -KW * Mathf.Abs(Mathf.Sin(bc.rudderAngle)) * boatSpeed * steeringFactor;
        float rudderTorque = -rudderForce * Mathf.Sign(bc.rudderAngle); // * Math.cos(steeringAngle);

        float newBoatAngle = (transform.rotation.z*Mathf.Deg2Rad - rudderTorque / M * DT) % (2 * Mathf.PI);
        // Compute boat acceleration
        float fwdAcceleration = (fwdForce + rudderForce + waterResistance) / M;
        // acceleration = Math.abs(acceleration) < 1 ? 0 : acceleration;

        // Update boat speed and direction
        boatSpeed += KA * fwdAcceleration * DT;
        boatDrift = driftForce / M;

        float bpX = transform.position.x + boatSpeed * Mathf.Cos(transform.rotation.z * Mathf.Deg2Rad) * DT + boatDrift * Mathf.Sin(transform.rotation.z) * DT;
        float bpY = transform.position.y + boatSpeed * Mathf.Sin(transform.rotation.z * Mathf.Deg2Rad) * DT + boatDrift * Mathf.Cos(transform.rotation.z) * DT;

        transform.position = new Vector3(bpX, bpY, 0);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, newBoatAngle * Mathf.Rad2Deg));

        //boatDirection += boatSpeed * Math.sin(windAngle) / L * DT;
    }
}   
