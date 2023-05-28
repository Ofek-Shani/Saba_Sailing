using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindController : MonoBehaviour
{
    [SerializeField] float displacement = 3;
    public float windDirection = 45;
    public int windStrength = 10;
    ParticleSystem pt;

    // Start is called before the first frame update
    void Start()
    {
        pt = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        float wdr = windDirection * Mathf.Deg2Rad;
        Vector2 windDirectionVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * windDirection), Mathf.Sin(Mathf.Deg2Rad * windDirection)) * windStrength;

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pt.particleCount];

        pt.GetParticles(particles);

        for(int i = 0; i < particles.Length; i++)
        {
            particles[i].velocity = windDirectionVector * windStrength;
        }

        pt.SetParticles(particles, pt.particleCount);

        gameObject.transform.localPosition = new Vector3(Mathf.Cos(wdr), Mathf.Sin(wdr), -3) * -1 * displacement;
    }
}
