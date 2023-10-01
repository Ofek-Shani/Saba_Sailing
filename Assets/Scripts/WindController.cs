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
    }

    // Update is called once per frame
    void Update()
    {
        float wdr = windDirection * Mathf.Deg2Rad;
        Vector2 windDirectionVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * windDirection), Mathf.Sin(Mathf.Deg2Rad * windDirection)) * windStrength;
        emissionModule.rateOverTime = windStrength/2;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pt.particleCount];
        
        pt.GetParticles(particles);
        float speed = windStrength * 0.001f * 0.1f; // .5f;
        Vector2 velocity = windDirectionVector * speed;
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].velocity = velocity;
        }
        Debug.Log(speed); //Debug.Log(windStrength);
        pt.SetParticles(particles, pt.particleCount); // pt.particleCount * 100);

        gameObject.transform.localPosition = new Vector3(Mathf.Cos(wdr), Mathf.Sin(wdr), 1) * -1 * displacement;
    }
}
