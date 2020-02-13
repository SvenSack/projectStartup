using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool FightRunning;
    public TeamManager teamManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFight()
    {
        FightRunning = true;
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
