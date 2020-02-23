using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer musicMixer;
    public GameObject settingsMenu;
    public Slider volumeSlider;
    public Toggle muteToggle;
    
    public bool muted;
    public float currentVolume = 0.0f;
    
    private void Start()
    {
        // Debug.Log("Current Muted bool value: " + muted);
        // Debug.Log(PlayerPrefs.GetInt("MutedBool") + " loaded MutedBool");
        
        // Load the previous audio level, and set the saved level on in the music mixer
        currentVolume = PlayerPrefs.GetFloat("MusicVol");
        musicMixer.SetFloat("MusicVolume", currentVolume);
        volumeSlider.value = PlayerPrefs.GetFloat("SliderPos");
        
        // Load whether it was muted bool
        if (PlayerPrefs.GetInt("MutedBool") == 0)
        {
            muted = false;
            muteToggle.isOn = false;
            //musicMixer.SetFloat("MusicVolume", currentVolume);
        } else if (PlayerPrefs.GetInt("MutedBool") == 1) 
        {
            muted = true;
            muteToggle.isOn = true;
            musicMixer.SetFloat("MusicVolume", -80.0f);
        }
    }

    public void SetVolume(float volume)
    {
        if (!muted)
        {
            musicMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            currentVolume = Mathf.Log10(volume) * 20;
            
            // Saving the values
            PlayerPrefs.SetFloat("SliderPos", volumeSlider.value);
            PlayerPrefs.SetFloat("MusicVol", currentVolume);
            
            // Debug.Log("Saved Music, Slider.pos");
        }
    }

    public void SetMute()
    {
        if (muted)
        {
            // Enable the music
            muted = false;
            musicMixer.SetFloat("MusicVolume", currentVolume);
            PlayerPrefs.SetInt("MutedBool", 0);
            
            // Debug.Log("Saved bool false");
        } else if (!muted)
        {
            // Disable the music
            muted = true;
            musicMixer.SetFloat("MusicVolume", -80.0f);
            PlayerPrefs.SetInt("MutedBool", 1);
            
            // Debug.Log("Saved bool true");
        }
    }

    public void EnableSettingsMenu()
    {
        if (!settingsMenu.activeInHierarchy)
        {
            settingsMenu.SetActive(true);
        } else if (settingsMenu.activeInHierarchy)
        {
            settingsMenu.SetActive(false);
        }
    }
}