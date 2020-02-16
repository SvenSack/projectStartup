using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public bool fightRunning;
    public TeamManager teamManager;
    public InventoryManager inventoryManager;

    private int castMask;
    private int floorMask;

    private Tile originTile;
    private bool fromTile;
    private bool holdingUnit;
    private Character heldUnit;
    private GameObject hideInFight;
    public GraphicRaycaster gRayCaster;
    public EventSystem eventSystem;

    public bool inventoryOpen;
    public Transform inventoryButton;
    
    // Start is called before the first frame update
    void Start()
    {
        castMask = LayerMask.GetMask("Tiles");
        floorMask = LayerMask.GetMask("Floor");
        hideInFight = GameObject.FindGameObjectWithTag("HideInFight");
    }

    // Update is called once per frame
    void Update()
    {
        if (!fightRunning)
        {
            // get units from drag and place them on drop
            if ( Input.GetMouseButtonDown (0)){ 
                RaycastHit hit; 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if ( Physics.Raycast (ray,out hit,100.0f, castMask))
                {
                    if (hit.transform.gameObject.GetComponent<Tile>().heldUnit != null &&
                        hit.transform.gameObject.GetComponent<Tile>().isYours)
                    {
                        originTile = hit.transform.gameObject.GetComponent<Tile>();
                        fromTile = true;
                        holdingUnit = true;
                    }
                }
                else
                {
                    List<RaycastResult> castHits = new List<RaycastResult>();
                    PointerEventData eventPoint = new PointerEventData(eventSystem);
                    eventPoint.position = Input.mousePosition;
                    gRayCaster.Raycast(eventPoint, castHits);
                    if (castHits.Count > 0)
                    {
                        for (int i = 0; i < castHits.Count; i++)
                        {
                            InventCharButton element = castHits[i].gameObject.GetComponentInParent<InventCharButton>();
                            if (element != null)
                            {
                                // take unit from inventory
                                Debug.Log(element.name);
                                heldUnit = inventoryManager.TakeFromInventory(element.indexNumber).GetComponent<Character>();
                                holdingUnit = true;
                                break;
                            }
                        }
                    }
                }
            }
            if ( Input.GetMouseButtonUp (0))
            { 
                RaycastHit hit; 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f, castMask))
                {
                    if (fromTile)
                    {
                        Tile targetTile = hit.transform.gameObject.GetComponent<Tile>();
                        if (targetTile.UnitPlace(originTile.heldUnit, originTile) != true)
                        {
                            // snap unit back
                            DropUnit();
                        }
                        else
                        {
                            holdingUnit = false;
                            fromTile = false;
                        }
                    }
                    else if (!fromTile && holdingUnit)
                    {
                        Tile targetTile = hit.transform.gameObject.GetComponent<Tile>();
                        if (targetTile.heldUnit != null && targetTile.isYours)
                        {
                            // swap unit with held unit
                            inventoryManager.AddInventoryCard(targetTile.heldUnit.instanceNumber);
                            Destroy(targetTile.heldUnit.gameObject);
                            teamManager.Remove(targetTile.heldUnit.gameObject);
                            teamManager.Add(heldUnit);
                            targetTile.heldUnit = heldUnit;
                            targetTile.CenterUnit();
                            holdingUnit = false;
                            heldUnit = null;
                        }
                        else if (teamManager.CheckSpot())
                        {
                            // place unit
                            if (targetTile.UnitPlace(heldUnit) != true)
                            {
                                // snap unit back into inventory
                                DropInventoryUnit();
                            }
                            else
                            {
                                teamManager.Add(heldUnit);
                                holdingUnit = false;
                                heldUnit = null;
                            }
                        }
                        else
                        {
                            // snap unit back into inventory and give feedback that team is full
                            DropInventoryUnit();
                        }
                    }
                }
                List<RaycastResult> castHits = new List<RaycastResult>();
                PointerEventData eventPoint = new PointerEventData(eventSystem);
                eventPoint.position = Input.mousePosition;
                gRayCaster.Raycast(eventPoint, castHits);
                if (castHits.Count > 0)
                {
                    for (int i = 0; i < castHits.Count; i++)
                    {
                        if (castHits[i].gameObject.CompareTag("InventoryBoard"))
                        {
                            // place into inventory, remove from board list
                            inventoryManager.AddInventoryCard(originTile.heldUnit.instanceNumber);
                            teamManager.Remove(originTile.heldUnit.gameObject);
                            inventoryManager.inventory.Add(inventoryManager.possibleCharacters[originTile.heldUnit.instanceNumber]);
                            Destroy(originTile.heldUnit.gameObject);
                            originTile.heldUnit = null;
                            originTile = null;
                            fromTile = false;
                            holdingUnit = false;
                        }
                    }
                }
                else
                {
                    if(fromTile)
                        DropUnit();
                    else if(heldUnit != null)
                        DropInventoryUnit();
                }
                    
                
            }
        }

        if (holdingUnit)
        {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, floorMask))
            {
                if(fromTile)
                    originTile.heldUnit.transform.position = hit.point + new Vector3(0, 0.5f, 0);
                else
                    heldUnit.transform.position = hit.point + new Vector3(0, 0.5f, 0);
            }
                
        }
        
    }

    public void StartFight()
    {
        fightRunning = true;
        hideInFight.SetActive(false);
        foreach (var character in teamManager.enemyTeam)
        {
            character.FindAggroTarget();
        }
        foreach (var character in teamManager.yourTeam)
        {
            character.FindAggroTarget();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryOpen)
        {
            inventoryButton.parent.LeanMoveX(Screen.width,.5f);
        }
        else
        {
            inventoryButton.parent.LeanMoveX(Screen.width - 240,.5f);
        }
        inventoryOpen = !inventoryOpen;
        StartCoroutine(RotateButton());
    }
    
    private IEnumerator RotateButton()
    {
        yield return new WaitForSeconds(.4f);
        inventoryButton.Rotate(new Vector3(0,0,180));
    }

    private void DropUnit()
    {
        originTile.CenterUnit();
        originTile = null;
        fromTile = false; 
        holdingUnit = false;
    }

    private void DropInventoryUnit()
    {
        inventoryManager.AddInventoryCard(heldUnit.instanceNumber);
        inventoryManager.inventory.Add(inventoryManager.possibleCharacters[heldUnit.instanceNumber]);
        Destroy(heldUnit.gameObject);
        holdingUnit = false;
        heldUnit = null;
    }
}
