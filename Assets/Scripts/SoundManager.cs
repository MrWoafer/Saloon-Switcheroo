using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 3f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    [HideInInspector]
    public AudioSource source;
    public bool loop = false;
}

public class SoundManager : MonoBehaviour
{
    [Header("Sounds")]
    public Sound[] sounds;

    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        foreach(Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Play();
                break;
            }
        }
    }
}
