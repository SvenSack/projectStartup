using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fronpy : Character
{
    private int jumps = 0;
    private bool isJumping;
    private float zeroHeight;

    public override bool TargetInRange(Character target)
    {
        // check if target is in range, return accordingly
        Vector3 targetVector = target.transform.position - transform.position;
        // Debug.Log(name + "says his target is " + targetVector.magnitude + " away");
        if (targetVector.magnitude <= range)
        {
            if (isJumping)
            {
                isJumping = false;
                movementSpeed = movementSpeed / 3;
                transform.LeanMoveY(zeroHeight, 0.2f);
            }
            return true;
        }
        return false;
    }

    public override void FindAggroTarget()
    {
        switch (isOnYourTeam)
        {
            case true:
                float closest = 0;
                int closestIndex = teamManager.enemyTeam.Length + 1;
                for (int i = 0; i < teamManager.enemyTeam.Length; i++)
                {
                    if (teamManager.enemyTeam[i] != null)
                        if (teamManager.enemyTeam[i].isDead == false)
                        {
                            // Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                            Vector3 distanceVector = teamManager.enemyTeam[i].transform.position - transform.position;
                            if (closest == 0 || closest < distanceVector.magnitude)
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
                int closestIndex1 = teamManager.yourTeam.Length + 1;
                for (int i = 0; i < teamManager.yourTeam.Length; i++)
                {
                    if (teamManager.yourTeam[i] != null)
                        if (teamManager.yourTeam[i].isDead == false)
                        {
                            // Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                            Vector3 distanceVector = teamManager.yourTeam[i].transform.position - transform.position;
                            if (closest1 == 0 || closest1 < distanceVector.magnitude)
                            {
                                closest1 = distanceVector.magnitude;
                                closestIndex1 = i;
                            }
                        }
                }

                aggroTarget = teamManager.yourTeam[closestIndex1];
                // Debug.Log(name + " has decided to target " + aggroTarget.name);
                break;
        }

        if (jumps < 1 + Convert.ToInt32(isUpgraded))
        {
            isJumping = true;
            movementSpeed = movementSpeed * 3;
            zeroHeight = transform.position.y;
            transform.LeanMoveY(transform.position.y + 3, 0.2f);
        }
    }

    public override void Upgrade()
    {
        isUpgraded = true;
        transform.LeanScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f);
    }
}
