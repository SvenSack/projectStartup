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
            musicMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            currentVolume = Mathf.Log10(volume) * 20;
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
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVol", currentVolume);
        PlayerPrefs.SetInt("MutedBool", (muted ? 1: 0));
    }

    public void LoadSettings()
    {
        // Load the previous audio level
        musicMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVol"));

        // Load whether it was muted bool
        muted = (PlayerPrefs.GetInt("MutedBool") != 0);
    }
}