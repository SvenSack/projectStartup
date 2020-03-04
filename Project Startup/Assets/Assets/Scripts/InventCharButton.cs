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
    public GameObject stats;
    private InventoryManager inventoryManager;
    private float defaultFontsize = 22;
    private Slider[] sliders;
    public bool detailShower = false;

    // Start is called before the first frame update
    void Awake()
    {
        backDrop = transform.GetChild(0).GetComponent<RectTransform>();
        
        stats = transform.GetChild(1).gameObject;
        sliders = stats.GetComponentsInChildren<Slider>();
    }

    void Start()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryManager>();
        Character myCharacter = inventoryManager.possibleCharacters[charIndex].GetComponent<Character>();
        
        sliders[0].value = myCharacter.health;
        sliders[1].value = myCharacter.defense;
        sliders[2].value = myCharacter.attackDamage;
        sliders[3].value = 4.1f - myCharacter.attackCooldown;
        sliders[4].value = myCharacter.range;
        
        if(!detailShower)
            stats.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(string nam, int index, Sprite img, Character.archetype archetype, string ability, bool change)
    {
        name = nam;
        charIndex = index;
        image = img;
        ApplyData(archetype);
        TextMeshProUGUI[] possible = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var element in possible)
        {
            if (element.gameObject.CompareTag("AbilityExplanation"))
            {
                element.text = "<b>Ability:</b> " + ability;
                break;
            }
        }

        if (change)
        {
            Character myCharacter = inventoryManager.possibleCharacters[charIndex].GetComponent<Character>();
            Slider[] sliders = stats.GetComponentsInChildren<Slider>();
            sliders[0].value = myCharacter.health;
            sliders[1].value = myCharacter.defense;
            sliders[2].value = myCharacter.attackDamage;
            sliders[3].value = 4.1f - myCharacter.attackCooldown;
            sliders[4].value = myCharacter.range;
        }
    }

    public bool ToggleDetails()
    {
        // Debug.Log("I am toggling " + gameObject.name);
        showDetails = !showDetails;
        if (showDetails)
        {
            // show the additional stats
            backDrop.LeanSize(new Vector2(375, 135), .2f);
            stats.SetActive(true);
            return true;
        }
        else
        {
            // hide the stats
            backDrop.LeanSize(new Vector2(96, 96), .2f);
            stats.SetActive(false);
            return false;
        }
    }

    private void ApplyData(Character.archetype archetype)
    {
        TextMeshProUGUI tmp = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        tmp.fontSize = defaultFontsize;
        tmp.text = name;
        if (name.Length > 6)
        {
            tmp.fontSize -= 6;
        }
        Image img = transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>();
        img.sprite = image;
        Image imgb = img.transform.parent.GetComponent<Image>();
        switch (archetype)
        {
            case Character.archetype.Attacker:
                imgb.color = new Color(0.7924528f, 0.1831613f, 0.2159052f);
                break;
            case Character.archetype.Tank:
                imgb.color = new Color(0.4298683f, 0.6714197f, 0.7924528f);
                break;
            case Character.archetype.Assassin:
                imgb.color = new Color(0.5283019f, 0.2815949f, 0.4347313f);
                break;
            case Character.archetype.Support:
                imgb.color = new Color(0.3177287f, 0.7924528f, 0.4481168f);
                break;
        }
    }
}
