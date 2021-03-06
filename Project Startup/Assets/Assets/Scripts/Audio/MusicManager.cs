﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{ 
    public AudioSource introMusic; 
    public AudioSource loopMusic; 
    [HideInInspector] public bool startedLoop;

    private void Awake()
    {
        if (introMusic == null || loopMusic == null)
        { throw new Exception("The intro and loop AudioSources haven't been placed in the " + gameObject.name); }
        introMusic.Play();
    } 
    
    private void FixedUpdate()
    {
        if (!introMusic.isPlaying && !startedLoop)
        {
            loopMusic.Play();
            // Debug.Log("Done playing");
            startedLoop = true;
        }
    }
}
