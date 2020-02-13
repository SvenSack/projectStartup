using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Character> inventory = new List<Character>();
    public GameObject[] possibleCharacters = new GameObject[6];
    
    // Start is called before the first frame update
    void Start()
    {
        
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
}
