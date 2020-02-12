using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Range(1.0f, 10.0f)] public float range = 1.0f;
    [Range(1.0f, 10.0f)] public float movementSpeed = 1.0f;
    [Range(1.0f, 100.0f)] public float health = 1.0f;
    [Range(1.0f, 10.0f)] public float attackDamage = 1.0f;
    [Range(0.1f, 4.0f)] public float attackSpeed = 1.0f;
    [Range(1.0f, 10.0f)] public float defense = 1.0f;
    public string name = "Sven, greatest of all Programmers";
    private bool fighting;
    private float attackCooldown;

    private TeamManager teamManager;
    
    public bool isOnYourTeam;
    public bool isDead;
    public Character aggroTarget;


    // Start is called before the first frame update
    void Start()
    {
        teamManager = GameObject.FindGameObjectWithTag("TeamManager").GetComponent<TeamManager>();
        if(!isDead)
            FindAggroTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (health <= 0)
            {
                Die();
            }
        
        
            if (!fighting)
            {
                // move towards aggroTarget at movementSpeed
                // check if aggroTarget TargetInRange if yes, make fighting true
            }

            if (fighting)
            {
                // check if aggroTarget TargetInRange, if yes, check attackCooldown against attack speed if yes Attack, else increase by deltatime, else make fighting false
            }
        }
    }

    public void FindAggroTarget()
    {
        // run through all enemies that are !isDead, assign the closest one to aggroTarget
        switch (isOnYourTeam)
        {
            case true:
                float closest = 0;
                int closestIndex = teamManager.enemyTeam.Length+1;
                for (int i = 0; i < teamManager.enemyTeam.Length; i++)
                {
                    if (teamManager.enemyTeam[i].isDead == false)
                    {
                        //Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                        Vector3 distanceVector = transform.position - teamManager.enemyTeam[i].transform.position;
                        if (closest == 0 || closest > distanceVector.magnitude)
                        {
                            closest = distanceVector.magnitude;
                            closestIndex = i;
                        }
                    }
                }
                aggroTarget = teamManager.enemyTeam[closestIndex];
                break;
            case false:
                float closest1 = 0;
                int closestIndex1 = teamManager.yourTeam.Length+1;
                for (int i = 0; i < teamManager.yourTeam.Length; i++)
                {
                    if (teamManager.yourTeam[i].isDead == false)
                    {
                        //Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                        Vector3 distanceVector = transform.position - teamManager.yourTeam[i].transform.position;
                        if (closest1 == 0 || closest1 > distanceVector.magnitude)
                        {
                            closest1 = distanceVector.magnitude;
                            closestIndex1 = i;
                        }
                    }
                }
                aggroTarget = teamManager.yourTeam[closestIndex1];
                Debug.Log(name + " has decided to target " + aggroTarget.name);
                break;
        }
    }

    public bool TargetInRange(Character Target)
    {
        // check if target is in range, return accordingly
        return true;
    }

    private void Attack()
    {
        // deal damage to the target mitigated by defense
    }

    private void Die()
    {
        isDead = true;
        // check if all on my team are dead, if yes, finish game
        // run through all living characters, check if they have me as their AggroTarget, if yes, make them FindAggroTarget
    }
}
