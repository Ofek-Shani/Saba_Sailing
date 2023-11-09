using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamBayam : Buoy
{
    [SerializeField] int spinningSpeed = 2;

    public float throwForce = 0.25f;
    float startSpin = 0;
    bool spinning = false;
    // Start is called before the first frame update

    protected override void Start_()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero; //  new Vector3(0f, 0f, transform.position.z);
    }

    float animateTime = -1f;
    Vector3 location;
    // Update is called once per frame
    protected override void Update_() {
        if (animateTime == -1f) return;
        Vector3 boatPosition = theBoat.transform.position;
        animateTime += Time.deltaTime;
        if (animateTime > 1f)
        {
            animateTime = -1f; // stops the animation
            gameObject.SetActive(false);
            return;
        }
        transform.localPosition = location * (1-animateTime) + boatPosition * animateTime;
    }

    void OnMouseDown() // starts an animation of pulling it up.
    {
        animateTime = 0;
        location = transform.position;
    }
    public void ThrowBuoy()
    {
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
        rb.AddForce(dirV2 * throwForce * Random.Range(0.8f, 1.2f), ForceMode2D.Impulse);
        // Debug.Log("rotations:" + direction + ", var:" + var + ", dirV:" + dirV + ",dirV2:" + dirV2);
    }

}
