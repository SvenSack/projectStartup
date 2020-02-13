using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public bool inventoryOpen;
    public Transform inventoryButton;
    
    // Start is called before the first frame update
    void Start()
    {
        castMask = LayerMask.GetMask("Tiles", "UI");
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
                    GameObject target = hit.transform.gameObject;
                    if (target.layer == 8)
                    {
                        if (hit.transform.gameObject.GetComponent<Tile>().heldUnit != null && hit.transform.gameObject.GetComponent<Tile>().isYours)
                        {
                            originTile = hit.transform.gameObject.GetComponent<Tile>();
                            fromTile = true;
                            holdingUnit = true;
                        }
                    }

                    if (target.layer == 5)
                    {
                        // SVEN MACH HIER WEITER DU DEPP, du musst noch sicherstellen das es ein inventoryElement ist,
                        // dann das script finden, den index lesen, und inventoryManager.TakeFromInventory nehmen um das object
                        // in heldObject dieses scriptes zu speichern. Wenn es dann noch nicht funktioniert, gib mir die schuld ^^"
                    }
                }
            }
            if ( Input.GetMouseButtonUp (0)){ 
                RaycastHit hit; 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if ( Physics.Raycast (ray,out hit,100.0f, castMask))
                {
                    if (fromTile && hit.transform.gameObject.layer == 8)
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
                        }
                    }
                    else if (!fromTile && hit.transform.gameObject.layer == 8 && holdingUnit)
                    {
                        Tile targetTile = hit.transform.gameObject.GetComponent<Tile>();
                        if (targetTile.heldUnit != null && targetTile.isYours)
                        {
                            // swap unit with held unit
                            inventoryManager.AddInventoryCard(targetTile.heldUnit.instanceNumber);
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
                                inventoryManager.AddInventoryCard(heldUnit.instanceNumber);
                            }
                            else
                            {
                                holdingUnit = false;
                                heldUnit = null;
                            }
                        }
                        else
                        {
                            // snap unit back into inventory and give feedback that team is full
                            inventoryManager.AddInventoryCard(heldUnit.instanceNumber);
                        }
                    }
                    else if (hit.transform.gameObject.layer == 5 && hit.transform.gameObject.CompareTag("InventoryBoard"))
                    {
                        // place into inventory, remove from board list
                        inventoryManager.AddInventoryCard(originTile.heldUnit.instanceNumber);
                        teamManager.Remove(originTile.heldUnit.gameObject);
                        holdingUnit = false;
                    }
                }
                else
                    DropUnit();
                
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
        holdingUnit = false;
    }
}
