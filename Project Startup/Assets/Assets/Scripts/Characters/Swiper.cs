using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Debug = System.Diagnostics.Debug;

public class Swiper : Character
{
    private int hitCount = 1;
    public float swipeRange = 3;
    public float swipeRadius = 90;
    private Animator anim;

    public override void Start()
    {
        base.Start();
        anim = GetComponentInChildren<Animator>();
    }
    
    public override void Update()
    {
        if (!isDead && gameManager.fightRunning)
        {
            Rotate();
        
            if (!fighting)
            {
                if (anim.GetBool("Walking") == false)
                {
                    anim.SetBool("Walking", true);
                }
                Move();
                if (TargetInRange(aggroTarget))
                {
                    anim.SetBool("Walking", false);
                    fighting = true;
                }
            }

            if (fighting)
            {
                // check if aggroTarget TargetInRange, if yes, check attackCooldown against attack speed if yes Attack, else increase by deltatime, else make fighting false
                if (TargetInRange(aggroTarget))
                {
                    if (attackCooldownValue >= attackCooldown)
                    {
                        attackCooldownValue = 0;
                        Attack();
                    }
                    else
                    {
                        attackCooldownValue += Time.deltaTime;
                    }
                }
                else
                {
                    anim.SetBool("Walking", true);
                    fighting = false;
                }
            }
        }
    }

    public override void Attack()
    {
        
        if (hitCount < 3 - Convert.ToInt32(isUpgraded))
        {
            // Play Hit SFX
            hitSfx.Play();
            
            hitCount++;
            // StartCoroutine(FakeAttackAnimation(.2f));
            anim.SetTrigger("Attack");
            if (attackDamage > aggroTarget.defense)
                if (aggroTarget.Damage(attackDamage - aggroTarget.defense))
                {
                    // upgrade would go here, the following is placeholder
                    if (!isUpgraded)
                    {
                        StartCoroutine(ClaimDeath());
                        Upgrade();
                    }
                }
        }
        else // swipe
        {
            hitCount = 1;

            // StartCoroutine(FakeAttackAnimation2(.2f));
            anim.SetTrigger("Swipe");
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
