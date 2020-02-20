using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

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
    private GameObject showInFight;
    public GraphicRaycaster gRayCaster;
    public EventSystem eventSystem;

    public bool inventoryOpen;
    public Transform inventoryButton;
    public InventoryHover inventoryHover;

    private bool checkingDrag;
    private Coroutine dragCheck;
    
    // Start is called before the first frame update
    void Start()
    {
        castMask = LayerMask.GetMask("Tiles");
        floorMask = LayerMask.GetMask("Floor");
        hideInFight = GameObject.FindGameObjectWithTag("HideInFight");
        showInFight = GameObject.FindGameObjectWithTag("ShowInFight");
        showInFight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!fightRunning)
        {
            // get units from drag and place them on drop
            if ( Input.GetMouseButtonDown (0))
            {
                dragCheck = StartCoroutine(waitToConfirmDrag(0.2f, Input.mousePosition));
            }
            if ( Input.GetMouseButtonUp (0))
            {
                if (checkingDrag)
                {
                    StopCoroutine(dragCheck);
                }
                else if(holdingUnit || fromTile)
                    UnitRelease(Input.mousePosition);
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
                {
                    heldUnit.transform.position = hit.point + new Vector3(0, 0.5f, 0);
                }
                 
            }
                
        }
        
    }

    public void StartFight()
    {
        fightRunning = true;
        hideInFight.SetActive(false);
        showInFight.SetActive(true);
        Slider[] healthbars = showInFight.transform.GetChild(0).gameObject.GetComponentsInChildren<Slider>();
        for (int i = 0; i < healthbars.Length; i++)
        {
            Debug.Log(healthbars[i].name);
            if (i < healthbars.Length/2)
            {
                teamManager.yourTeam[i].healthBar = healthbars[i];
            }
            else
            {
                teamManager.enemyTeam[i - healthbars.Length / 2].healthBar = healthbars[i];
            }
        }
        foreach (var character in teamManager.enemyTeam)
        {
            character.FindAggroTarget();
            character.healthBar.maxValue = character.health;
            character.healthBar.value = character.health;
            character.healthBar.transform.parent.position = Camera.main.WorldToScreenPoint(character.transform.position
                                                                                           + new Vector3(0, 1.5f, 0));
        }
        foreach (var character in teamManager.yourTeam)
        {
            character.FindAggroTarget();
            character.healthBar.maxValue = character.health;
            character.healthBar.value = character.health;
            character.healthBar.transform.parent.position = Camera.main.WorldToScreenPoint(character.transform.position
                                                                                           + new Vector3(0, 1.5f, 0));
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
        StopCoroutine(dragCheck);
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
        if (inventoryHover.isHolding)
        {
            TakeUnitFromInventory(inventoryHover.heldObject.GetComponent<InventCharButton>());
            inventoryHover.DropIt();
        }
        inventoryManager.AddInventoryCard(heldUnit.instanceNumber);
        inventoryManager.inventory.Add(inventoryManager.possibleCharacters[heldUnit.instanceNumber]);
        Destroy(heldUnit.gameObject);
        holdingUnit = false;
        heldUnit = null;
    }

    public void TakeUnitFromInventory(InventCharButton element)
    {
        heldUnit = inventoryManager.TakeFromInventory(element.gameObject).GetComponent<Character>();
        holdingUnit = true;
    }

    private void UnitDrag(Vector3 target)
    {
        RaycastHit hit; 
        Ray ray = Camera.main.ScreenPointToRay(target);
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
            eventPoint.position = target;
            gRayCaster.Raycast(eventPoint, castHits);
            if (castHits.Count > 0)
            {
                for (int i = 0; i < castHits.Count; i++)
                {
                    InventCharButton element = castHits[i].gameObject.GetComponentInParent<InventCharButton>();
                    if (element != null)
                    {
                        // take unit from inventory
                        if(element.showDetails)
                            inventoryManager.ToggleInventoryDetails(element.gameObject);
                        if (Input.mousePosition.x < Screen.width-250)
                        {
                            TakeUnitFromInventory(element);
                        }
                        else
                            inventoryHover.HoldThis(element.gameObject);
                        break;
                    }
                }
            }
        }
    }

    private void UnitRelease(Vector3 target)
    {
        RaycastHit hit; 
                Ray ray = Camera.main.ScreenPointToRay(target);
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
                            inventoryManager.inventory.Add(inventoryManager.possibleCharacters[targetTile.heldUnit.instanceNumber]);
                            teamManager.Remove(targetTile.heldUnit.gameObject);
                            Destroy(targetTile.heldUnit.gameObject);
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
                eventPoint.position = target;
                gRayCaster.Raycast(eventPoint, castHits);
                if (castHits.Count > 0)
                {
                    for (int i = 0; i < castHits.Count; i++)
                    {
                        if (castHits[i].gameObject.CompareTag("InventoryBoard"))
                        {
                            // place into inventory, remove from board list
                            if (originTile != null)
                            {
                                inventoryManager.AddInventoryCard(originTile.heldUnit.instanceNumber);
                                teamManager.Remove(originTile.heldUnit.gameObject);
                                inventoryManager.inventory.Add(inventoryManager.possibleCharacters[originTile.heldUnit.instanceNumber]);
                                Destroy(originTile.heldUnit.gameObject);
                                originTile.heldUnit = null;
                                originTile = null;
                                fromTile = false;
                                holdingUnit = false;
                            }
                            else
                            {
                                DropInventoryUnit();
                            }
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

    IEnumerator waitToConfirmDrag(float waitTime, Vector3 pointerPosition)
    {
        yield return new WaitForSeconds(waitTime);
        if (Input.GetMouseButton(0))
        {
            UnitDrag(pointerPosition);
        }
        else
        {
            // Debug.Log("I was only clicked");
            List<RaycastResult> castHits = new List<RaycastResult>();
            PointerEventData eventPoint = new PointerEventData(eventSystem);
            eventPoint.position = pointerPosition;
            gRayCaster.Raycast(eventPoint, castHits);
            if (castHits.Count > 0)
            {
                for (int i = 0; i < castHits.Count; i++)
                {
                    // Debug.Log("I hit " + castHits[i].gameObject.name);
                    InventCharButton element = castHits[i].gameObject.GetComponentInParent<InventCharButton>();
                    if (element != null)
                    {
                        inventoryManager.ToggleInventoryDetails(element.gameObject);
                        break;
                    }
                }
            }
        }
    }
}
