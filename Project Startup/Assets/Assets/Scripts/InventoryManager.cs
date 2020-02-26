using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    // Start is called before the first frame update
    void Start()
    {
        inventoryBoard = GameObject.FindGameObjectWithTag("InventoryBoard").transform;
        inventoryCardPlacer = inventoryBoard.GetChild(1);
        
        if(debugPopulate)
            DebugPopulate(debugInventorySize);
        
        CreateInventoryCards();
        
        SortInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        GameObject card = Instantiate(inventoryCard);
        float xValue = 75.0f * inventoryCards.Count - 225.0f * Mathf.Floor((float) inventoryCards.Count / 3);
        float yValue = -60.0f * Mathf.Floor((float) inventoryCards.Count / 3);
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
        for (int i = 0; i < inventory.Count; i++)
        {
            float xValue = 75.0f * i - 225.0f * Mathf.Floor((float) i / 3);
            float yValue = -60.0f * Mathf.Floor((float) i / 3);
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
            int row = (int) Mathf.Floor((float) index / 3);
            for (int i = index -2; i < inventoryCards.Count; i++)
            {
                if (i != index)
                {
                    if ((int)Mathf.Floor((float) i / 3) == row && (float) i / 3 < (float) index / 3)
                    {
                        float xValue = 75.0f * i - 225.0f * Mathf.Floor((float) i / 3);
                        float yValue = -60.0f * (Mathf.Floor((float) i / 3)+1) -25;
                        Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
                        inventoryCards[i].LeanMove(cardPosition, 0.2f);
                    }
                    else if (Mathf.Floor((float) i / 3) >= row)
                    {
                        float xValue = 75.0f * (i-1) - 225.0f * Mathf.Floor((float) (i-1) / 3);
                        float yValue = -60.0f * (Mathf.Floor((float) (i-1) / 3)+1) -25;
                        Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
                        inventoryCards[i].LeanMove(cardPosition, 0.2f);
                    }
                }
                else
                {
                    float yValue = -60.0f * Mathf.Floor((float) i / 3);
                    Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(0, yValue, 0);
                    inventoryCards[i].LeanMove(cardPosition, 0.2f);
                }
            }
        }
        else
        {
            // get all elements that used to be on the same row and put them back
            int row = (int) Mathf.Floor((float) index / 3);
            for (int i = index -2; i < inventoryCards.Count; i++)
            {
                if (Mathf.Floor((float) i / 3) >= row)
                {
                    float xValue = 75.0f * i - 225.0f * Mathf.Floor((float) i / 3);
                    float yValue = -60.0f * Mathf.Floor((float) i / 3);
                    Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(xValue, yValue, 0);
                    inventoryCards[i].LeanMove(cardPosition, 0.2f);
                }
            }
        }
    }

    #endregion
}

