using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Friend : Character
{
    private int hugs = 0;
    private bool isHugging;
    private float oldSpeed;

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

        if (hugs < 1 + Convert.ToInt32(isUpgraded))
        {
            isHugging = true;
            hugs++;
            oldSpeed = movementSpeed;
            movementSpeed = 0;
            StartCoroutine(Hug());
        }
    }

    public override void Upgrade()
    {
        isUpgraded = true;
        transform.LeanScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f);
        isHugging = true;
        hugs++;
        oldSpeed = movementSpeed;
        movementSpeed = 0;
        StartCoroutine(Hug());
    }

    private IEnumerator Hug()
    {
        yield return new WaitForSeconds(0.3f);
        aggroTarget.transform.LeanMove(transform.position + transform.forward * range, 0.3f);
        yield return new WaitForSeconds(0.3f);
        isHugging = false;
        movementSpeed = oldSpeed;
        aggroTarget.healthBar.transform.parent.position = Camera.main.WorldToScreenPoint(aggroTarget.transform.position + new Vector3(0, 1.5f, 0));
    }
}
