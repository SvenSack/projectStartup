using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDontDestroy : MonoBehaviour
{
    private void Awake()
    {
        GameObject music = GameObject.FindGameObjectWithTag("music");
        Destroy(music);
    }
}
