using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventCharButton : MonoBehaviour
{
    private string name = "Please Senpai, I want a name UwU"; // the name on the card
    public int charIndex; // the characters possible unit index
    private Sprite image; // the picture on the card
    public bool showDetails; // bool tracking if the card is in extended detail mode

    private RectTransform backDrop;
    private GameObject stats;
    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
        backDrop = transform.GetChild(0).GetComponent<RectTransform>();
        stats = transform.GetChild(1).gameObject;
        Slider[] sliders = stats.GetComponentsInChildren<Slider>();
        Character myCharacter = inventoryManager.possibleCharacters[charIndex].GetComponent<Character>();
        sliders[0].value = myCharacter.health;
        sliders[1].value = myCharacter.defense;
        sliders[2].value = myCharacter.attackDamage;
        sliders[3].value = 4.1f - myCharacter.attackCooldown;
        sliders[4].value = myCharacter.range;
        stats.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(string nam, int index, Sprite img)
    {
        name = nam;
        charIndex = index;
        image = img;
        ApplyData();
    }

    public bool ToggleDetails()
    {
        showDetails = !showDetails;
        if (showDetails)
        {
            // show the additional stats
            backDrop.LeanSize(new Vector2(200, 50), .2f);
            stats.SetActive(true);
            return true;
        }
        else
        {
            // hide the stats
            backDrop.LeanSize(new Vector2(50, 50), .2f);
            stats.SetActive(false);
            return false;
        }
    }

    private void ApplyData()
    {
        TextMeshProUGUI tmp = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        tmp.text = name;
        if (name.Length > 6)
        {
            tmp.fontSize -= 2;
        }
        transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>().sprite = image;
    }
}
