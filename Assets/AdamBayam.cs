using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdamBayam : MonoBehaviour
{
    [SerializeField] int spinningSpeed = 2;
    float startSpin = 0;
    bool spinning = false;
    [SerializeField] GameObject theAdamBayam;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        theAdamBayam.transform.eulerAngles += new Vector3(0, 0, spinningSpeed) * Time.deltaTime;
    }

    public void Throw()
    {
        spinning = true;
    }
}
