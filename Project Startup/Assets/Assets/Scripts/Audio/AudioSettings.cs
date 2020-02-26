using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer musicMixer;
    public GameObject settingsMenu;
    public Slider volumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle muteToggle;
    
    [HideInInspector] public bool muted;
    [HideInInspector] public float musicVolume = 0.0f;
    [HideInInspector] public float currentSfxVolume = 0.0f;

    private void Start()
    {
        LoadSettings();
        if (settingsMenu.activeInHierarchy == true)
        {
            settingsMenu.SetActive(false);
        }
    }

    // Change the Music volume
    public void SetVolume(float volume)
    {
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        musicVolume = Mathf.Log10(volume) * 20;
            
        // Saving the values
        PlayerPrefs.SetFloat("SliderPos", volumeSlider.value);
        PlayerPrefs.SetFloat("MusicVol", musicVolume);
    }
    
    // Change the sound effect volume
    public void SetSfxVolume(float volume)
    { 
        musicMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20); 
        currentSfxVolume = Mathf.Log10(volume) * 20;
            
        // Saving the values
        PlayerPrefs.SetFloat("SFXSliderPos", sfxVolumeSlider.value);
        PlayerPrefs.SetFloat("SFXVol", currentSfxVolume);
    }

    public void SetMute()
    {
        if (muted)
        {
            // Enable the music
            muted = false;
            musicMixer.SetFloat("GameVolume", 0);
            PlayerPrefs.SetInt("MutedBool", 0);
        } else if (!muted)
        {
            // Disable the music
            muted = true;
            musicMixer.SetFloat("GameVolume", -80.0f);
            PlayerPrefs.SetInt("MutedBool", 1);
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

    private void LoadSettings()
    {
        // Load the previous audio level, and set the saved level on in the music mixer
        musicVolume = PlayerPrefs.GetFloat("MusicVol", 0.0f);
        musicMixer.SetFloat("MusicVolume", musicVolume);
        volumeSlider.value = PlayerPrefs.GetFloat("SliderPos", 1.0f);
        
        currentSfxVolume = PlayerPrefs.GetFloat("SFXVol", 0.0f);
        musicMixer.SetFloat("SFXVolume", currentSfxVolume);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXSliderPos", 1.0f);
        
        
        // Load whether it was muted bool
        if (PlayerPrefs.GetInt("MutedBool", 0) == 0)
        {
            muted = false;
            muteToggle.isOn = false;
        } else if (PlayerPrefs.GetInt("MutedBool", 0) == 1) 
        {
            muted = true;
            muteToggle.isOn = true;
            musicMixer.SetFloat("GameVolume", -80.0f);
        }
    }
}