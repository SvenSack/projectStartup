using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer musicMixer;

    public bool muted = false;
    public float currentVolume = 0.0f;
    public void SetVolume(float volume)
    {
        if (!muted)
        {
            musicMixer.SetFloat("MusicVolume", volume);
            currentVolume = volume;
        }
    }

    public void SetMute()
    {
        if (muted)
        {
            // Enable the music
            muted = false;
            musicMixer.SetFloat("MusicVolume", currentVolume);
        } else if (!muted)
        {
            // Disable the music
            muted = true;
            musicMixer.SetFloat("MusicVolume", -80.0f);
        } 
        
        /*
        if (muted)
        {
            muted = false;
            musicMixer.SetFloat("MusicVolume", currentVolume);
        }
        */
        
    }
}