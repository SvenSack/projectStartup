using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    
    public float speed = 30;
    public Transform target;

    private void Start()
    {
        transform.LeanMove(target.position+(target.forward)*.2f, 1f / speed);
    }

}
