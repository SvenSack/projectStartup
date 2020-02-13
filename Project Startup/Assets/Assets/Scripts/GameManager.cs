using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool fightRunning;
    public TeamManager teamManager;

    private int castMask;

    private Tile originTile;
    private bool fromTile;
    
    // Start is called before the first frame update
    void Start()
    {
        castMask = LayerMask.GetMask("Tiles");
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
                    if (hit.transform.gameObject.GetComponent<Tile>().heldUnit != null)
                    {
                        originTile = hit.transform.gameObject.GetComponent<Tile>();
                        fromTile = true;
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
            }
        }
        
    }

    public void StartFight()
    {
        fightRunning = true;
        foreach (var character in teamManager.enemyTeam)
        {
            character.FindAggroTarget();
        }
        foreach (var character in teamManager.yourTeam)
        {
            character.FindAggroTarget();
        }
    }
    
    
}
