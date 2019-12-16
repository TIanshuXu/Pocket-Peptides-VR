using UnityEngine;
using UnityEngine.Audio;

public class OptionMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMasterVolume(float vol)
    {
        audioMixer.SetFloat("Master", vol);
    }

    public void SetSFXVolume(float vol)
    {
        audioMixer.SetFloat("SFX", vol);
    }

    public void SetMusicVolume(float vol)
    {
        audioMixer.SetFloat("Music", vol);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
