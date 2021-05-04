using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollision : MonoBehaviour
{
    public bool isInvis;

    private void OnParticleCollision(GameObject col)
    {
        if (isInvis)
            return;

        // Check if particle collided with a character
        if(col.tag == "Character")
        {
            // Check if the character was phasing, if so then do nothing
            if(col.GetComponent<CollisionScript>().phasing) {
                return;
            }

            // Otherwise, kill the player
            int playerID = col.GetComponent<CollisionScript>().playerID;
            Destroy(col);
            GameObject.Find("GameManager").GetComponent<GameManager>().PlayerHasDied(playerID);
        }
    }

    // Called everytime particles meet the trigger requirements
    private void OnParticleTrigger() {
        if (isInvis)
            return;

        // Particle System instance
        ParticleSystem trailPS = GetComponent<ParticleSystem>();

        // Different event calls
        ParticleEnter(trailPS);
        ParticleStay(trailPS);
        ParticleExit(trailPS);
    }

    // Called when a particle enters a collider
    private void ParticleEnter(ParticleSystem trailPS) {
        // Get all particles that entered collider
        List<ParticleSystem.Particle> enteredParticles = new List<ParticleSystem.Particle>();
        int enterCount = trailPS.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enteredParticles);

        // Get all sight cones
        GameObject[] sightCones = GameObject.FindGameObjectsWithTag("SightCone");

        // Keep track of which had colliisons
        bool[] hasParticles = new bool[sightCones.Length];

        // Calculate closest particle for each sightcone
        float[] minDistance = new float[sightCones.Length];
        for(int i = 0; i < minDistance.Length; i++) minDistance[i] = Mathf.Infinity;

        // Loop through particles
        foreach(ParticleSystem.Particle particle in enteredParticles) {
            for(int i = 0; i < sightCones.Length; i++) {
                Collider collider = sightCones[i].GetComponent<Collider>();
                if(collider.bounds.Contains(particle.position)) { 
                    // Get sight cones parent and notify AI controller
                    minDistance[i] = Mathf.Min(minDistance[i], Vector3.Distance(sightCones[i].transform.position, particle.position));
                    hasParticles[i] = true;
                }
            }
        }

        // Notify correct sight cones
        for(int i = 0; i < sightCones.Length; i++) {
            if(hasParticles[i]) {
                sightCones[i].GetComponentInParent<AIController>().NotifyTrail(true, minDistance[i]);
            }
        }
    }

    // Called when a particle stays inside a collider
    private void ParticleStay(ParticleSystem trailPS) {
        // Get all particles that entered collider
        List<ParticleSystem.Particle> insideParticles = new List<ParticleSystem.Particle>();
        int insideCount = trailPS.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideParticles);

        // Get all sight cones
        GameObject[] sightCones = GameObject.FindGameObjectsWithTag("SightCone");

        // Calculate closest particle for each sightcone
        float[] minDistance = new float[sightCones.Length];
        for(int i = 0; i < minDistance.Length; i++) minDistance[i] = Mathf.Infinity;

        // Loop through particles
        foreach(ParticleSystem.Particle particle in insideParticles) {
            for(int i = 0; i < sightCones.Length; i++) {
                Collider collider = sightCones[i].GetComponent<Collider>();
                if(collider.bounds.Contains(particle.position)) { 
                    // Get sight cones parent and notify AI controller
                    minDistance[i] = Mathf.Min(minDistance[i], Vector3.Distance(sightCones[i].transform.position, particle.position));
                    sightCones[i].GetComponentInParent<AIController>().NotifyTrail(true, minDistance[i]);
                }
            }
        }
    }

    // Called when a particle exits a collider
    private void ParticleExit(ParticleSystem trailPS) {
        // Get all particles that exited collider
        List<ParticleSystem.Particle> exitedParticles = new List<ParticleSystem.Particle>();
        int exitCount = trailPS.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitedParticles);

        // Get all sight cones
        GameObject[] sightCones = GameObject.FindGameObjectsWithTag("SightCone");
        // Bools that correspond with sight cones to determine if any particles still exist in each
        bool[] hasParticles = new bool[sightCones.Length];

        // Loop through particles and see what collisions still exist
        foreach(ParticleSystem.Particle particle in exitedParticles) {
            for(int i = 0; i < sightCones.Length; i++) {
                Collider collider = sightCones[i].GetComponent<Collider>();
                if(collider.bounds.Contains(particle.position)) {
                    // Set corresponding index in bool array
                    hasParticles[i] = true;
                }
            }
        }

        // For all cones that no longer have particles, notify their AI Controller
        for(int i = 0; i < hasParticles.Length; i++) {
            if(!hasParticles[i]) {
                sightCones[i].GetComponentInParent<AIController>().NotifyTrail(false, Mathf.Infinity);
            }
        }
    }
}