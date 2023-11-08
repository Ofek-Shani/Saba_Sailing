using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindController : MonoBehaviour
{
    [SerializeField] public float windDirection = 0;
    public float windStrength = 10f;
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
    {   windStrength = Mathf.Clamp(windStrength, 1f, 15f);
        float boatDirection = boat.transform.rotation.eulerAngles.z;
        // shapeModule.position = boat.transform.position;
        float wdr = windDirection * Mathf.Deg2Rad;
        Vector2 windDirectionVector = new Vector3(Mathf.Cos(wdr), Mathf.Sin(wdr)) * windStrength;
        emissionModule.rateOverTime = windStrength*10f;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pt.particleCount];
        ParticleSystem.MainModule mainModule = pt.main;
        mainModule.maxParticles = (int)(windStrength / 15f * 750) + 250;
        mainModule.simulationSpeed = 5f + windStrength / 2f;
        pt.GetParticles(particles);
        float speed = 15 /*windStrength*/  / 2000f; // .5f;
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
