using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OrbCollector : MonoBehaviour
{
    public ParticleSystem theParticleSystem;
    GameObject player;
    public float speed = 20f;

    public float activationTime;
    private bool isActivated;
    public UnityEvent OnActivated;

    IEnumerator Start()
    {
        player = GameObject.Find("Player");

        yield return new WaitForSecondsRealtime(activationTime);

        isActivated = true;

        OnActivated?.Invoke();
    }

    void Update()
    {
        if (!isActivated)
        {
            return;
        }

        var particles = new ParticleSystem.Particle[theParticleSystem.main.maxParticles];
        var currentAmount = theParticleSystem.GetParticles(particles);

        for (int i = 0; i < currentAmount; i++)
        {
            var direction = (player.transform.position - particles[i].position).normalized;
            particles[i].position = particles[i].position + direction * Time.deltaTime * speed;
        }

        // Apply the particle changes to the Particle System
        theParticleSystem.SetParticles(particles, currentAmount);
    }
}
