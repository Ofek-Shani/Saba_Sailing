using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindController : MonoBehaviour
{
    [SerializeField] float displacement = 3;
    public float windDirection = 0;
    public int windStrength = 10;
    ParticleSystem pt;
    ParticleSystem.EmissionModule emissionModule;
    // ParticleSystem.ShapeModule shapeModule;
    GameObject boat;

    public static WindController instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pt = GetComponent<ParticleSystem>();
        emissionModule = pt.emission;
        boat = transform.parent.gameObject;
        // shapeModule = pt.shape;
    }

    // Update is called once per frame
    void Update()
    {
        float boatDirection = boat.transform.rotation.eulerAngles.z;
        // shapeModule.position = boat.transform.position;
        float wdr = boatDirection * Mathf.Deg2Rad; // windDirection * Mathf.Deg2Rad;
        Vector2 windDirectionVector = new Vector2(Mathf.Cos(wdr), Mathf.Sin(wdr)) * windStrength;
        emissionModule.rateOverTime = windStrength/2;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pt.particleCount];
        
        pt.GetParticles(particles);
        float speed = windStrength * 0.001f * 0.1f; // .5f;
        Vector2 velocity = windDirectionVector * speed;
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].velocity = velocity;
        }
        // Debug.Log("boat: " + boatDirection.ToShortString() + ", wdr " + (wdr*Mathf.Rad2Deg).ToShortString()); //Debug.Log(windStrength);
        pt.SetParticles(particles, pt.particleCount); // pt.particleCount * 100);

      //  gameObject.transform.localPosition = new Vector3(Mathf.Cos(wdr), Mathf.Sin(wdr), 0) * -1 * displacement;
    }
}
