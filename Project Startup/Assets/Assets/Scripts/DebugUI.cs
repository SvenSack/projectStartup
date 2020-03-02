using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    private TextMeshProUGUI textOutput;
    // Start is called before the first frame update
    void Awake()
    {
        textOutput = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textOutput.text = "" + 1.0f / Time.deltaTime;
    }
}
