using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isYours;
    public Character heldUnit;

    private GameManager gameManager;
    private TeamManager teamManager;
    private Transform placementSpot;
    
    // Start is called before the first frame update
    void Start()
    {
        teamManager = GameObject.FindGameObjectWithTag("TeamManager").GetComponent<TeamManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        placementSpot = transform.GetChild(0).GetChild(0);
        if(heldUnit != null)
            CenterUnit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool UnitPlace(Character unit, Tile origin)
    {
        // check if I am on your team, if yes then swap me
        if (isYours)
        {
            origin.heldUnit = heldUnit;
            heldUnit = unit;
            if(origin.heldUnit != null)
                origin.CenterUnit();
            CenterUnit();
            return true;
        }

        return false;
    }
    
    public bool UnitPlace(Character unit)
    {
        // check if I am on your team, if yes then place me
        if (isYours)
        {
            // place the currently held unit back into the inventory
            heldUnit = unit;
            CenterUnit();
            return true;
        }

        return false;
    }

    public void CenterUnit()
    {
        heldUnit.transform.position = placementSpot.position;
    }
}
