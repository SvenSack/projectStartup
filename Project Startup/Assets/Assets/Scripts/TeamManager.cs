using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class TeamManager : MonoBehaviour
{
    public Character[] yourTeam = new Character[3]; // your currently placed units
    public Character[] enemyTeam = new Character[3]; // the units on the enemy team
    public GameObject[] yourTileRows;
    public GameObject[] enemyTileRows;
    public InventoryManager inventoryManager;

    public TextMeshProUGUI unitCount;
    // Start is called before the first frame update
    void Start()
    {
        ReadTeamFile();
        // get all characters, sort them into the teams according to their isOnYourTeam
        Character[] characters = FindObjectsOfType<Character>();
        int counter = 0;
        foreach (var character in characters)
        {
            if (character.isOnYourTeam)
            {
                Add(character, false);
                counter++;
            }
            else
                Add(character, true);
        }

        unitCount.text = counter + unitCount.text.TrimStart(unitCount.text.ToCharArray()[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Remove(GameObject character)
    {
        for (int i = 0; i < yourTeam.Length; i++)
        {
            if(yourTeam[i] != null)
                if (yourTeam[i].gameObject == character)
                {
                    yourTeam[i] = null;
                    break;
                }
        }
        UpdateNumber();
    }

    public void Add(Character character)
    {
        for (int i = 0; i < yourTeam.Length; i++)
        {
            if (yourTeam[i] == null)
            {
                yourTeam[i] = character;
                break;
            }
        }
        UpdateNumber();
    }
    
    public void Add(Character character, bool isEnemy)
    {
        if (!isEnemy)
        {
            for (int i = 0; i < yourTeam.Length; i++)
            {
                if (yourTeam[i] == null)
                {
                    yourTeam[i] = character;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < enemyTeam.Length; i++)
            {
                if (enemyTeam[i] == null)
                {
                    enemyTeam[i] = character;
                    break;
                }
            }
        }
    }

    public bool CheckSpot()
    {
        for (int i = 0; i < yourTeam.Length; i++)
        {
            if (yourTeam[i] == null)
            {
                return true;
            }
        }

        StartCoroutine(GentleReminder());
        return false;
    }

    private void UpdateNumber()
    {
        Character[] characters = FindObjectsOfType<Character>();
        int counter = 0;
        foreach (var charact in yourTeam)
        {
            if (charact != null)
            {
                counter++;
            }
        }

        unitCount.text = counter + unitCount.text.TrimStart(unitCount.text.ToCharArray()[0]);
    }

    private IEnumerator GentleReminder()
    {
        unitCount.transform.LeanScale(new Vector3(2, 2, 2), .3f);
        unitCount.color = new Color(0.7924528f, 0.1831613f, 0.2159052f);
        yield return new WaitForSeconds(0.3f);
        unitCount.color = new Color(50f/255f, 50f/255f, 50f/255f);
        unitCount.transform.LeanScale(new Vector3(1, 1, 1), .1f);
    }

    public void ReadTeamFile()
    {
        Tile[] tilesRow1 = enemyTileRows[0].GetComponentsInChildren<Tile>();
        Tile[] tilesRow2 = enemyTileRows[1].GetComponentsInChildren<Tile>();
        Tile[] tilesRow3 = enemyTileRows[2].GetComponentsInChildren<Tile>();
        Tile[][] tileRows = new Tile[][]{tilesRow1,tilesRow2,tilesRow3};
        string json = File.ReadAllText(Application.dataPath + "/EnemyTeam.json");
        TeamData teamData = JsonUtility.FromJson<TeamData>(json);
        TeamData.Entry[] data = teamData.data;
        for (int i = 0; i < data.Length; i++)
        {
            Tile[] targetRow = tileRows[data[i].collumn];
            Character newChar = inventoryManager.NewCharacter(data[i].unit);
            newChar.isOnYourTeam = false;
            targetRow[data[i].row].heldUnit = newChar;
            if(enemyTeam[i] != null)Destroy(enemyTeam[i].gameObject);
            enemyTeam[i] = newChar;
            targetRow[data[i].row].CenterUnit();
        }
    }

    public void WriteTeamFile(bool stealPlayerTeam)
    {
        Tile[] tilesRow1 = enemyTileRows[0].GetComponentsInChildren<Tile>();
        Tile[] tilesRow2 = enemyTileRows[1].GetComponentsInChildren<Tile>();
        Tile[] tilesRow3 = enemyTileRows[2].GetComponentsInChildren<Tile>();
        Tile[][] tileRows = new Tile[][]{tilesRow1,tilesRow2,tilesRow3};
        Tile[] tilesRowy1 = yourTileRows[0].GetComponentsInChildren<Tile>();
        Tile[] tilesRowy2 = yourTileRows[1].GetComponentsInChildren<Tile>();
        Tile[] tilesRowy3 = yourTileRows[2].GetComponentsInChildren<Tile>();
        Tile[][] tileRowsY = new Tile[][]{tilesRowy1,tilesRowy2,tilesRowy3};
        TeamData dat = new TeamData();
        if (stealPlayerTeam)
        {
            for (int i = 0; i < yourTeam.Length; i++)
            {
                if (yourTeam[i] != null)
                {
                    int collumn = 0;
                    int row = 0;
                    for (int j = 0; j < tileRowsY.Length; j++)
                    {
                        for (int k = 0; k < tileRowsY[j].Length; k++)
                        {
                            if (tileRowsY[j][k].heldUnit == yourTeam[i])
                            {
                                collumn = j;
                                row = k;
                                break;
                            }
                        }
                    }
                    TeamData.Entry entry = new TeamData.Entry();
                    entry.collumn = collumn;
                    entry.row = row;
                    entry.unit = yourTeam[i].instanceNumber;
                    dat.data[i] = entry;
                }
                else
                {
                    TeamData.Entry entry = new TeamData.Entry();
                    entry.collumn = i;
                    entry.row = UnityEngine.Random.Range(0, tileRows[i].Length);
                    entry.unit = UnityEngine.Random.Range(0, inventoryManager.possibleCharacters.Length);
                    dat.data[i] = entry;
                }
                
            }
        }
        string jsonData = JsonUtility.ToJson(dat);
        // print(jsonData);
        File.WriteAllText(Application.dataPath + "/EnemyTeam.json", jsonData);
    }
    
    [System.Serializable] public class TeamData
    {
        public Entry[] data = new Entry[3];

        [System.Serializable]
        public class Entry
        {
            public int collumn;
            public int row;
            public int unit;
        }
    }
}
