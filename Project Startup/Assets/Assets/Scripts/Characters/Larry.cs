using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Larry : Character
{

    public float protectioncheckCD = 0.1f;
    private float protCheckCurrent = -2;
    private float oldSpeed;
    private int saves = 0;
    
    public override void Update()
    {
        base.Update();
        if (protCheckCurrent < protectioncheckCD)
        {
            protCheckCurrent += Time.deltaTime;
        }
        else
        {
            protCheckCurrent = 0;
            if (saves < 1 + Convert.ToInt32(isUpgraded))
            {
                Character[] results = ProtectCheck();
                if (results != null)
                {
                    StartCoroutine(ProtectJump(results));
                }
            }
        }
    }

    private Character[] ProtectCheck()
    {
        switch (isOnYourTeam)
        {
            case true:
                float furthest = 0;
                int furthestIndex = teamManager.yourTeam.Length+1;
                for (int i = 0; i < teamManager.yourTeam.Length; i++)
                {
                    if(teamManager.yourTeam[i] != null)
                        if (teamManager.yourTeam[i].isDead == false)
                        {
                            // Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                            Vector3 distanceVector = teamManager.yourTeam[i].transform.position - transform.position;
                            if (furthest < distanceVector.magnitude)
                            {
                                furthest = distanceVector.magnitude;
                                furthestIndex = i;
                            }
                        }
                }

                foreach (var unit in teamManager.enemyTeam)
                {
                    if (unit.aggroTarget == teamManager.yourTeam[furthestIndex])
                    {
                        Character[] outArray = new Character[2] {teamManager.yourTeam[furthestIndex], unit};
                        return outArray;
                    }
                }
                return null;
            case false:
                float furthest1 = 0;
                int furthestIndex1 = teamManager.enemyTeam.Length+1;
                for (int i = 0; i < teamManager.enemyTeam.Length; i++)
                {
                    if(teamManager.enemyTeam[i] != null)
                        if (teamManager.enemyTeam[i].isDead == false)
                        {
                            // Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                            Vector3 distanceVector = teamManager.enemyTeam[i].transform.position - transform.position;
                            if (furthest1 < distanceVector.magnitude)
                            {
                                furthest1 = distanceVector.magnitude;
                                furthestIndex1 = i;
                            }
                        }
                }

                foreach (var unit in teamManager.yourTeam)
                {
                    if (unit.aggroTarget == teamManager.enemyTeam[furthestIndex1])
                    {
                        Character[] outArray = new Character[2] {teamManager.yourTeam[furthestIndex1], unit};
                        return outArray;
                    }
                }
                return null;
            default: return null;
        }
    }

    private IEnumerator ProtectJump(Character[] data)
    {
        saves++;
        oldSpeed = movementSpeed;
        movementSpeed = 0;
        aggroTarget = data[1];
        transform.LeanMoveX(data[0].transform.position.x, .4f);
        transform.LeanMoveZ(data[0].transform.position.z, .4f);
        transform.LeanMoveY(transform.position.y + 2, .2f);
        yield return new WaitForSeconds(.2f);
        transform.LeanMoveY(transform.position.y - 2, .2f);
        yield return new WaitForSeconds(.2f);
        data[1].aggroTarget = this;
        movementSpeed = oldSpeed;
        transform.Translate(transform.forward*.3f);
    }
}