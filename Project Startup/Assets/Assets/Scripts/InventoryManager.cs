using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>(); // the inventory of units
    public GameObject[] possibleCharacters = new GameObject[6]; // the list of possible units
    public GameObject inventoryCard; // the blueprint for each inventorycard

    public int debugInventorySize = 1; // the size of the testing inventory
    public bool debugPopulate; // bool checking if you are currently using the testing inventory
    
    public List<GameObject> inventoryCards = new List<GameObject>(); // the list of current inventorycards

    private Transform inventoryBoard;
    private Transform inventoryCardPlacer;
    
    private bool inventoryOpen = true;
    public bool inventoryScreen;
    private EventSystem eventSystem;
    private GraphicRaycaster gRayCaster;
    private bool showingDetails;
    private Character shownUnit;
    private Transform unitSpotlight;
    private Transform statShower;
    private SpiderChart spiderMate;
    private TextMeshProUGUI[] statsBreakdown;
    public Transform uiScroll;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (inventoryScreen)
        {
            unitSpotlight = GameObject.FindGameObjectWithTag("UnitSpotlight").transform;
            statShower = GameObject.FindGameObjectWithTag("UnitStats").transform;
            eventSystem = FindObjectOfType<EventSystem>();
            gRayCaster = FindObjectOfType<GraphicRaycaster>();
            spiderMate = FindObjectOfType<SpiderChart>();
            GameObject statBreakdown = statShower.GetChild(3).gameObject;
            statsBreakdown = statBreakdown.GetComponentsInChildren<TextMeshProUGUI>();
        }

        inventoryBoard = GameObject.FindGameObjectWithTag("InventoryBoard").transform;
        inventoryCardPlacer = inventoryBoard.GetChild(1);
        
        if(debugPopulate)
            DebugPopulate(debugInventorySize);
        else
        {
            if (!System.IO.File.Exists(Application.persistentDataPath + "/saveFile.json"))
            {
                InventoryData dat = new InventoryData();
                string jsonData = JsonUtility.ToJson(dat);
                File.WriteAllText(Application.persistentDataPath + "/saveFile.json", jsonData);
            }
            string json = File.ReadAllText(Application.persistentDataPath + "/saveFile.json");
            int[] loadData = JsonUtility.FromJson<InventoryData>(json).data;
            for (int i = 0; i < possibleCharacters.Length; i++)
            {
                if(loadData[i] > 0)
                    for (int j = 0; j < loadData[i]; j++)
                    {
                        inventory.Add(possibleCharacters[i]);
                    }
            }
        }
        
        CreateInventoryCards();
        
        
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        SortInventory();
        if (inventoryScreen)
            ShowUnit(inventory[0].GetComponent<Character>().instanceNumber, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && inventoryScreen)
        {
            List<RaycastResult> castHits = new List<RaycastResult>();
            PointerEventData eventPoint = new PointerEventData(eventSystem);
            eventPoint.position = Input.mousePosition;
            gRayCaster.Raycast(eventPoint, castHits);
            if (castHits.Count > 0)
            {
                for (int i = 0; i < castHits.Count; i++)
                {
                    // Debug.Log("I hit " + castHits[i].gameObject.name);
                    InventCharButton element = castHits[i].gameObject.GetComponentInParent<InventCharButton>();
                    if (element != null)
                    {
                        // show details
                        ShowUnit(element.charIndex, false);
                        break;
                    }
                }
            }
        }
    }

    private void ShowUnit(int charIndex, bool firstRun)
    {
        if (shownUnit != null)
        {
            Button[] stars1 = statShower.GetChild(1).GetComponentsInChildren<Button>();
            for (int j = 0; j < shownUnit.rarity; j++)
            {
                stars1[j].interactable = true;
            }
                            
            Destroy(shownUnit.gameObject);
        }
        GameObject newGO = Instantiate(possibleCharacters[charIndex], unitSpotlight.position, Quaternion.Euler(0,150,0));
        newGO.transform.SetParent(unitSpotlight, true);
        shownUnit = newGO.GetComponent<Character>();
        showingDetails = true;
        if(!firstRun)
            ToggleButton();
        TextMeshProUGUI name = statShower.GetChild(0).GetComponent<TextMeshProUGUI>();
        Button[] stars = statShower.GetChild(1).GetComponentsInChildren<Button>();
        name.text = shownUnit.name;
        for (int j = 0; j < shownUnit.rarity; j++)
        {
            stars[j].interactable = true;
        }
        spiderMate.Spiderize(shownUnit.defense/10f,shownUnit.health/100f, shownUnit.range/10f, (10-shownUnit.attackCooldown*5)/10f, shownUnit.attackDamage/10f);
        string[] tempText;
        tempText = statsBreakdown[0].text.Split(' ');
        statsBreakdown[0].text = tempText[0] + " " + (int)shownUnit.attackDamage * 10;
        tempText = statsBreakdown[1].text.Split(' ');
        statsBreakdown[1].text = tempText[0] + " " + (int)(1/shownUnit.attackCooldown) + " / second";
        tempText = statsBreakdown[2].text.Split(' ');
        statsBreakdown[2].text = tempText[0] + " " + (int)shownUnit.range * 10;
        tempText = statsBreakdown[3].text.Split(' ');
        statsBreakdown[3].text = tempText[0] + " " + (int)shownUnit.health * 10;
        tempText = statsBreakdown[4].text.Split(' ');
        statsBreakdown[4].text = tempText[0] + " " + (int)shownUnit.defense * 10;
        shownUnit.enabled = false;
    }
    
    public Character NewCharacter(int index)
    {
        Quaternion rot = new Quaternion(0,0,0,0);
        rot.eulerAngles = new Vector3(0,0,0);
        GameObject newChar = Instantiate(possibleCharacters[index], new Vector3(0,0,0), rot);
        Character character = newChar.GetComponent<Character>();
        character.isOnYourTeam = true;
        return character;
    }

    #region Setup
    
    private class InventoryData
    {
    
        public int[] data = new int[] {2, 4, 2, 2, 5, 2, 1};
    }
    private void DebugPopulate(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            inventory.Add(possibleCharacters[Random.Range(0, possibleCharacters.Length)]);
        }
    }

    private void CreateInventoryCards()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject card = AddInventoryCard(inventory[i].GetComponent<Character>().instanceNumber);
        }
    }

    private GameObject AddInventoryCard(int index)
    {
        GameObject card = Instantiate(inventoryCard, inventoryBoard);
        float xValue = 100.0f * inventoryCards.Count - (100f*4) * Mathf.Floor((float) inventoryCards.Count / 4);
        float yValue = -90.0f * Mathf.Floor((float) inventoryCards.Count / 4);
        Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
        inventoryCards.Add(card);
        Character character = possibleCharacters[index].GetComponent<Character>();
        card.GetComponent<InventCharButton>().Set(character.name, index , character.profilePic, character.type, character.ability, false);
        card.transform.position = cardPosition;
        card.transform.SetParent(inventoryBoard, true);
        return card;
    }
    private int SortUnitByCharacterArchetype(GameObject c1, GameObject c2)
    {
        int val = c1.GetComponent<Character>().type.CompareTo(c2.GetComponent<Character>().type);
        if(val == 0)
            val = c1.GetComponent<Character>().name.CompareTo(c2.GetComponent<Character>().name);
        return val;
    }
    
    private int SortCardByCharacterArchetype(GameObject c1, GameObject c2)
    {
        int val = possibleCharacters[c1.GetComponent<InventCharButton>().charIndex].GetComponent<Character>().type.
            CompareTo(possibleCharacters[c2.GetComponent<InventCharButton>().charIndex].GetComponent<Character>().type);
        if(val == 0)
            val = possibleCharacters[c1.GetComponent<InventCharButton>().charIndex].GetComponent<Character>().name.
                CompareTo(possibleCharacters[c2.GetComponent<InventCharButton>().charIndex].GetComponent<Character>().name);
        return val;
    }

    private void SortInventory()
    {
        inventory.Sort(SortUnitByCharacterArchetype);
        inventoryCards.Sort(SortCardByCharacterArchetype);
        foreach (var card in inventoryCards)
        {
            InventCharButton element = card.GetComponent<InventCharButton>();
            if (element.showDetails)
                element.ToggleDetails();
        }
        if(!inventoryScreen)
        for (int i = 0; i < inventory.Count; i++)
        {
            float xValue = 100.0f * i - (100f*4) * Mathf.Floor((float) i / 4);
            float yValue = -90.0f * Mathf.Floor((float) i / 4);
            Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
            inventoryCards[i].LeanMove(cardPosition, 0.2f);
        }
        else
            for (int i = 0; i < inventory.Count; i++)
            {
                float xValue = 120.0f * i - (120f*6) * Mathf.Floor((float) i / 6);
                float yValue = -90.0f * Mathf.Floor((float) i / 6);
                Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
                inventoryCards[i].LeanMove(cardPosition, 0.2f);
            }
    }

    
    #endregion

    #region Inventory Manipulation

    public GameObject TakeFromInventory(GameObject target)
    {
        int index = target.GetComponent<InventCharButton>().charIndex;
        RemoveInventoryCard(target);
        return NewCharacter(index).gameObject;
    }

    private void RemoveInventoryCard(GameObject target)
    {
        int index = FetchCardIndex(target);
        
        inventory.Remove(inventory[index]);
        inventoryCards.Remove(target);
        Destroy(target);
        SortInventory();
    }

    public void AddCard(int index)
    {
        inventory.Add(possibleCharacters[index]);
        AddInventoryCard(index);
        SortInventory();
    }

    #endregion

    private int FetchCardIndex(GameObject cardObject)
    {
        for (int i = 0; i < inventoryCards.Count; i++)
        {
            if (inventoryCards[i] == cardObject)
            {
                return i;
            }
        }

        return inventoryCards.Count + 1;
    }

    #region Toggle Inventory Details

    public void ToggleInventoryDetails(GameObject target)
    {
        int index = FetchCardIndex(target);

        if (inventoryCards[index].GetComponent<InventCharButton>().ToggleDetails())
        {
            // toggle it off for all that are currently open
            foreach (var card in inventoryCards)
            {
                if (inventoryCards[index] != card && card.GetComponent<InventCharButton>().showDetails)
                {
                    ToggleInventoryDetails(card);
                }
            }
            // get all elements from the same and its following rows and move them down and to the left by one
            int row = (int) Mathf.Floor((float) index / 4);
            for (int i = index -3; i < inventoryCards.Count; i++)
            {
                if (i != index)
                {
                    if ((int)Mathf.Floor((float) i / 4) == row && (float) i / 4 < (float) index / 4)
                    {
                        float xValue = 100.0f * i - (100f*4) * Mathf.Floor((float) i / 4);
                        float yValue = -90.0f * (Mathf.Floor((float) i / 4)+1) -25;
                        Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
                        inventoryCards[i].LeanMove(cardPosition, 0.2f);
                    }
                    else if (Mathf.Floor((float) i / 4) >= row)
                    {
                        float xValue = 100.0f * (i-1) - (100f*4) * Mathf.Floor((float) (i-1) / 4);
                        float yValue = -90.0f * (Mathf.Floor((float) (i-1) / 4)+1) -25;
                        Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
                        inventoryCards[i].LeanMove(cardPosition, 0.2f);
                    }
                }
                else
                {
                    float yValue = -90.0f * Mathf.Floor((float) i / 4);
                    Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(0, yValue, 0);
                    inventoryCards[i].LeanMove(cardPosition, 0.2f);
                }
            }
        }
        else
        {
            // get all elements that used to be on the same row and put them back
            int row = (int) Mathf.Floor((float) index / 4);
            for (int i = index -3; i < inventoryCards.Count; i++)
            {
                if (Mathf.Floor((float) i / 4) >= row)
                {
                    float xValue = 100.0f * i - (100f*4) * Mathf.Floor((float) i / 4);
                    float yValue = -90.0f * Mathf.Floor((float) i / 4);
                    Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
                    inventoryCards[i].LeanMove(cardPosition, 0.2f);
                }
            }
        }
    }

    public void ToggleButton()
    {
        if (inventoryOpen)
        {
            inventoryBoard.LeanMoveX(Screen.width,1f);
            uiScroll.LeanRotate(new Vector3(0, 120, 0), 1f);
        }
        else
        {
            inventoryBoard.LeanMoveX(Screen.width - 870,1f);
            uiScroll.LeanRotate(new Vector3(0, -120, 0), 1f);
        }
        inventoryOpen = !inventoryOpen;
    }

    #endregion
}



