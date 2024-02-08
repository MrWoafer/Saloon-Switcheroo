using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Particle
{
    [SerializeField]
    public string name;
    [SerializeField]
    public ParticleSystem particles;
}

public class ParticleManager : MonoBehaviour
{
    [Header("Particles")]
    public Particle[] particles;

    public void Play(string particleName, Vector3 position, Vector3 rotation)
    {
        foreach (Particle i in particles)
        {
            if (i.name == particleName)
            {
                Instantiate(i.particles, position, Quaternion.Euler(rotation), null);
                break;
            }
        }
    }

    public void Play(string particleName, Vector3 position, Vector3 rotation, int sortingOrder)
    {
        foreach (Particle i in particles)
        {
            if (i.name == particleName)
            {
                Renderer rend = Instantiate(i.particles, position, Quaternion.Euler(rotation), null).GetComponent<Renderer>();
                rend.sortingOrder = sortingOrder;
                break;
            }
        }
    }
}
