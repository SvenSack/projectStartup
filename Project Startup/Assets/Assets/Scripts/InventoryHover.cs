using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryHover : MonoBehaviour, IPointerExitHandler
{
    public bool isHolding;
    public GameObject heldObject;
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            heldObject.transform.position = Input.mousePosition;
        }
    }

    public void HoldThis(GameObject toHold)
    {
        isHolding = true;
        heldObject = toHold;
        heldObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        // Debug.Log("UWU pweased to howd this: " + toHold.name);
    }

    public void DropIt()
    {
        heldObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        isHolding = false;
        heldObject = null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHolding)
        {
            Debug.Log("OWO whewe awe you going ?");
            gameManager.TakeUnitFromInventory(heldObject.GetComponent<InventCharButton>());
            DropIt();
        }
    }
}
