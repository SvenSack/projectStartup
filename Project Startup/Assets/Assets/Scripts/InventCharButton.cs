using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventCharButton : MonoBehaviour
{
    private string name = "Please Senpai, I want a name UwU";
    public int indexNumber;
    private Sprite image;
    private bool showDetails;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(string nam, int index, Sprite img)
    {
        name = nam;
        indexNumber = index;
        image = img;
        ApplyData();
    }

    public void ToggleDetails()
    {
        if (showDetails)
        {
            // show the additional stats
        }
        else
        {
            // hide the stats
        }

        showDetails = !showDetails;
    }

    private void ApplyData()
    {
        TextMeshProUGUI tmp = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = name;
        if (name.Length > 6)
        {
            tmp.fontSize -= 2;
        }
        transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>().sprite = image;
    }
}
