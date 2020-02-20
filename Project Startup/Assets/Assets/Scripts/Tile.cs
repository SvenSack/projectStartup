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
            heldUnit = unit;
            CenterUnit();
            return true;
        }

        return false;
    }
    
    public void UnitPlace(Character unit, bool ignoreRules)
    {
        // place unit regardless of other factors
        if (ignoreRules)
        {
            heldUnit = unit;
            CenterUnit();
        }
    }

    public void CenterUnit()
    {
        if (heldUnit != null)
        {
            heldUnit.transform.position = placementSpot.position;
            if (!heldUnit.isOnYourTeam)
            {
                heldUnit.transform.eulerAngles = new Vector3(0,180,0);
            }
        }
    }
}
