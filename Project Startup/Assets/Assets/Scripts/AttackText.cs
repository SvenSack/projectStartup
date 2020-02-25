using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float textFadeValue = 1.0f;
    public Color baseColor = new Color(1,1,1, 1);
    
    // Start is called before the first frame update
    void Start()
    {
        transform.LeanMove(transform.position + new Vector3(0, 35.0f, 0), 2.0f);
        transform.LeanScale(Vector3.zero, 2.0f);
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textFadeValue > 0)
        {
            textFadeValue -= Time.deltaTime * .5f;
        }
        else Destroy(gameObject);

        baseColor.a = textFadeValue;
        textMesh.color = baseColor;
    }
}
