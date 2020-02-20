using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public Character[] yourTeam = new Character[3];
    public Character[] enemyTeam = new Character[3];
    // Start is called before the first frame update
    void Start()
    {
        // get all characters, sort them into the teams according to their isOnYourTeam
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

        return false;
    }
}
