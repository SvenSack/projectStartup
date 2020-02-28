using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Random = UnityEngine.Random;

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
            jumps++;
            movementSpeed = movementSpeed * 3;
            zeroHeight = transform.position.y;
            transform.LeanMoveY(transform.position.y + 3, 0.2f);
        }
    }

    public override void Upgrade()
    {
        base.Upgrade();
        isJumping = true;
        jumps++;
        movementSpeed = movementSpeed * 3;
        zeroHeight = transform.position.y;
        transform.LeanMoveY(transform.position.y + 3, 0.2f);
    }
    
    public override bool Damage(float amount)
    {
        if (amount < 0)
            amount = 0;
        health -= amount;
        if (health > 0)
        {
            GameObject newDamage = Instantiate(damageText,  Camera.main.WorldToScreenPoint(transform.position + new Vector3(Random.Range(-.5f,.5f),
                                                                                               1, 0)), Quaternion.identity, FindObjectOfType<Canvas>().transform);
            TextMeshProUGUI textMesh = newDamage.GetComponent<TextMeshProUGUI>();
            AttackText newText = newDamage.GetComponent<AttackText>();
            if (amount > 0)
            {
                // FindAggroTarget();
                textMesh.text = Mathf.RoundToInt(amount*10) + " !";
                textMesh.color = new Color(0.7924528f, 0.1831613f, 0.2159052f);
                newText.baseColor = new Color(0.7924528f, 0.1831613f, 0.2159052f);
            }
            else
            {
                textMesh.text = "Blocked !";
            }
            healthBar.value = health;
            return false;
        }
        else
        {
            Die();
            return true;
        }
    }
}
