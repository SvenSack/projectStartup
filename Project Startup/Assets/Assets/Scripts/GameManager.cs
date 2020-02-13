using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public bool fightRunning;
    public TeamManager teamManager;

    private int castMask;
    private int floorMask;

    private Tile originTile;
    private bool fromTile;
    private bool holdingUnit;
    private GameObject hideInFight;

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
                    // Debug.Log("You selected " + hit.transform.name);
                    if (hit.transform.gameObject.GetComponent<Tile>().heldUnit != null && hit.transform.gameObject.GetComponent<Tile>().isYours)
                    {
                        originTile = hit.transform.gameObject.GetComponent<Tile>();
                        fromTile = true;
                        holdingUnit = true;
                    }
                }
            }
            if ( Input.GetMouseButtonUp (0)){ 
                RaycastHit hit; 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if ( Physics.Raycast (ray,out hit,100.0f, castMask))
                {
                    if (fromTile)
                    {
                        Tile targetTile = hit.transform.gameObject.GetComponent<Tile>();
                        if (targetTile.UnitPlace(originTile.heldUnit, originTile) != true)
                        {
                            // snap unit back
                            DropUnit();
                        }
                    }
                    else
                    {
                        Tile targetTile = hit.transform.gameObject.GetComponent<Tile>();
                        if (targetTile.UnitPlace(originTile.heldUnit) != true)
                        {
                            // snap unit back into inventory
                        }
                    }
                }
                DropUnit();
                
            }
        }

        if (holdingUnit)
        {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, floorMask))
            {
                originTile.heldUnit.transform.position = hit.point + new Vector3(0, 0.5f, 0);
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
