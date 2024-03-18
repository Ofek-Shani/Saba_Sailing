using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
// (c) 2023 copyright Uri Shani, Ofek Shani

public class WindController : MonoBehaviour
{
    [Range(0,360)] public float windDirection = 0;
    [Range(0,30)] public float windStrength = 10f;
    ParticleSystem pt;
    ParticleSystem.Particle[] particles;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.ShapeModule shapeModule;
    GameObject boat;

    public static WindController instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    AnimationCurve sizeCurve;
    void Start()
    {
        pt = GetComponent<ParticleSystem>();
        // Set the custom size curve over the lifetime
        AnimationCurve lifetimeCurve = new AnimationCurve();
        ParticleSystem.SizeOverLifetimeModule soltM = pt.sizeOverLifetime;
        lifetimeCurve.AddKey(0f, .3f);
        lifetimeCurve.AddKey(0.33f, 1f);
        lifetimeCurve.AddKey(0.67f, 1f);
        lifetimeCurve.AddKey(1f, .3f);
        soltM.size = new ParticleSystem.MinMaxCurve(1f, lifetimeCurve);
        emissionModule = pt.emission;
        //Debug.Log("enmission Module: ");
        //Debug.Log(emissionModule);
        boat = GameObject.Find("Boat"); // transform.parent.gameObject;
        //Debug.Log(boat);
        particles = new ParticleSystem.Particle[pt.main.maxParticles];
        shapeModule = pt.shape;
    }

    // Update is called once per frame
    void Update()
    {   windStrength = Mathf.Clamp(windStrength, 1f, 30f);
        float boatDirection = boat.transform.rotation.eulerAngles.z;
        // shapeModule.position = boat.transform.position;
        float wdr = windDirection * Mathf.Deg2Rad;
        Vector2 windDirectionVector = new Vector3(Mathf.Cos(wdr), Mathf.Sin(wdr)) * windStrength;
        emissionModule.rateOverTimeMultiplier = windStrength/15f*10f; //new ParticleSystem.MinMaxCurve(windStrength*10f);
        
        ParticleSystem.MainModule mainModule = pt.main;
        mainModule.maxParticles = (int)(windStrength / 15f * 750) + 250;
        mainModule.simulationSpeed = 5f + windStrength / 2f;
        int particleCount = pt.GetParticles(particles);
        float speed = windStrength / 2000f; // .5f;
        Vector2 velocity = windDirectionVector * speed;
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].velocity = velocity;
            Color32 c = particles[i].GetCurrentColor(pt);
            c.a = 128;
            //particles[i].SetCurrentColor(c);
            //particles[i].rotation = windDirection;
        }
        
        // Debug.Log("boat: " + boatDirection.ToShortString() + ", wdr " + (wdr*Mathf.Rad2Deg).ToShortString()); //Debug.Log(windStrength);
        pt.SetParticles(particles, particleCount); // pt.particleCount * 100);

      //  gameObject.transform.localPosition = new Vector3(Mathf.Cos(wdr), Mathf.Sin(wdr), 0) * -1 * displacement;
    }
}
