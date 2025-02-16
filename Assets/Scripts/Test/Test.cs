using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public float entanglementStrength = 0.1f;
    public float movementSpeed = 1f;

    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        int particleCount = particleSystem.GetParticles(particles);

        for (int i = 0; i < particleCount; i++)
        {
            for (int j = i + 1; j < particleCount; j++)
            {
                Vector3 direction = particles[j].position - particles[i].position;
                float distance = direction.magnitude;

                // Ä£Äâ¾À²øÁ¦
                Vector3 force = direction.normalized * (entanglementStrength / (distance * distance));

                particles[i].velocity += force * movementSpeed * Time.deltaTime;
                particles[j].velocity -= force * movementSpeed * Time.deltaTime;
            }
        }

        particleSystem.SetParticles(particles, particleCount);
    }
}
