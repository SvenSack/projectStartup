using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AveryHatesThis : MonoBehaviour
{
    public float spinSpeed = 1;
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = transform.eulerAngles+Time.deltaTime * spinSpeed * new Vector3(0,1,0);
    }
}
