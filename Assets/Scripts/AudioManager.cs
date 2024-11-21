using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public Sound[] sounds;

    void Awake()
    {
        instance = this;
        
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name)
    {   
        FindSound(name).source.Play();
    }

    private Sound FindSound(string name)
    {
        for (int x = 0; x < sounds.Length; x++)
        {
            if (sounds[x].name == name)
                return sounds[x];
        }

        return null;
    }
}
