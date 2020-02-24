using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Swiper : Character
{
    private int hitCount = 1;
    public float swipeRange = 3;
    public float swipeRadius = 90;
    public override void Attack()
    {
        if (hitCount < 3 - Convert.ToInt32(isUpgraded))
        {
            hitCount++;
            StartCoroutine(FakeAttackAnimation(.2f));
            if (attackDamage > aggroTarget.defense)
                if (aggroTarget.Damage(attackDamage - aggroTarget.defense))
                {
                    // upgrade would go here, the following is placeholder
                    if (!isUpgraded)
                    {
                        Upgrade();
                    }
                }
        }
        else // swipe
        {
            hitCount = 1;
            StartCoroutine(FakeAttackAnimation2(.2f));
            if(isOnYourTeam)
                foreach (var unit in teamManager.enemyTeam)
                {
                    float angle = Mathf.Abs(Vector3.Angle(transform.forward, unit.transform.position));
                    float dist = (unit.transform.position - transform.position).magnitude;
                    if (angle <= swipeRadius && dist <= swipeRange)
                    {
                        if (unit.Damage(attackDamage - unit.defense))
                        {
                            // upgrade would go here, the following is placeholder
                            if (!isUpgraded)
                            {
                                Upgrade();
                            }
                        }
                    }
                }
            else
            {
                foreach (var unit in teamManager.yourTeam)
                {
                    float angle = Mathf.Abs(Vector3.Angle(transform.forward, unit.transform.position));
                    float dist = (unit.transform.position - transform.position).magnitude;
                    if (angle <= swipeRadius && dist <= swipeRange)
                    {
                        if (unit.Damage(attackDamage - unit.defense))
                        {
                            // upgrade would go here, the following is placeholder
                            if (!isUpgraded)
                            {
                                Upgrade();
                            }
                        }
                    }
                }
            }
        }
    }

    public override void Upgrade()
    {
        isUpgraded = true;
        transform.LeanScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f);
    }
    
    public virtual IEnumerator FakeAttackAnimation2(float animationLength)
    {
        Vector3 direction = transform.forward * .3f;
        transform.LeanMove(transform.position - direction, animationLength/2);
        transform.LeanRotate(transform.rotation.eulerAngles+new Vector3(0,-90,0), animationLength / 2);
        yield return new WaitForSeconds(animationLength/2);
        transform.LeanMove(transform.position + direction, animationLength/2);
        transform.LeanRotate(transform.rotation.eulerAngles+new Vector3(0,180,0), animationLength / 2);
    }
}
