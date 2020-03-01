using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [HideInInspector] public Character[] yourTeam = new Character[3]; // your currently placed units
    [HideInInspector] public Character[] enemyTeam = new Character[3]; // the units on the enemy team
    public GameObject[] yourTileRows;
    public GameObject[] enemyTileRows;

    public TextMeshProUGUI unitCount;
    // Start is called before the first frame update
    void Start()
    {
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
        string json = File.ReadAllText(Application.dataPath + "/saveFile.json");
        TeamData teamData = JsonUtility.FromJson<TeamData>(json);
        int[][] data = teamData.data;
        for (int i = 0; i < data.Length; i++)
        {
            
        }
    }

    public void WriteTeamFile()
    {
        TeamData dat = new TeamData();
        string jsonData = JsonUtility.ToJson(dat);
        //File.WriteAllText(Application.dataPath + "/saveFile.json", jsonData);
    }
    
    private class TeamData
    {
        public int[][] data = new int[3][]{new []{0,2,3}, new []{1,1,1}, new []{2,2,4}};
    }
}
