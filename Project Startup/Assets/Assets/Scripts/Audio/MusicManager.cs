using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{ 
    public AudioSource introMusic; 
    public AudioSource loopMusic; 
    private bool _startedLoop;
 
    // This I didn't come up with myself
    // But it is quite fun
    // Basically, there's the parent, with the intro music itself as the AudioSource
    // Then, as its children, it has intro clip and the loop clip (wherein the loop clip is enabled as loopable)
    // When the IntroMusic is no longer playing, it then starts playing the looping music
    // and the started loop is set as active thus, meaning it won't break the loop
    // and because it's in FixedUpdate, it won't make the music stutter in low framerate
    
    private void Start()
    {
        if (introMusic == null || loopMusic == null)
        { throw new Exception("The intro and loop AudioSources haven't been placed in the " + gameObject.name); }
    } 
    
    private void FixedUpdate()
    {
        if (!introMusic.isPlaying && !_startedLoop)
        {
            loopMusic.Play();
            Debug.Log("Done playing");
            _startedLoop = true;
        }
    }
}
