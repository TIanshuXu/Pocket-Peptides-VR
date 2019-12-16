using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "CutScene")
        {
            Destroy(GameObject.Find(gameObject.name));
            return;
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip   = s.clip;
            s.source.volume = s.volume;
            s.source.pitch  = s.pitch;
            s.source.loop   = s.loop;
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "CutScene") { return; }
        if (SceneManager.GetActiveScene().buildIndex == 0) { Play("Theme Main"); }
        if (SceneManager.GetActiveScene().buildIndex == 1) { Play("Theme Play"); }
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    } // this is used in Draggable script
}
