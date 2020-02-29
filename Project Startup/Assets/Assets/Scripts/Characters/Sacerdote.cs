using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sacerdote : Character
{
    private int hitCounter;
    public float healRadius = 2;
    
    public override void Attack()
    {
        base.Attack();
        if (hitCounter < 1 - Convert.ToInt32(isUpgraded))
        {
            hitCounter++;
        }
        else
        {
            hitCounter = 0;
            if((attackDamage - aggroTarget.defense) > 0)
                HealAlly(attackDamage - aggroTarget.defense);
        }
    }

    private void HealAlly(float amount)
    {
        switch (isOnYourTeam)
        {
            case true:
                foreach (var unit in teamManager.yourTeam)
                {
                    if(unit != null)
                        if (unit != this && !unit.isDead)
                        {
                            if ((unit.transform.position - transform.position).magnitude < healRadius)
                            {
                                unit.Heal(amount);
                                break;
                            }
                        }
                }

                break;
            case false:
                foreach (var unit in teamManager.enemyTeam)
                {
                    if(unit != null)
                        if (unit != this && !unit.isDead)
                        {
                            if ((unit.transform.position - transform.position).magnitude < healRadius)
                            {
                                unit.Heal(amount);
                                break;
                            }
                        }
                }

                break;
        }
    }
}
