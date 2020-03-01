using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public bool fightRunning; // bool tracking if the fight is currently running
    [HideInInspector] public bool endScreen; // bool tracking if you are currently on the end screen
    public TeamManager teamManager;
    public InventoryManager inventoryManager;

    public bool notSetup = true; // bool checking if the element has been setup properly

    private int castMask; // the mask for the tile raycast
    private int floorMask; // the mask for the floor raycast (for the mouse follow)

    private Tile originTile; // the tile the unit you are holding came from
    private bool fromTile; // bool tracking if the unit is from a tile
    private bool holdingUnit; // bool tracking if you are currently holding a unit
    private Character heldUnit; // the unit you are currently holding (only from inventory)
    private GameObject hideInFight; // the UI for the pre-fight setup
    private GameObject showInFight; // the UI for the combat
    private GameObject showOnVictory; // the UI for the victory screen
    private GameObject showOnDefeat; // the UI for the defeat screen
    public GraphicRaycaster gRayCaster;
    public EventSystem eventSystem;
    private bool showingDetails;
    private Character detailShown;
    public GameObject detailShower;
    public Transform uiScroll;

    [HideInInspector] public bool inventoryOpen; // bool tracking if the inventory is open
    public Transform inventoryButton; // the inventory open/close button
    public InventoryHover inventoryHover;

    public Transform tileBoard; // the tileboard (the group with all the tiles)

    private bool checkingDrag; // bool tracking if you are currently waiting for a drag check
    private Coroutine dragCheck;

    private GameObject music;
    public GameObject startFightMusicBtn;
    public GameObject stopFightMusicBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        castMask = LayerMask.GetMask("Tiles");
        floorMask = LayerMask.GetMask("Floor");
        hideInFight = GameObject.FindGameObjectWithTag("HideInFight");
        showInFight = GameObject.FindGameObjectWithTag("ShowInFight");
        showOnVictory = GameObject.FindGameObjectWithTag("ShowOnVictory");
        showOnDefeat = GameObject.FindGameObjectWithTag("ShowOnDefeat");
        showInFight.SetActive(false);
        showOnVictory.SetActive(false);
        showOnDefeat.SetActive(false);
        detailShower.GetComponent<InventCharButton>().stats.SetActive(true);
        detailShower.GetComponent<InventCharButton>().stats.SetActive(true);
        detailShower.SetActive(false);
        
        music = GameObject.FindGameObjectWithTag("music");
        
        // Adding listeners to buttons
        startFightMusicBtn.GetComponent<Button>().onClick.AddListener(StartFMusic);

        stopFightMusicBtn.GetComponent<Button>().onClick.AddListener(StartMMusic);
    }

    private void StartFMusic()
    {
        //Debug.Log("I added a listener");
        music.GetComponent<ChangingMusic>().StartFightMusic();
    }

    private void StartMMusic()
    {
        //Debug.Log("I added a listener");
        music.GetComponent<ChangingMusic>().StartMainMusicAgain();
    }

    // Update is called once per frame
    void Update()
    {
        if (!fightRunning && !endScreen)
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

    #region Start Fight

    public void StartFight()
    {
        fightRunning = true;
        hideInFight.SetActive(false);
        showInFight.SetActive(true);
        Slider[] healthbars = showInFight.transform.GetChild(0).gameObject.GetComponentsInChildren<Slider>();
        for (int i = 0; i < healthbars.Length; i++)
        {
            if (i < healthbars.Length/2)
            {
                if(teamManager.yourTeam[i] != null)
                    teamManager.yourTeam[i].healthBar = healthbars[i];
                else
                    healthbars[i].transform.parent.position = new Vector3(Screen.width *2, 0,0);
            }
            else
            {
                if(teamManager.enemyTeam[i - healthbars.Length / 2] != null)
                    teamManager.enemyTeam[i - healthbars.Length / 2].healthBar = healthbars[i];
                else
                    healthbars[i].transform.parent.position = new Vector3(Screen.width *2, 0,0);
            }
        }
        foreach (var character in teamManager.enemyTeam)
        {
            if (character != null)
            {
                character.FindAggroTarget();
                character.healthBar.maxValue = character.health;
                character.healthBar.value = character.health;
                character.healthBar.transform.parent.position = Camera.main.WorldToScreenPoint(character.transform.position
                                                                                               + new Vector3(0, 1.5f, 0));
            }
        }
        foreach (var character in teamManager.yourTeam)
        {
            if (character != null)
            {
                character.FindAggroTarget();
                character.healthBar.maxValue = character.health;
                character.healthBar.value = character.health;
                character.healthBar.transform.parent.position = Camera.main.WorldToScreenPoint(character.transform.position
                                                                                               + new Vector3(0, 1.5f, 0));
            }
        }

        Tile[] checkGroup = FindObjectsOfType<Tile>();
        foreach (var tile in checkGroup)
        {
            tile.GetComponent<MeshRenderer>().material = tile.off;
        }
    }

    #endregion

    #region Unit placement and Inventory

    public void ToggleInventory()
    {
        if (inventoryOpen)
        {
            inventoryButton.parent.LeanMoveX(Screen.width,.5f);
            uiScroll.LeanRotate(new Vector3(0, 120, 0), .5f);
        }
        else
        {
            inventoryButton.parent.LeanMoveX(Screen.width - 440,.5f);
            uiScroll.LeanRotate(new Vector3(0, -120, 0), .5f);
        }
        StopCoroutine(dragCheck);
        inventoryOpen = !inventoryOpen;
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
        inventoryManager.AddCard(heldUnit.instanceNumber);
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
                    if (element.showDetails)
                    {
                        inventoryManager.ToggleInventoryDetails(element.gameObject);
                    }
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

    private void UnitRelease(Vector3 target)
    {
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
                        inventoryManager.AddCard(originTile.heldUnit.instanceNumber);
                        teamManager.Remove(originTile.heldUnit.gameObject);
                        Destroy(originTile.heldUnit.gameObject);
                        originTile.heldUnit = null;
                        originTile.CenterUnit();
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
                    inventoryManager.AddCard(targetTile.heldUnit.instanceNumber);
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
        showingDetails = false;
        detailShower.SetActive(false);
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
            else
            {
                RaycastHit hit; 
                Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
                if (Physics.Raycast(ray, out hit, 100.0f, castMask))
                {
                    Tile target = hit.transform.GetComponent<Tile>();
                    if (target.heldUnit != null)
                    {
                        ShowDetails(target.heldUnit);
                    }
                }
            }
        }
    }

    private void ShowDetails(Character character)
    {
        showingDetails = true;
        detailShower.SetActive(true);
        detailShower.GetComponent<InventCharButton>().Set(character.name, character.instanceNumber,character.profilePic,character.type,character.ability,true);
    }
    #endregion

    #region Post Combat

    public void FightOver(bool youWon)
    {
        fightRunning = false;
        endScreen = true;
        showInFight.SetActive(false);
        
        music.GetComponent<ChangingMusic>().StartWinMusic();
        if (youWon)
        {
            // display victory screen
            showOnVictory.SetActive(true);
            teamManager.WriteTeamFile(true);
        }
        else
        {
            // display loss screen
            showOnDefeat.SetActive(true);
        }
    }

    public void Retry()
    {
        endScreen = false;
        showOnDefeat.SetActive(false);
        hideInFight.SetActive(true);
        Tile[] tiles = GameObject.FindGameObjectWithTag("TileBoard").GetComponentsInChildren<Tile>();
        teamManager.yourTeam = new Character[3];
        teamManager.enemyTeam = new Character[3];

        foreach (var tile in tiles)
        {
            if (tile.heldUnit != null)
            {
                tile.heldUnit.healthBar.transform.parent.gameObject.SetActive(true);
                Character newChar = inventoryManager.NewCharacter(tile.heldUnit.instanceNumber);
                bool oldAlignment = tile.heldUnit.isOnYourTeam;
                newChar.isOnYourTeam = oldAlignment;
                Destroy(tile.heldUnit.gameObject);
                tile.UnitPlace(newChar, true);
                teamManager.Add(newChar, !newChar.isOnYourTeam);
            }
        }
    }

    #endregion
}
