using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();
    public GameObject[] possibleCharacters = new GameObject[6];
    public GameObject inventoryCard;

    public int debugInventorySize = 1;
    public bool debugPopulate;
    
    private List<GameObject> inventoryCards = new List<GameObject>();

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Character NewCharacter(int index)
    {
        GameObject newChar = Instantiate(possibleCharacters[index]);
        Character character = newChar.GetComponent<Character>();
        return character;
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

    public GameObject AddInventoryCard(int index)
    {
        GameObject card = Instantiate(inventoryCard);
        Vector3 cardPosition = inventoryCardPlacer.position + new Vector3(75.0f * inventoryCards.Count-225.0f*Mathf.Floor((float)inventoryCards.Count /3), -60.0f*Mathf.Floor((float)inventoryCards.Count /3), 0);
        inventoryCards.Add(card);
        Character character = possibleCharacters[index].GetComponent<Character>();
        card.GetComponent<InventCharButton>().Set(character.name, index , character.profilePic);
        card.transform.position = cardPosition;
        card.transform.SetParent(inventoryBoard, true);
        return card;
    }
}

